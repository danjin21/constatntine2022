using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{

    public bool Blocked = false;


    public int Id { get; set; }

    public StatInfo _stat = new StatInfo();

    public bool IsDead = false;

    public List<PositionInfo> PosHistory = new List<PositionInfo>();

    public virtual StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.MergeFrom(value);

            //_stat.Hp = value.Hp;
            //_stat.MaxHp = value.MaxHp;
            //_stat.Speed = value.Speed;


        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }


    public virtual int Hp
    {
        get { return Stat.Hp; }
        set
        {
            Stat.Hp = value;
        }
    }

    public virtual int Mp
    {
        get { return Stat.Mp; }
        set
        {
            Stat.Mp = value;
        }
    }

    public virtual int MaxHp
    {
        get { return Stat.MaxHp; }
        set
        {
            Stat.MaxHp = value;
        }
    }

    public virtual int MaxMp
    {
        get { return Stat.MaxMp; }
        set
        {
            Stat.MaxMp = value;
        }
    }


    //[SerializeField]
    //public float _speed;

    protected bool _updated = false;



    [SerializeField]
    PositionInfo _positionInfo = new PositionInfo();

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;


            // 각자 부근에서 updated를 처리할 것이다.
            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;


        }
    }



    // 바로 이동 시켜주는것
    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);
        transform.position = destPos;

    }


    public Vector3Int CellPos
    {
        get
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            // Creature 일 경우에만 Map의 Object 리스트에 반영해준다.
            if (this.GetType() == typeof(PlayerController) ||
                this.GetType() == typeof(MyPlayerController) ||
                this.GetType() == typeof(MonsterController) ||
                   this.GetType() == typeof(NpcController)
                )
                //if (this.GetType() == typeof(CreatureController) )
                Managers.Map.ApplyMove(gameObject, PosInfo.PosX, PosInfo.PosY, value.x, value.y);

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;

            //// 진형추가
            //if (_sprite != null)
            //    _sprite.sortingOrder = -(int)value.y;

            // 진형추가
            if (_sprite != null)
                _sprite.sortingOrder = -(int)value.y;

            // 모든 레이어의 소팅 오더를 자기랑 똑같이
            foreach (Transform child in transform)
            {
                // SpriteRenderer가 없는 Canvas 등은 넘어감
                if (child.GetComponent<SpriteRenderer>() == null)
                {



                    // 캔버스인 경우 ( 닉네임이므로 오더를 같게 한다.)
                    if (child.GetComponent<Canvas>() != null)
                        child.GetComponent<Canvas>().sortingOrder = -(int)value.y;

  
                    if (child.transform.childCount == 0)
                        continue; // return; 하면 안됨!!!!!!!!!!!!
     

                    if (child.transform.GetChild(0).GetComponent<SpriteRenderer>() != null)
                        child.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = -(int)value.y;

                    if (child.transform.GetChild(0).GetComponent<ParticleSystem>() != null)
                        child.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = -(int)value.y;

    

      
                }
                else
                {

                    child.GetComponent<SpriteRenderer>().sortingOrder = -(int)value.y;
                }
            }
        }

    }



    protected Animator _animator;
    protected SpriteRenderer _sprite;


    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnimation();
            _updated = true;

        }
    }





    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value)
                return;

            PosInfo.MoveDir = value;


            // 진형 삭제 -> MyPlayer movenextmove에 있음
            //UpdateAnimation();
            _updated = true;

        }

    }



    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else /*if (dir.y < 0)*/
            return MoveDir.Down;


    }



    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;

        }

        return cellPos;
    }






    protected virtual void UpdateAnimation()
    {

        if (_animator == null || _sprite == null)
            return;

        if (IsDead == true)
            return;


        if (State == CreatureState.Idle )
        {
            switch (Dir)
            {


                case MoveDir.Up:
                    _animator.Play("Idle_Up");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:

                    _animator.Play("Idle_Down");
                    _sprite.flipX = false;
                    break;


                case MoveDir.Left:

                    _animator.Play("Idle_Left");
                    _sprite.flipX = false;
                    break;


                case MoveDir.Right:

                    _animator.Play("Idle_Right");
                    _sprite.flipX = false;
                    break;
            }

        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("Walk_Up");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("Walk_Down");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("Walk_Left");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("Walk_Right");
                    _sprite.flipX = false;
                    break;

            }

        }
        else if (State == CreatureState.Skill)
        {

            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("Attack_Up");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("Attack_Down");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("Attack_Left");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("Attack_Right");
                    _sprite.flipX = false;
                    break;

            }
        }
        else if(State == CreatureState.Dead)
        {
            //어차피 죽으면 바로 사라짐.
            _animator.Play("Die");
            //_sprite.flipX = false;
        }
        else if(State == CreatureState.Hit)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("Hit_Up");
                    break;
                case MoveDir.Down:
                    _animator.Play("Hit_Down");
                    break;
                case MoveDir.Left:
                    _animator.Play("Hit_Left");
                    break;
                case MoveDir.Right:
                    _animator.Play("Hit_Right");
                    break;
            }
        }
        else
        {

        }
    }


    // Start is called before the first frame update
    void Start()
    {

        Init();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();

 
    }

    public virtual void Init()
    {
        //_speed = 90;

        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();


        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);
        transform.position = pos;


        // 초기 강제코드
        //State = CreatureState.Idle;
        //Dir = MoveDir.Down;
        // CellPos = new Vector3Int(0, 0, 0);

        // 초기에 레이어 설정
        _sprite.sortingOrder = -(int)PosInfo.PosY;

        UpdateAnimation();

        // 죽음 초기화
        IsDead = false;

    }
    protected virtual void UpdateController()
    {

        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }


    }



    protected virtual void UpdateIdle()
    {



        //// 멈춰있을때 위치가 다르면, 서버의 위치대로 저장해준다. BaseCreature들 모두
        //if (Managers.Map != null)
        //{
        //    Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);

        //    if (transform.position != destPos)
        //    {
        //        transform.position = destPos;
        //    }
        //}



        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);
        Vector3 moveDir = destPos - transform.position;



        // 도착 여부 체크
        float dist = moveDir.magnitude;


        Vector3Int destPosInt = new Vector3Int((int)destPos.x, (int)destPos.y, (int)destPos.z);




        if (dist < Speed * Time.smoothDeltaTime)
        {

            //if (Managers.Map.CanGo(destPosInt))

            //    transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;

            if (Managers.Map.CanGo(destPosInt))
                transform.position = destPosInt;



            if (this.GetType() == typeof(PlayerController))
            {
                State = CreatureState.Idle;

                //if (this.GetComponent<PlayerController>()._skillId != -1)
                //    this.GetComponent<PlayerController>().UseSkill(this.GetComponent<PlayerController>()._skillId);


                if (PosHistory.Count > 0)
                {
                    PosInfo = PosHistory[0];
                }

                if (PosHistory.Count > 0)
                {
                    PosHistory.RemoveAt(0);
                }



                //if (PosHistory.Count > 0)
                //{
                //    PosInfo = PosHistory[0];
                //    PosHistory.RemoveAt(0);
                //}



            }


            if (this.GetType() == typeof(MonsterController))
            {
                State = CreatureState.Idle;


                if (PosHistory.Count > 0)
                {
                    PosInfo = PosHistory[0];
                }

                if (PosHistory.Count > 0)
                {
                    PosHistory.RemoveAt(0);
                }


                //if (this.GetComponent<MonsterController>()._skillId != -1)
                //    this.GetComponent<MonsterController>().UseSkill(this.GetComponent<MonsterController>()._skillId);

            }

            if (this.GetType() == typeof(MyPlayerController))
            {
                //if (this.GetComponent<PlayerController>()._skillId != -1)
                //    this.GetComponent<PlayerController>().UseSkill(this.GetComponent<PlayerController>()._skillId);
            }


            MoveToNextPos();




        }
        else
        {
            //transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;
            //State = CreatureState.Moving;


            Vector3 A = transform.position;

            // 살짝 다 안왔을 때에는 채워주고 난뒤에 이동시킨다.
            // 0.00001 <-- 이부분은 time.deltatime 으로 조정해줘야ㅕ할듯
            if (Mathf.Abs(moveDir.x) > 0.000001f && Mathf.Abs(moveDir.y) > 0.000001f)
            {

                if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
                {
                    transform.position = new Vector3(destPos.x, transform.position.y, transform.position.z);
                }
                else if (Mathf.Abs(moveDir.y) < Mathf.Abs(moveDir.x))
                {
                    transform.position = new Vector3(transform.position.x, destPos.y, transform.position.z);
                }
            }
            else
            {
                transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;

            }

            //State = CreatureState.Moving;


        }




    }


















    // 스르륵 이동하는 것을 처리
    protected virtual void UpdateMoving()
    {



        //// 이동이 아니면 리턴한다
        //if (State != CreatureState.Moving)
        //    return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);
        Vector3 moveDir = destPos - transform.position;



        // 도착 여부 체크
        float dist = moveDir.magnitude;


        Vector3Int destPosInt = new Vector3Int((int)destPos.x, (int)destPos.y, (int)destPos.z);


 



        if (dist < Speed * Time.smoothDeltaTime )
        {
            //transform.position = destPos;
 
            //if (Managers.Map.CanGo(destPosInt))
            //    transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;


            if (Managers.Map.CanGo(destPosInt))
                transform.position = destPosInt;

            //if (State == CreatureState.Idle || Blocked)
            //    transform.position = destPos;
            //else
            //{
            //    transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;
            //}

            if (this.GetType() == typeof(PlayerController))
            {
                State = CreatureState.Idle;

                //if (this.GetComponent<PlayerController>()._skillId != -1)
                //    this.GetComponent<PlayerController>().UseSkill(this.GetComponent<PlayerController>()._skillId);

                if (PosHistory.Count > 0)
                {
                    PosInfo = PosHistory[0];
                }

                if (PosHistory.Count > 0)
                {
                    PosHistory.RemoveAt(0);
                }


            }




            if (this.GetType() == typeof(MonsterController))
            {
                State = CreatureState.Idle;


                if (PosHistory.Count > 0)
                {
                    PosInfo = PosHistory[0];
                }

                if (PosHistory.Count > 0)
                {
                    PosHistory.RemoveAt(0);
                }


                //if (this.GetComponent<MonsterController>()._skillId != -1)
                //    this.GetComponent<MonsterController>().UseSkill(this.GetComponent<MonsterController>()._skillId);

            }




            if (this.GetType() == typeof(MyPlayerController))
            {
                //if (this.GetComponent<PlayerController>()._skillId != -1)
                //    this.GetComponent<PlayerController>().UseSkill(this.GetComponent<PlayerController>()._skillId);
            }


            MoveToNextPos();



        }
        else
        {
            //transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;
            //State = CreatureState.Moving;







            Vector3 A = transform.position;

            // 살짝 다 안왔을 때에는 채워주고 난뒤에 이동시킨다.
            // 0.00001 <-- 이부분은 time.deltatime 으로 조정해줘야ㅕ할듯
            if (Mathf.Abs(moveDir.x) > 0.000001f && Mathf.Abs(moveDir.y) > 0.000001f)
            {

                if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
                {
                    transform.position = new Vector3(destPos.x, transform.position.y, transform.position.z);
                }
                else if (Mathf.Abs(moveDir.y) < Mathf.Abs(moveDir.x))
                {
                    transform.position = new Vector3(transform.position.x, destPos.y, transform.position.z);
                }
            }
            else
            {
                transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;

            }




            //transform.position += moveDir.normalized * 8;



            //Debug.Log("(" + (A.x - transform.position.x) + "," + (A.y - transform.position.y) + ")");

            //State = CreatureState.Moving;


        }






    }


    protected virtual void MoveToNextPos()
    {
 

    }

    protected virtual void UpdateSkill()
    {


        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);
        Vector3 moveDir = destPos - transform.position;



        // 도착 여부 체크
        float dist = moveDir.magnitude;


        Vector3Int destPosInt = new Vector3Int((int)destPos.x, (int)destPos.y, (int)destPos.z);




        if (dist < Speed * Time.smoothDeltaTime)
        {

            if (Managers.Map.CanGo(destPosInt))

                transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;


            if (this.GetType() == typeof(PlayerController))
            {
                // State = CreatureState.Idle;

                //if (this.GetComponent<PlayerController>()._skillId != -1)
                //{
                //    //this.GetComponent<PlayerController>().UseSkill(this.GetComponent<PlayerController>()._skillId);
                //}
                //else
                //{
                //    if (PosHistory.Count > 0)
                //    {
                //        PosInfo = PosHistory[0];
                //        PosHistory.RemoveAt(0);
                //    }
                //}


                if (PosHistory.Count > 0)
                {
                    // 위치만 이동시킨다.
                    PosInfo.PosX = PosHistory[0].PosX;
                    PosInfo.PosY = PosHistory[0].PosY;
                }

                if (PosHistory.Count > 0)
                {
                    PosHistory.RemoveAt(0);
                }
            }

        }
        else
        {

                transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;


        }


    }


    protected virtual void UpdateDead()
    {

        IsDead = true;
        State = CreatureState.Dead;
        Managers.Object.Remove(Id, 8.0f);

        this.transform.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.3f * Time.smoothDeltaTime);


        // 모든 레이어의 투명도를 자기랑 똑같이
        foreach (Transform child in transform)
        {
            // SpriteRenderer가 없는 Canvas 등은 넘어감
            if (child.GetComponent<SpriteRenderer>() == null)
            {

            }
            else
            {

                // 이펙트는 제외
                if (child.tag == "HitEffect")
                    return;

                child.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.3f * Time.smoothDeltaTime);
            }
        }

        // 다사라지면 클라쪽에서 초기화해준다. (서버는 상관없음) 
        if (this.transform.GetComponent<SpriteRenderer>().color.a <= 0.0f)
        {
            State = CreatureState.Idle;
            IsDead = false;
        }


        
    }


    public void SetSprite(Sprite a)
    {

        if(_sprite ==null)
            _sprite = GetComponent<SpriteRenderer>();

        _sprite.sprite = a;
    }

}
