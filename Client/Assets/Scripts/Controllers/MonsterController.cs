using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;


public class MonsterController : CreatureController
{

    Coroutine _coSkill;
    public string HitSoundPath;
    string DeadSoundPath;
    //Coroutine _coPatrol;
    //Coroutine _coSearch;

    //[SerializeField]
    //Vector3Int _destCellPos;

    //[SerializeField]
    //GameObject _target;

    //[SerializeField]
    //float _searchRange = 10.0f;

    //[SerializeField]
    //float _skillRange = 1.0f;

    //[SerializeField]
    //bool _rangedSkill = false;

    //public override CreatureState State
    //{
    //    get { return PosInfo.State ; }
    //    set
    //    {
    //        if (PosInfo.State == value)
    //            return;

    //        // 우아한 갱싱방법 -> State 가 변하면 애니메이션이 바뀌가 하는걸 
    //        base.State = value;
    //        //if (_coPatrol != null)
    //        //{
    //        //    StopCoroutine(_coPatrol);
    //        //    _coPatrol = null;
    //        //}

    //        //if (_coSearch != null)
    //        //{
    //        //    StopCoroutine(_coSearch);
    //        //    _coSearch = null;
    //        //}

    //    }
    //}
    public int _skillId = -1;

    public override void Init()
    {

        base.Init();

        // 초기 강제코드
        //State = CreatureState.Idle;
        //Dir = MoveDir.Down;

        //_speed = 60;
        //_rangedSkill = true;  // (Random.Range(0, 2) == 0 ? true : false);

        // 장거리 공격
        //_rangedSkill = (Random.Range(0, 2) == 0 ? true : false);


        Data.MonsterData monsterData = null;
        Managers.Data.MonsterDict.TryGetValue(Stat.TemplateId, out monsterData);

        if (monsterData == null)
            return;

        HitSoundPath = monsterData.HitSoundPath;

        //if (_rangedSkill)
        //    _skillRange = 10.0f;
        //else
        //    _skillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        //if(_coPatrol == null)
        //{
        //    _coPatrol = StartCoroutine("CoPatrol");
        //}
        //if (_coSearch == null)
        //{
        //    _coSearch = StartCoroutine("CoSearch");
        //}



    }




    protected override void UpdateController()
    {
        base.UpdateController();

        // 진형 추가 // Monster일 경우에만 애니메이션은 바로 업데이트해준다.
        if (this.GetType() == typeof(MonsterController))
            UpdateAnimation();
    }



    //protected override void MoveToNextPos()
    //{
    //    Vector3Int destPos = _destCellPos;

    //    if(_target !=null)
    //    {

    //        destPos = _target.GetComponent<CreatureController>().CellPos;

    //        Vector3Int dir = destPos - CellPos;

    //        if(dir.magnitude <= _skillRange && (dir.x ==0 || dir.y ==0))
    //        {


    //            Dir = GetDirFromVec(dir);

    //            State = CreatureState.Skill;

    //            if (_rangedSkill)
    //                _coSkill = StartCoroutine("CoStartShootArrow");
    //            else
    //                _coSkill = StartCoroutine("CoStartPunch");


    //            return;
    //        }
    //    }

    //    List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision:true); // 목표에 오브젝트가 있더라도 그곳으로 가게 한다.
    //    if(path.Count <2 || (_target !=null && path.Count > 20))  //길을 못찾았거나, 타겟이 있고 상대방이 너무 멀때
    //    {
    //        _target = null;
    //        State = CreatureState.Idle;
    //        return;
    //    }

    //    Vector3Int nextPos = path[1];  // 0은 나 , 1은 다음위치

    //    Vector3Int moveCellDir = nextPos - CellPos;
    //    //TODO : Astar

    //    Dir = GetDirFromVec(moveCellDir);



    //    //State = CreatureState.Moving;

    //    if (Managers.Map.CanGo(nextPos) && Managers.Object.FindCreature(nextPos) == null)
    //    {
    //            CellPos = nextPos;
    //    }
    //    else
    //    {
    //        State = CreatureState.Idle;
    //    }

    //}


    public override void OnDamaged(int damage, int skillId, List<int> DamageList, int attackerId)
    {
        // Managers.Object.Remove(Id); // 몬스터와의 충돌을 이제 없애기 위해.
        // Managers.Resource.Destroy(gameObject); // 몬스터 그 자체를 없애기 위해.

        base.OnDamaged(damage,skillId, DamageList, attackerId);

        // Managers.Sound.Play(HitSoundPath, Define.Sound.Effect);
    }


    //IEnumerator CoPatrol()
    //{
    //    int waitSeconds = Random.Range(1, 4);
    //    yield return new WaitForSeconds(waitSeconds);

    //    for(int i = 0; i< 10; i++)
    //    {
    //        int xRange = Random.Range(-5, 6);
    //        int yRange = Random.Range(-5, 6);
    //        Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

    //        if(Managers.Map.CanGo(randPos) && Managers.Object.FindCreature(randPos) == null)
    //        {
    //            _destCellPos = randPos;
    //            State = CreatureState.Moving;
    //            yield break;
    //        }

    //    }

    //    State = CreatureState.Idle;


    //}

    //IEnumerator CoSearch()
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(1);

    //        if (_target != null)
    //            continue;

    //        _target = Managers.Object.Find((go) =>
    //        {
    //            PlayerController pc = go.GetComponent<PlayerController>();

    //            if (pc == null) // 플레이어가 아니면 안넣는다.
    //                return false;

    //            Vector3Int dir = (pc.CellPos - CellPos);
    //            if (dir.magnitude > _searchRange) // 멀면 안넣는다.
    //                return false;

    //            return true;
    //        });


    //    }
    //}

       







    //IEnumerator CoStartPunch()
    //{

    //    // 피격 판정
    //    GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
    //    if (go != null)
    //    {
    //        CreatureController cc = go.GetComponent<CreatureController>();
    //        if (cc != null)
    //        {
    //            cc.OnDamaged();
    //        }
    //    }

    //    // 대기 시간
    //    yield return new WaitForSeconds(0.5f);
    //    State = CreatureState.Moving;
    //    _coSkill = null;
    //}

    //IEnumerator CoStartShootArrow()
    //{
    //    GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
    //    ArrowController ac = go.GetComponent<ArrowController>();

    //    ac.Dir = Dir;
    //    ac.CellPos = CellPos;

    //    // 대기 시간
    //    yield return new WaitForSeconds(0.3f);
    //    State = CreatureState.Moving;
    //    _coSkill = null;
    //}

    public void OnIdle()
    {
        State = CreatureState.Idle;

    }

    public override void UseSkill(int skillId)
    {



        if (State == CreatureState.Moving)
        {
            //if(this.GetType() == typeof(PlayerController))
            return;
        }

        // 먼저 몬스터 방향 갱신
        UpdateAnimation();

        // 스킬아이디 갱신
        _skillId = skillId;


        if (skillId == 9001000)
        {
            //_coSkill = StartCoroutine("CoStartPunch");
            State = CreatureState.Skill;

        }
        else if (skillId == 2001000)
        {
            //_coSkill = StartCoroutine("CoStartShootArrow");
            State = CreatureState.Skill;

        }

        _skillId = -1;
    }

    public override void OnDead(int damage)
    {
        base.OnDead(damage);

        //GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        //effect.transform.position = transform.position + new Vector3(0, 0, -1); ;
        //effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        //effect.GetComponent<Animator>().Play("START");

        //// 게임 이펙트를 몇초 후에 삭제
        //GameObject.Destroy(effect, 0.5f);

    }


}
