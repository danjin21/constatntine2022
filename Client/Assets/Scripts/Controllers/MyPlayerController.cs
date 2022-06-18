using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using WindowsInput;

public class MyPlayerController : PlayerController
{

    bool _actionKeyPressed = false;
    public bool _moveKeyPressed = false;
    public float count = 0;

    public int WeaponDamage { get; private set; }
    public int ArmorDefence { get;  private set; }

    public int itemStr { get; private set; }
    public int itemDex { get; private set; }
    public int itemInt { get; private set; }
    public int itemLuk { get; private set; }
    public int itemHp { get; private set; }
    public int itemMp { get; private set; }
    public int itemWAtk { get; private set; }
    public int itemMAtk { get; private set; }
    public int itemWDef { get; private set; }
    public int itemMDef { get; private set; }
    public int itemSpeed { get; private set; }
    public int itemAtkSpeed { get; private set; }
    public int itemWPnt { get; private set; }
    public int itemMPnt { get; private set; }


    public int TotalStr { get { return Stat.Str + itemStr; } }
    public int TotalDex { get { return Stat.Dex + itemDex; } }
    public int TotalInt { get { return Stat.Int + itemInt; } }
    public int TotalLuk { get { return Stat.Luk + itemLuk; } }
    public int TotalHp { get { return Stat.MaxHp + itemHp; } }
    public int TotalMp { get { return Stat.MaxMp + itemMp; } }
    //public int TotalWAtk { get { return Stat.WAtk + itemStr; } }
    //public int TotalMAtk { get { return Stat.Str + itemStr; } }
    //public int TotalWDef { get { return Stat.Str + itemStr; } }
    //public int TotalMDef { get { return Stat.Str + itemStr; } }
    public float TotalSpeed { get { return Stat.Speed + itemSpeed; } }
    //public int TotalAtkSpeed { get { return Stat.AtkSp + itemJump; } }
    //public int TotalWPnt { get { return Stat.Str + itemStr; } }
    //public int TotalMPnt { get { return Stat.Str + itemStr; } }

    public PositionInfo TempPosInfo = new PositionInfo();

    public int Key = -1;
    public int ConsumeKey = -1;

    //public Vector2 center;
    //public Vector2 size = new Vector2(32 * 30, 32 * 20);
    //public float height;
    //public float width;

    public bool MoveReset;
    public float MoveResetCount;

    protected override void Init()
    {
        base.Init();
        Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -100);

        // 카메라 높이 및 너비 구하기
        //height =  Camera.main.orthographicSize;
        //width = height * Screen.width / Screen.height;


        // 시작할때 한번 스텟 리프레쉬
        RefreshAdditionalStat();

        // 초기에만 TempPosInfo에 서버 좌표 저장.
        TempPosInfo = PosInfo;

