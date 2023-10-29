using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using WindowsInput;
using System;
using UnityEngine.Tilemaps;

public class MyPlayerController : PlayerController
{

    bool _actionKeyPressed = false;
    public bool _moveKeyPressed = false;
    public float count = 0;
    public float count_checkDistance = 0; 

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

    public bool IsSkillSend;

    public int IsTargetChoice;


    protected override void Init()
    {
        base.Init();







        Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);


        _hpBar.transform.gameObject.SetActive(true);

        // 카메라 높이 및 너비 구하기
        //height =  Camera.main.orthographicSize;
        //width = height * Screen.width / Screen.height;


        // 시작할때 한번 스텟 리프레쉬
        RefreshAdditionalStat();

        // 초기에만 TempPosInfo에 서버 좌표 저장.
        TempPosInfo = PosInfo;

        // 초기 포커스는 되니까
        key_window_active = true;

        IsTargetChoice = - 1;




        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;


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


    public bool hold1 = false;
    public bool hold2 = false;
    public bool hold3 = false;
    public bool hold4 = false;

    public Tilemap tmBase;

    int CameraRange = 8;

    public Vector2 cameraCenter;
    public Vector2 size;

    float height;
    float width;


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cameraCenter, size);
    }

    private void LateUpdate()
    {

        Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);




        float lx = size.x * 0.5f - width;
        float clampX = Mathf.Clamp(Camera.main.transform.position.x, -lx + cameraCenter.x, lx + cameraCenter.x);

        float ly = size.y * 0.5f - height;
        float clampY = Mathf.Clamp(Camera.main.transform.position.y, -ly + cameraCenter.y, ly + cameraCenter.y);

      
        Camera.main.transform.position = new Vector3(clampX,clampY, -1000);


        //Vector3 clampedPosition = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);

        //clampedPosition.x = Mathf.Clamp(clampedPosition.x, Managers.Map.MinX+ CameraRange, Managers.Map.MaxX- CameraRange);
        //clampedPosition.y = Mathf.Clamp(clampedPosition.y, Managers.Map.MinY+ CameraRange, Managers.Map.MaxY- CameraRange);

        //Camera.main.transform.position = clampedPosition;

        //if (CellPos.x > Managers.Map.MaxX - CameraRange)
        //{
        //    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y - 96.0f, -1000);
        //}
        //else
        //{
        //    if (Dir == MoveDir.Left)
        //    {
        //        if (Camera.main.transform.position.x > transform.position.x + 128.0f)
        //            Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, Camera.main.transform.position.y, -1000);
        //    }
        //    else
        //    {
        //        Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);
        //    }

        //    //Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);
        //}




        //if (CellPos.x > Managers.Map.MaxX - CameraRange || CellPos.x < Managers.Map.MinX + CameraRange)
        //{
        //    Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);
        //}

        //if (CellPos.y > Managers.Map.MaxY - CameraRange || CellPos.x < Managers.Map.MinY + CameraRange)
        //{
        //    Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);
        //}

        // 현재맵의 




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





        //CameraMove();

        //else
        //{
        //    Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, transform.position.y - 96.0f, -1000);


        //}







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


    public void CameraMove()
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

        // 미니맵 제목 바꾸기
        UI_MiniMap miniMapUI = gameSceneUI.MiniMapUI;

        if (!miniMapUI.holdCamera4 && !miniMapUI.holdCamera2)
        {
            if (Dir == MoveDir.Right)
            {
                if (Camera.main.transform.position.x <= transform.position.x + 128.0f)
                    Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, Camera.main.transform.position.y, -1000);
            }
            else if (Dir == MoveDir.Left)
            {
                if (Camera.main.transform.position.x > transform.position.x + 128.0f)
                    Camera.main.transform.position = new Vector3(transform.position.x + 128.0f, Camera.main.transform.position.y, -1000);
            }
        }


        if (!miniMapUI.holdCamera3 && !miniMapUI.holdCamera1)
        {
            if (Dir == MoveDir.Up)
            {
                if (Camera.main.transform.position.y <= transform.position.y - 96.0f)
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y - 96.0f, -1000);
            }
            else if (Dir == MoveDir.Down)
            {
                if (Camera.main.transform.position.y > transform.position.y - 96.0f)
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y - 96.0f, -1000);
            }
        }

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
        if(IsTargetChoice != -1)
        {
            return;
        }

        base.UpdateController();

        //if (!Managers.Chat.ChatInput.isFocused)
        //{
        //GetUIKeyInput();

        if (MoveReset==true)
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




            switch (State)
            {
                case CreatureState.Idle:
                if (MoveReset == false)
                        GetDirInput();
                    break;
                case CreatureState.Moving:
                if (MoveReset == false)
                {
                    GetDirInput_Moving();
                }
                    break;

        }

  
        //}
        UpdateGetInput();



    }


    //void LateUpdate()
    //{
    //    Camera.main.transform.position = new Vector3(transform.position.x + 128, transform.position.y - 96, -10);
    //    //Camera.main.transform.position = new Vector3(transform.position.x , transform.position.y , -10);

    //}


    //[SerializeField]
    //public Coroutine _coCheckDistance;



    //IEnumerator CoCheckDistance(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    _coSkillCooltime = null;
    //}



    protected override void UpdateIdle()
    {
        if (IsSkillSend == true)
            return;

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




        // 현재 위치에 다른 크리쳐가 있으면 서버 위치랑 동기화 시킨다.
        GameObject Creature = Managers.Map.Find(CellPos);

        // 그냥 클라이언트에서는 막 움직이게

        // 크리쳐가 있다느 뜻임
        if (Creature != null && Creature != this.gameObject)
        {
            // 서버랑 위치 동기화

            // -----------------------------------------------------------------//
            Managers.Map.ApplyMove(gameObject, PosInfo.PosX, PosInfo.PosY, TempPosInfo.PosX, TempPosInfo.PosY);

            // 자기 위치로 안바꿔줬따보니 계속 같은 좌표여서 이동,방향등이 매치가 안되었던 것이다.
            PosInfo.PosX = TempPosInfo.PosX;
            PosInfo.PosY = TempPosInfo.PosY;

            // SyncPos();

        
            State = CreatureState.Idle;
            _updated = true;
            CheckUpdatedFlag();
        }


        // 거리가 2칸이상일 경우에만 서버 대로 움직이게 해준다.

        if (CellPos.x != TempPosInfo.PosX || CellPos.y != TempPosInfo.PosY)
        {
            Debug.Log($"CellPos X = {CellPos.x}/{CellPos.y} / Temp = {TempPosInfo.PosX}/{TempPosInfo.PosY}");

            int difX = Math.Abs(CellPos.x - TempPosInfo.PosX);
            int difY = Math.Abs(CellPos.y - TempPosInfo.PosY);

            Debug.Log($"{difX}/{difY}");

            // 시간 체크를 한다.
            count_checkDistance += Time.smoothDeltaTime;


            if (difX <= 0 && difY <= 0)
                return;

            // 3초 동안 다른 상태면 이동시켜준다. ( 서버가 조금 느리게 답변을 주기 때문 )
            if (count_checkDistance < 3.00f)
                return;



            // 2023.06.14 진형 밖에 있는걸 IF문 안으로 집어넣음.
            // 위 예외처리(return)를 받으려면 안에다 넣어야함
            //====================================================================================================================================================//

            // 일정 시간 지난후에... 핑 검사후 그다음에 돌아가게.. 혹은 2칸 이상일 경우에만..


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
            Managers.Map.ApplyMove(gameObject, PosInfo.PosX, PosInfo.PosY, TempPosInfo.PosX, TempPosInfo.PosY);


            // 자기 위치로 안바꿔줬따보니 계속 같은 좌표여서 이동,방향등이 매치가 안되었던 것이다.
            PosInfo.PosX = TempPosInfo.PosX;
            PosInfo.PosY = TempPosInfo.PosY;

            SyncPos();

            State = CreatureState.Idle;
            CheckUpdatedFlag();

            // Idle로 

            //State = CreatureState.Idle;

            //====================================================================================================================================================//




        }
        else
        {
            count_checkDistance = 0;
        }

    }

    [SerializeField]
    public Coroutine _coSkillCooltime = null;
    [SerializeField]
    public Coroutine _coConsumeCooltime = null;
    [SerializeField]
    public Coroutine _coShortKeyCooltime = null;
    [SerializeField]
    public Coroutine _coShortKeyCooltime_Potion = null;
    [SerializeField]
    public Coroutine _coShortKeyCooltime_Teleport = null;
    [SerializeField]
    public Coroutine _coMessageCooltime = null;
    [SerializeField]
    public Coroutine _coShortKeyCooltime_Soonbo = null;


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
        // 서버에서 받아야지만 되게
        // yield return null;

        // 서버에서 응답 안받을 경우를 대비
        yield return new WaitForSeconds(time);

        _coShortKeyCooltime = null;
    }

    IEnumerator CoMessageCooltime(float time)
    {
        // 서버에서 응답 안받을 경우를 대비
        yield return new WaitForSeconds(time);

        _coMessageCooltime = null;
    }


    public override void UseSkill(int skillId)
    {

        base.UseSkill(skillId);

        if (skillId != 3101000 && skillId != 4001000)
            Managers.Object.MyPlayer.SkillCool();



        // StopCoroutine(_coShortKeyCooltime);
    }


    // 물약은 따로 
    IEnumerator CoInputCooltime_ShortKey_Potion(float time)
    {
        yield return new WaitForSeconds(time);
        _coShortKeyCooltime_Potion = null;
    }

    // 텔레포트 따로
    IEnumerator CoInputCooltime_ShortKey_Teleport(float time)
    {
        yield return new WaitForSeconds(time);
        _coShortKeyCooltime_Teleport = null;
    }

    // 순보 따로
    IEnumerator CoInputCooltime_ShortKey_Soonbo(float time)
    {
        yield return new WaitForSeconds(time);
        _coShortKeyCooltime_Soonbo = null;
    }


    public string A;
    public float B;
    public float C;

    public bool IsEntered = false;

    public int lastKey = -1;

    void UpdateGetInput()
    {

        if (Managers.Chat.ChatInput.isFocused)
            return;


        //// 채팅 엔터 끝나자마자 누르고 있는거 바로 스킬 쳐지지 않게
        //{

        //    bool OneClick = false;

        //    // Else 말고 If 로 해야 순서 중복 되는것까지 확인을 한다.
        //    if ((WinInput.GetKeyDown(KeyCode.LeftShift)) || (WinInput.GetKeyDown(KeyCode.RightShift)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.LeftControl)) || (WinInput.GetKeyDown(KeyCode.RightControl)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.LeftAlt)) || (WinInput.GetKeyDown(KeyCode.RightAlt)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.Q)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.W)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.E)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.A)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.S)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.D)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.Space)))
        //        OneClick = true;
        //    if ((WinInput.GetKeyDown(KeyCode.Z)))
        //        OneClick = true;


        //    if (OneClick == true)
        //        IsEntered = false;

        //}


        if (IsEntered)
            return;

     

        Key = -1;
        ConsumeKey = -1;

        int FinalKey = -1;

        if (key_window_active == true)
        {

            // anykey none
            // Else 말고 If 로 해야 순서 중복 되는것까지 확인을 한다.
            if ((WinInput.GetKey(KeyCode.LeftShift)) || (WinInput.GetKey(KeyCode.RightShift)))
            { Key = 1; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.LeftControl)) || (WinInput.GetKey(KeyCode.RightControl)))
            { Key = 2; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.LeftAlt)) || (WinInput.GetKey(KeyCode.RightAlt)))
            { Key = 3; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.Q)))
            { Key = 4; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.W)))
            { Key = 5; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.E)))
            { Key = 6; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.A)))
            { Key = 7; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.S)))
            { Key = 8; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.D)))
            { Key = 9; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.Space)))
            { Key = 10; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
            if ((WinInput.GetKey(KeyCode.Z)))
            { Key = 11; ConsumeKey = IsConsumeFromKey(Key); if (Key != lastKey) FinalKey = Key; }
        }

        // 혹시 동시에 눌렀는데 마지막에 누른애가 위에 있다면, 위에 있는애가 키가 되게한다.
        if(FinalKey != -1)
        {
            Key = FinalKey;
        }

        lastKey = Key;



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
            if (_actionKeyPressed != true  && key.Action != 3101000)
            {
                //if(key.Action != 90000 && key.Action != 90001)
                // 물약인 경우만
                if (key.Action >= 90000 && key.Action <= 90100)
                {

                }
                else
                    _actionKeyPressed = true;
            }

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(key.Action, out itemData);

            // 만약에 포션류라면 그냥 액션키 누른게 아니라고해서 쭉 걸으면서 이동되게한다.
            if (itemData != null && itemData.itemType == ItemType.Consumable)
                _actionKeyPressed = false;


            //if (_coSkillCooltime != null)
            //    _actionKeyPressed = false;

            //// 스킬이랑 포션은 구분짓는다.- 1
            //if (_coSkillCooltime != null)
            //    return;



            // 스킬이랑 포션은 구분짓는다.- 2
            if (_coConsumeCooltime != null)
                return;
  
            // Shrot 키 쿨타임
            if (_coShortKeyCooltime != null)
                return;

            // Shrot_Potion 키 쿨타임
            if (_coShortKeyCooltime_Potion != null)
                return;

            // 텔레포트 키 쿨타임
            if (_coShortKeyCooltime_Teleport != null)
                return;

            // 순보 키 쿨타임 
            if (_coShortKeyCooltime_Soonbo != null && key.Action == 4001000)
                return;


   
            // 액션이 스킬인지 확인
            Skills PlayerSkill = Managers.Skill.Find(i => i.SkillId == key.Action);
            if (PlayerSkill != null)
            {
                // 클라 자체에서도 마나 없는지 확인 (서버는 이미 되어있음)
                if (Stat.Mp < PlayerSkill.Mp )
                {
                    if (_coMessageCooltime == null)
                    {
                        Managers.Chat.ChatRPC("<color=#F78181>스킬에 필요한 마나가 부족합니다.</color>");
                        MessageCooltime();
                    }

                    SkillCool();

                    return;
                }


                // 텔레포트나 물약 아니면 멈추게 만든다 -> 이건 서버쪽에서도 관리해야할듯?
                if (key.Action != 3101000)
                {

                    //Idle 일때 액션이 되게

                    //if (State != CreatureState.Idle )
                    if (State == CreatureState.Moving)
                        return;

                    MoveReset = true;
                    _moveKeyPressed = false;
                    Debug.Log("!!!!@1 _moveKeyPressed = " + _moveKeyPressed);



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

                if(key.Action == 2001000)
                {
                    Item bowItem = null;
                    bowItem = Managers.Inven.Find(i => i.Equipped && i.ItemType == ItemType.Weapon);

                    if (((Weapon)bowItem).WeaponType != WeaponType.Bow )
                    {
                        if (_coMessageCooltime == null)
                        {
                            Managers.Chat.ChatRPC("<color=#F78181>활을 착용해야 사용할 수 있습니다.</color>");

                            MessageCooltime();
                        }
                        SkillCool();
                        return;
                    }
                }

                if(key.Action == 3101002)
                {

                    if (IsTargetChoice == -1)
                    {
                        Managers.Chat.ChatRPC("<color=#77C2FD>대상을 골라주세요.</color>");
                        IsTargetChoice = key.Action;

                        if (GetComponent<MyPlayerController_SkillTarget>().target == null)
                        {
                            GetComponent<MyPlayerController_SkillTarget>().target = this;
                            GetComponent<MyPlayerController_SkillTarget>().TargetBox();

                        }
                        else  // 타겟 있으면 박스 켜준다.
                            GetComponent<MyPlayerController_SkillTarget>().TargetBox();

                    }
                    

                    return;

                }

            }






            // 물약을 먹고 있으면 이동 안되는것 해결
            if (ConsumeKey == -1)
            {
                if(key.Action == 3101000) // 텔레포트
                {
                    ShortKeyCool_Teleport();
                }
                else if (key.Action == 4001000) // 순보
                {
                    ShortKeyCool_Soonbo();
                }
                else
                {


  
                    // 서버랑 위치가 같아야 쓸 수 있다.
                    if (CellPos.x != TempPosInfo.PosX || CellPos.y != TempPosInfo.PosY)
                    {
                        return;
                    }



                    ShortKeyCool();
                    if (IsSkillSend == false)
                    {
                        IsSkillSend = true;
                        Debug.Log("+++++++++++++++++++++++++++스킬 전송 완료");

                        A = DateTime.Now.ToString("ss.fffffff");
                        float NowTime = float.Parse(A);
                        B = NowTime;


                        //Managers.Chat.ChatRPC($"<color=#000000>쿨타임 시간 : {TempC}</color>");
                        //Managers.Chat.ChatRPC($"<color=#000000>지연율 : {TempC - C}</color>");


                    }
               
                }
            }               
            else
                ShortKeyCool_Potion();

            // 타입이 맞는지 확인

            C_ShortKey shortKey = new C_ShortKey()
            {
                Action = key.Action
            };
            Managers.Network.Send(shortKey);


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

     

        if (key != null)
        {
            // 아이템 값의 코드를 가져온다.
            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(key.Action, out itemData);

            if (itemData != null && itemData.itemType == ItemType.Consumable)
            {
                return KeyCode;
            }           
        }
        
        // 이미 앞에서 ConsumeKey 갱신이 된 상태라면 -1를 반환하지 않는다. (덮음 방지)
        if (ConsumeKey != -1)
            return ConsumeKey;

        return -1;
    }



    // 어차피 서버에서는 800임
    public void SkillCool()
    {
        // 핑 지연율만큼 더해주거나, 빼준다.
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
        //_coShortKeyCooltime = StartCoroutine("CoInputCooltime_ShortKey", 0.05f);

        // 서버에서 반응 안와서 null 하는거 안될까봐 5초 뒤에 null 되게 한다.
        _coShortKeyCooltime = StartCoroutine("CoInputCooltime_ShortKey", 0.35f);


    }

    // 어차피 서버에서는 800임
    public void MessageCooltime()
    {
        // 핑 지연율만큼 더해주거나, 빼준다.
        _coMessageCooltime = StartCoroutine("CoMessageCooltime", 2.0f); // 서버에서는 0.8초로 쿨을 준다.
        // 원래 0.8초로 하는게 좋은데, 비교를 하기 위해  0.1f 로 일단 해놓음
    }

    // 이건 자체적으로 실행 => 어차피 서버에서는 700임
    public void ShortKeyCool_Potion()
    {
        _coShortKeyCooltime_Potion = StartCoroutine("CoInputCooltime_ShortKey_Potion", 0.35f);
    }

    // 텔레포트도 따로 쿨타임
    public void ShortKeyCool_Teleport()
    {
        _coShortKeyCooltime_Teleport = StartCoroutine("CoInputCooltime_ShortKey_Teleport", 0.1f);
    }

    // 순보도 따로 쿨타임
    public void ShortKeyCool_Soonbo()
    {
        _coShortKeyCooltime_Soonbo = StartCoroutine("CoInputCooltime_ShortKey_Soonbo", 0.5f);
    }


    void GetDirInput_Moving()
    {
        if(!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            _moveKeyPressed = false;

            //Dir = MoveDir.None;
            count = 0.22f; // 같은 방향이면 좀 빠르게 가게 0으로 안 만든다.

            // 텔레포트 딱 뗼때 키 안먹혀서 (moving)으로 되어있어서. 그래서 떌때 Idle로 바꾸게함

        }
    }

    void GetDirInput_NextPos()
    {

        if (IsSkillSend == true)
            return;

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



        //else
        //{
        //    _moveKeyPressed = false;

        //    //Dir = MoveDir.None;
        //    count = 0.22f; // 같은 방향이면 좀 빠르게 가게 0으로 안 만든다.

        //    // 텔레포트 딱 뗼때 키 안먹혀서 (moving)으로 되어있어서. 그래서 떌때 Idle로 바꾸게함

        //}

        //// 딜레이
        //if (count >= 0.27f)
        //{
        //    _moveKeyPressed = true;

        //    count = 0;
        //}
    }





    // 키보드 입력
    void GetDirInput()
    {


        // 채팅창 켜져있으면 방향도 비활성화
        if (Managers.Chat.ChatInput.isFocused)
            return;


        if (IsSkillSend == true)
            return;


        // 텔레포트 패킷 보내자마자 방향키 바뀌지 않게 방지 ( 키 누르고 0.35초 간 방향전환 X )
        if (_coShortKeyCooltime_Teleport != null)
            return;



        // 단축키 누른 직후 방향키 전환하면 안되게

        //if (_coShortKeyCooltime != null)
        //{
        //    return;
        //}
        //else
        //{
        //    // 그 서버에서 스킬 쓰라고 왔어도
        //    // 스킬 쓰는동안에도 못가게 하기
        //    if (State == CreatureState.Skill)
        //        return;
        //}


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


        if (IsSkillSend == true)
            return;

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



        // 서버랑 5칸 이상 벌어지는데 간다고 하면, 중지
        // 서버에서도 5칸 이상이면 return 한다.

        int difX = Math.Abs(CellPos.x - TempPosInfo.PosX);
        int difY = Math.Abs(CellPos.y - TempPosInfo.PosY);

        if (difX > 4 || difY > 4)
        {
            Managers.Map.ApplyMove(gameObject, PosInfo.PosX, PosInfo.PosY, TempPosInfo.PosX, TempPosInfo.PosY);
            PosInfo.PosX = TempPosInfo.PosX;
            PosInfo.PosY = TempPosInfo.PosY;
            SyncPos();

            State = CreatureState.Idle;
            CheckUpdatedFlag();

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


        GetDirInput_NextPos();

        Vector3Int destPos = CellPos;
        // Vector3Int destPos = new Vector3Int(TempPosInfo.PosX, TempPosInfo.PosY, CellPos.z);
        // destPos 를 TempPos 로 바꾸면, 서버에 따라 움직이는게 된다.


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

            if(Managers.Map.Find(destPos) == null)
            {

                // 현재 위치에 다른 크리쳐가 있으면 서버 위치랑 동기화 시킨다.
                GameObject Creature = Managers.Map.Find(CellPos);

                // 크리쳐가 있다느 뜻임
                if (Creature != null && Creature != this.gameObject)
                {
                    // 서버랑 위치 동기화

                    // -----------------------------------------------------------------//
                    Managers.Map.ApplyMove(gameObject, PosInfo.PosX, PosInfo.PosY, TempPosInfo.PosX, TempPosInfo.PosY);


                    // 자기 위치로 안바꿔줬따보니 계속 같은 좌표여서 이동,방향등이 매치가 안되었던 것이다.
                    PosInfo.PosX = TempPosInfo.PosX;
                    PosInfo.PosY = TempPosInfo.PosY;

                    SyncPos();
                    Blocked = true;


                }
                else
                {
                    CellPos = destPos;
                    Blocked = false;



                    // 미니맵 교체해주기

                    UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

                    UI_MiniMap miniMapUI = gameSceneUI.MiniMapUI;
                    miniMapUI.DrawCollision_center(Managers.Map.MaxY - PosInfo.PosY - 1, PosInfo.PosX - Managers.Map.MinX);
                }

            }
            else
            {
                Blocked = true;
            }

            //if (Managers.Object.FindCreature(destPos) == null)
            //{
                
            //        CellPos = destPos;
            //        Blocked = false;
            //}
            //else
            //{
            //    Blocked = true;
            //}


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


    protected override void UpdateSkill()
    {
        if (IsSkillSend == true)
            return;

        base.UpdateSkill();


    }

    // UpdateIdle 은 위에 있따.



    protected override void UpdateMoving()
    {
        if (IsSkillSend == true)
            return;

        base.UpdateMoving();


    }

    //protected override void UpdateMoving()
    //{


    //    int difX = Math.Abs(CellPos.x - TempPosInfo.PosX);
    //    int difY = Math.Abs(CellPos.y - TempPosInfo.PosY);

    //    ////멈춰있을떄 서버와의 위치 동기화

    //    if (difX >= 8 || difY >= 8)
    //    {
    //        // 걷고있을땐 동기화 안되게...
    //        if (State == CreatureState.Moving)
    //        {




    //            // 그래서 두칸부터 이동되게 만듬
    //            Managers.Map.ApplyMove(gameObject, PosInfo.PosX, PosInfo.PosY, TempPosInfo.PosX, TempPosInfo.PosY);
    //            PosInfo.PosX = TempPosInfo.PosX;
    //            PosInfo.PosY = TempPosInfo.PosY;
    //        }
    //    }
    //    else
    //    {
    //        base.UpdateMoving();
    //    }

    //}


}