        // 초기 포커스는 되니까
        key_window_active = true;
    }

    bool key_window_active;
    void OnApplicationFocus(bool b) // 게임창이 활성화 되면 true 비활성화 false
    {
        key_window_active = b;
    }

    // 카메라부분

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(center, size);
    //}

    private void LateUpdate()
    {

        //float A = 32 * 4;
        //float B = 32 * 3;
        //int xRange = 6;
        //int yRange = 6;

        //if (Camera.main.transform.position.x - transform.position.x - A > xRange * 32)
        //{
        //    Camera.main.transform.position = new Vector3(transform.position.x + xRange * 32 + A, Camera.main.transform.position.y, -10);

        //}
        //else if (Camera.main.transform.position.x - transform.position.x - A < -xRange * 32)
        //{
        //    Camera.main.transform.position = new Vector3(transform.position.x - xRange * 32 + A, Camera.main.transform.position.y, -10);
        //}

        Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -100);



        //if (Camera.main.transform.position.y - transform.position.y + B > yRange * 32)
        //{
        //    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y + yRange * 32 - B, -10);

        //}
        //else if (Camera.main.transform.position.y - transform.position.y + B < -yRange * 32)
        //{
        //    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y - yRange * 32 - B, -10);
        //}

        // 카메라 범위 구하기
        //float lx = size.x  - width;
        //float clampX = Mathf.Clamp(Camera.main.transform.position.x, -lx + center.x, lx + center.x);

        //float ly = size.y - height;
        //float clampY = Mathf.Clamp(Camera.main.transform.position.y, -ly + center.y, ly + center.y);

        //Camera.main.transform.position = new Vector3(clampX, clampY, -10f);
    }


    // HpBar 업데이트
    public override void UpdateHpBar()
    {
        base.UpdateHpBar();

        //UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        //gameSceneUI.Hp.text = Hp.Tostring() + "/" + Stat.MaxHp.Tostring();

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Stat statUI = gameSceneUI.StatUI;
        statUI.RefreshUI_HpMp();

    }

    public void UpdatePositionUI(int x, int y)
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Stat statUI = gameSceneUI.StatUI;
        statUI.RefreshUI_Position(x,y);
    }



    protected override void UpdateController()
    {
        //if (!Managers.Chat.ChatInput.isFocused)
        //{
        //GetUIKeyInput();

        if(MoveReset==true)
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                MoveResetCount += Time.smoothDeltaTime;

                if (MoveResetCount >= /*0.25f*/ 0.1f)
                {
                    MoveReset = false;
                    MoveResetCount = 0;
                }
            }
        }


        UpdateGetInput();

            switch (State)
            {
                case CreatureState.Idle:
                    if(MoveReset == false)
                        GetDirInput();
                    break;
                case CreatureState.Moving:
                    if (MoveReset == false)
                        GetDirInput();
                    break;

            }

            base.UpdateController();
        //}



    }


    //void LateUpdate()
    //{
    //    Camera.main.transform.position = new Vector3(transform.position.x + 128, transform.position.y - 96, -10);
    //    //Camera.main.transform.position = new Vector3(transform.position.x , transform.position.y , -10);

    //}



    protected override void UpdateIdle()
    {


        //base.UpdateIdle();




        // 위치와 바라보는 곳과 상태를 서버와 동기화 시킨다. 빠르게.

        //// 멈춰있을떄 서버와의 위치 동기화
        //PosInfo = TempPosInfo;
        //SyncPos(); //부드럽게 이동하는것 방지

        //posX = CellPos.x;
        //posY = CellPos.y;

        //// 임시로 저장한 서버의 위치를 MyPlayer에도 적용.
        //if (Managers.Map != null)
        //{
        //    Vector3Int A;
        //    A = new Vector3Int(TempPosInfo.PosX, TempPosInfo.PosY, 0);

        //    Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(A) + new Vector3(16.0f, 36.0f, 0);

        //    if (transform.position != destPos)
        //    {
        //        transform.position = destPos;
        //    }
        //}


        if (Managers.Chat.ChatInput.isFocused)
            return;

        // 이동 상태로 갈지 확인
        //if (_moveKeyPressed && Input.GetKey(KeyCode.Space) == false && Input.GetKey(KeyCode.LeftControl) == false
        //    && Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.Z) == false)
        if (_moveKeyPressed && _actionKeyPressed == false)
        {

            State = CreatureState.Moving;
            return;
        }


        Vector3Int A;
        A = new Vector3Int(TempPosInfo.PosX, TempPosInfo.PosY, 0);
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(A) + new Vector3(16.0f, 36.0f, 0);

        Vector3Int destPosInt = new Vector3Int((int)destPos.x, (int)destPos.y, (int)destPos.z);



        // -----------------------------------------------------------------//
        Vector3 moveDir = destPos - transform.position;
        // 도착 여부 체크
        float dist = moveDir.magnitude;

        if (dist < Speed * Time.smoothDeltaTime)
        {

            if (Managers.Map.CanGo(destPosInt))
                transform.position = destPosInt;
            //transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;

        }
        else
        {

            transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime;


        }

        // -----------------------------------------------------------------//


        //transform.position = destPos;


        // -----------------------------------------------------------------//


        // 자기 위치로 안바꿔줬따보니 계속 같은 좌표여서 이동,방향등이 매치가 안되었던 것이다.
        PosInfo.PosX = TempPosInfo.PosX;
        PosInfo.PosY = TempPosInfo.PosY;
        //CellPos = destPosInt;


    }
    [SerializeField]
    public Coroutine _coSkillCooltime;
    [SerializeField]
    public Coroutine _coConsumeCooltime;
    [SerializeField]
    public Coroutine _coShortKeyCooltime;

    IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }


    IEnumerator CoInputCooltime_Comsume(float time)
    {
        yield return new WaitForSeconds(time);
        _coConsumeCooltime = null;
    }

    IEnumerator CoInputCooltime_ShortKey(float time)
    {
        yield return new WaitForSeconds(time);
        _coShortKeyCooltime = null;
    }



    void UpdateGetInput()
    {

        if (Managers.Chat.ChatInput.isFocused)
            return;


        Key = -1;
        ConsumeKey = -1;

        if (key_window_active == true)
        {
            // Else 말고 If 로 해야 순서 중복 되는것까지 확인을 한다.
            if ((WinInput.GetKey(KeyCode.LeftShift)) || (WinInput.GetKey(KeyCode.RightShift)))
            { Key = 1; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.LeftControl)) || (WinInput.GetKey(KeyCode.RightControl)))
            { Key = 2; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.LeftAlt)) || (WinInput.GetKey(KeyCode.RightAlt)))
            { Key = 3; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.Q)))
            { Key = 4; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.W)))
            { Key = 5; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.E)))
            { Key = 6; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.A)))
            { Key = 7; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.S)))
            { Key = 8; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.D)))
            { Key = 9; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.Space)))
            { Key = 10; ConsumeKey = IsConsumeFromKey(Key); }
            if ((WinInput.GetKey(KeyCode.Z)))
            { Key = 11; ConsumeKey = IsConsumeFromKey(Key); }
        }

        //// Else 말고 If 로 해야 순서 중복 되는것까지 확인을 한다.
        //if ((Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightShift)))
        //{ Key = 1; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.LeftControl)) || (Input.GetKey(KeyCode.RightControl)))
        //{ Key = 2; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.LeftAlt)) || (Input.GetKey(KeyCode.RightAlt)))
        //{ Key = 3; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.Q)))
        //{ Key = 4; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.W)))
        //{ Key = 5; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.E)))
        //{ Key = 6; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.A)))
        //{ Key = 7; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.S)))
        //{ Key = 8; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.D)))
        //{ Key = 9; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.Space)))
        //{ Key = 10; ConsumeKey = IsConsumeFromKey(Key); }
        //if ((Input.GetKey(KeyCode.Z)))
        //{ Key = 11; ConsumeKey = IsConsumeFromKey(Key); }

        // 중간에 포션 값이 나왔다면
        if (ConsumeKey != -1)
            Key = ConsumeKey;

        Key key = Managers.KeySetting.Get_KeyValue(Key);

        if ( Key == -1 || key == null)
        {
            if (_actionKeyPressed != false)
            {
                _actionKeyPressed = false;
                // 아무것도 입력 안했으면 리턴
                return;
            }
        }
        else
        {


            // 90000 : 물약이면 그냥 이동되게 만든다. => 이부분은 약간 서버가 봐야될것 같긴함
            if (_actionKeyPressed != true && key.Action != 90000 && key.Action != 90001 && key.Action != 3101000)
            {
                    _actionKeyPressed = true;
            }

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(key.Action, out itemData);

            // 만약에 포션류라면 그냥 액션키 누른게 아니라고해서 쭉 걸으면서 이동되게한다.
            if (itemData != null && itemData.itemType == ItemType.Consumable)
                _actionKeyPressed = false;


            if (_coSkillCooltime != null)
                _actionKeyPressed = false;

            // 스킬이랑 포션은 구분짓는다.- 1
            if (_coSkillCooltime != null)
                return;
 
            // 스킬이랑 포션은 구분짓는다.- 2
            if (_coConsumeCooltime != null)
                return;
  
            // Shrot 키 쿨타임
            if (_coShortKeyCooltime != null)
                return;


            // 액션이 스킬인지 확인
            Skills PlayerSkill = Managers.Skill.Find(i => i.SkillId == key.Action);
            if (PlayerSkill != null)
            {
                // 클라 자체에서도 마나 없는지 확인 (서버는 이미 되어있음)
                if (Stat.Mp < PlayerSkill.Mp)
                {
                    Managers.Chat.ChatRPC("<color=#F78181>스킬에 필요한 마나가 부족합니다.</color>");
                    SkillCool();
                    return;
                }

                // 텔레포트나 물약 아니면 멈추게 만든다 -> 이건 서버쪽에서도 관리해야할듯?
                if (key.Action != 3101000)
                {
                    MoveReset = true;
                    _moveKeyPressed = false;
                }

                // 텔레포트인데, 방향키를 누르고 있지 않으면 return 한다.
                if (key.Action == 3101000)
                {

                    if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
                    {
                        //Managers.Chat.ChatRPC("<color=#99CCFF>텔레포트는 방향키를 누른 상태에서 발동 됩니다.</color>");
                        SkillCool();

                        return;



                    }
                }

            }





            C_ShortKey shortKey = new C_ShortKey()
            {
                Action = key.Action
            };
            Managers.Network.Send(shortKey);
            ShortKeyCool();
            Debug.Log("누르고 있슴다.");

            //UseSkill(key.Action);

            // 스킬 쿨 딜레이는 스킬 패킷 받은다음에 준다,
            // 밑에 UseSkill 에서..

            // 키에 대한 딜레이 -> 패킷이 0.2초마다 갈것이다. -> UseSkill() -> CoStartPunch()
            //_coSkillCooltime = StartCoroutine("CoInputCooltime", 0.8f); // 0.2f


        }

    }


    public int IsConsumeFromKey(int KeyCode)
    {

        Key key = Managers.KeySetting.Get_KeyValue(KeyCode);

        if (key != null && key.Action == 90000)
            return KeyCode;
        
        // 이미 앞에서 ConsumeKey 갱신이 된 상태라면 -1를 반환하지 않는다. (덮음 방지)
        if (ConsumeKey != -1)
            return ConsumeKey;

        return -1;
    }


    // 어차피 서버에서는 800임
    public void SkillCool()
    {

        _coSkillCooltime = StartCoroutine("CoInputCooltime", 0.6f); // 서버에서는 0.8초로 쿨을 준다.
        // 원래 0.8초로 하는게 좋은데, 비교를 하기 위해  0.1f 로 일단 해놓음
    }

    // 어차피 서버에서는 500임
    public void ConsumeCool()
    {

        _coConsumeCooltime = StartCoroutine("CoInputCooltime_Comsume", 0.2f);// 서버에서는 0.5초로 쿨을 준다.
        // 원래 0.8초로 하는게 좋은데, 비교를 하기 위해  0.1f 로 일단 해놓음
    }

    // 이건 자체적으로 실행 => 어차피 서버에서는 700임
    public void ShortKeyCool()
    {
 

            _coShortKeyCooltime = StartCoroutine("CoInputCooltime_ShortKey", 0.05f);

    }




    // 키보드 입력
    void GetDirInput()
    {



        // 채팅창 켜져있으면 방향도 비활성화
        if (Managers.Chat.ChatInput.isFocused)
            return;


        // 단축키 누른 직후 방향키 전환하면 안되게

        if (_coShortKeyCooltime != null)
            return;


        //_moveKeyPressed = true;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            // 이전에 이어 받은거면 count 를 0으로 만들고 이어나간다.
            if (Dir != MoveDir.Up)
                count = 0;

            Dir = MoveDir.Up;
            count += Time.smoothDeltaTime;



        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // 이전에 이어 받은거면 count 를 0으로 만들고 이어나간다.
            if (Dir != MoveDir.Down)
                count = 0;

            Dir = MoveDir.Down;
            count += Time.smoothDeltaTime;

        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            // 이전에 이어 받은거면 count 를 0으로 만들고 이어나간다.
            if (Dir != MoveDir.Left)
                count = 0;

            Dir = MoveDir.Left;
            count += Time.smoothDeltaTime;

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // 이전에 이어 받은거면 count 를 0으로 만들고 이어나간다.
            if (Dir != MoveDir.Right)
                count = 0;

            Dir = MoveDir.Right;
            count += Time.smoothDeltaTime;



        }
        else
        {
            _moveKeyPressed = false;
            //Dir = MoveDir.None;
            count = 0.22f; // 같은 방향이면 좀 빠르게 가게 0으로 안 만든다.

            // 텔레포트 딱 뗼때 키 안먹혀서 (moving)으로 되어있어서. 그래서 떌때 Idle로 바꾸게함

        }




        // 딜레이
        if (count >= 0.27f)
        {
            _moveKeyPressed = true;
            count = 0;
        }
        else
        {

            if (State == CreatureState.Moving)
                return;


            UpdateAnimation();
            CheckUpdatedFlag();

            //// 키 계속 누르고 있을때도 서버와의 위치 동기화
            //PosInfo = TempPosInfo;
            //SyncPos(); //부드럽게 이동하는것 방지
        }

        // 시간을 둔뒤 계속 누르고 있으면 _moveKeyPressed = true;

    }




    protected override void MoveToNextPos()
    {
        // 이동 취약점
        //// 이동하기전에 서버랑 동기화
        //PosInfo = TempPosInfo;




        //if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Z))
        //{
        //    State = CreatureState.Idle;
        //    CheckUpdatedFlag();
        //    return;
        //}

        // 액션키가 활성화 되면 멈춘다.
        if (_actionKeyPressed == true)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }


        if (_moveKeyPressed == false && State != CreatureState.Idle)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();

            ////멈춰있을떄 서버와의 위치 동기화
            ////PosInfo = TempPosInfo;

            //PosInfo.PosX = TempPosInfo.PosX;
            //PosInfo.PosY = TempPosInfo.PosY;

            //SyncPos(); //부드럽게 이동하는것 방지
            return;
        }

        //// 서버에서 받은 위치와 내 현재 위치가 다르면 리턴한다.
        //if (TempPosInfo.PosX != CellPos.x || TempPosInfo.PosY != CellPos.y)
        //{

        //    ////멈춰있을떄 서버와의 위치 동기화
        //    ////PosInfo = TempPosInfo;

        //    //PosInfo.PosX = TempPosInfo.PosX;
        //    //PosInfo.PosY = TempPosInfo.PosY;

        //    // 자기 위치로 안바꿔줬따보니 계속 같은 좌표여서 이동,방향등이 매치가 안되었던 것이다.
        //    PosInfo.PosX = TempPosInfo.PosX;
        //    PosInfo.PosY = TempPosInfo.PosY;

        //    SyncPos();
        //    //CellPos = destPosInt;

        //    //SyncPos(); //부드럽게 이동하는것 방지
        //    return;
        //}



        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;

                break;

            case MoveDir.Down:
                destPos += Vector3Int.down;


                break;

            case MoveDir.Left:
                destPos += Vector3Int.left;

                break;

            case MoveDir.Right:
                destPos += Vector3Int.right;

                break;
        }

        // 진형 추가
        UpdateAnimation();
        //State = CreatureState.Moving;

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.FindCreature(destPos) == null)
            {
                CellPos = destPos;
                Blocked = false;
            }
            else
            {
                Blocked = true;
            }


        }
        else
        {
            Blocked = true;
        }



        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {


        if (_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }




    public void RefreshAdditionalStat()
    {
        WeaponDamage = 0;
        ArmorDefence = 0;

        itemStr = 0;
        itemDex = 0;
        itemInt = 0;
        itemLuk = 0;
        itemHp = 0;
        itemMp = 0;
        itemWAtk = 0;
        itemMAtk = 0;
        itemWDef = 0;
        itemMDef = 0;
        itemSpeed = 0;
        itemAtkSpeed = 0;
        itemWPnt = 0;
        itemMPnt = 0;


        // Managers.Inven에서 아이템 값들을 갖고온다.
        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    //WeaponDamage += ((Weapon)item).Damage;
                    break;
                case ItemType.Armor:
                    //ArmorDefence += ((Armor)item).Defence;
                    break;

            }


            itemStr += item.Str;
            itemDex += item.Dex;
            itemInt += item.Int;
            itemLuk += item.Luk;
            itemHp += item.Hp;
            itemMp += item.Mp;
            itemWAtk += item.WAtk;
            itemMAtk += item.MAtk;
            itemWDef += item.WDef;
            itemMDef += item.MDef;
            itemSpeed += item.Speed;
            itemAtkSpeed += item.AtkSpeed;
            itemWPnt += item.WPnt;
            itemMPnt += item.MPnt;
        }
    }

    public override void OnDead(int damage)
    {
        base.OnDead(damage);

        // 화면이 확 어두워졌다가 점차 밝아지는 것으로 
    }

}
