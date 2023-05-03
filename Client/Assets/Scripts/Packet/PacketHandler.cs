using Data;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class PacketHandler
{
	//public static void S_ChatHandler(PacketSession session, IMessage packet)
	//{
	//	S_Chat chatPacket = packet as S_Chat;
	//	ServerSession serverSession = session as ServerSession;

	//	Debug.Log(chatPacket.Context);
	//}

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
        // ServerSession serverSession = session as ServerSession;

        Managers.Map.LoadMap(enterGamePacket.Player.StatInfo.Map);

        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
   
        Debug.Log("모죠");

        // 채팅 이니시에이팅
        Managers.Chat.Init();


        // 캐릭터가 들어오면 그때 아이템과 스텟을 갱신해준다.

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        invenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.

        // 단축키도 리프뤠시
        UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        KeySettingUI.RefreshUI();



    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
        //ServerSession serverSession = session as ServerSession;

        //Managers.Object.RemoveMyPlayer();
        //Managers.Object.RemoveAll();
        Managers.Object.Clear();

    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {

        // 서버에서 패킷이 두번와서 동시에...
        // 원인은 진짜 못찾고 있음...
        // 그래서.. 우선 화살 예외처리 여기서 부터 땡김
        

        S_Spawn spawnPacket = packet as S_Spawn;
        //ServerSession serverSession = session as ServerSession;

        ObjectInfo A = null;

        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            if (Managers.Object._objects.ContainsKey(obj.ObjectId))
                continue;

            Managers.Object.Add(obj, myPlayer: false);
            A = obj;
            Debug.Log("finish" + A.ObjectId + "/" + Time.deltaTime);

        }




        //Debug.Log(spawnPacket.Players);



    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;



        foreach (int id in despawnPacket.ObjectIds)
        {
            if (Managers.Object.FindById(id) != null && Managers.Object.FindById(id).GetComponent<BaseController>().State == CreatureState.Dead)
            {
                return;
            }

            // 화살은 안지워준다. 클라이언트에서 처리
            //if (Managers.Object.FindById(id) != null && Managers.Object.FindById(id).GetComponent<ArrowController>() != null)
            //    return;

            // 혹시 몰라서 늦게라도 안지워졌을 경우, 서버에 따라 지워지게만든다.
            // 무조건 서버보단 클라가 먼저 도착해서 Remove 하긴한다.

            //if (Managers.Object.FindById(id) != null && Managers.Object.FindById(id).GetComponent<ArrowController>() != null)
            //    return;

            GameObject A = null;
            A = Managers.Object.FindById(id);


 
 

            // 화살일경우에는 자식 화살들도 지워지게 해야한다.

            if (A != null && A.GetComponent<ArrowController>() != null)
            {
                A.GetComponent<ArrowController>().removeChild();

                // 화살일 경우, 클라에서 맞아야 사라지게 몇초 뒤에 사라지게 한다.

                  // Managers.Object.Remove(id, 2.0f);
                return;
            }

            Managers.Object.Remove(id);

        }
        //ServerSession serverSession = session as ServerSession;
    }





    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        //ServerSession serverSession = session as ServerSession;


        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        
        if (go == null)
            return;

        // 나의 플레이어 일 경우, 서버의 패킷을 받지 않는다. 다른 플레이어꺼만 받는다.
        // 엇갈릴 경우를 대비해 임시로 서버의 위치는 저장해놓는다. 추후 MyPlayerController에서 검증
        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
        {
            MyPlayerController myPC = go.GetComponent<MyPlayerController>();
            if (myPC == null)
                return;


            myPC.TempPosInfo = movePacket.PosInfo;

            // 서버에서 받은 실제 플레이어의 위치를 저장해준다.
            myPC.UpdatePositionUI(myPC.TempPosInfo.PosX, myPC.TempPosInfo.PosY);



            // 서버에서 받은 위치와 내 현재 위치가 다르면 리턴한다.

            // ☆★☆★☆★☆★☆★☆★ 일정시간동안 다른지를 확인해야할듯
            if (myPC.TempPosInfo.PosX != myPC.CellPos.x || myPC.TempPosInfo.PosY != myPC.CellPos.y)
            {


                ////멈춰있을떄 서버와의 위치 동기화


                // when : 몬스터랑 절묘하게 맞아 떨어져서 통과할때.

                // 걷고있을땐 동기화 안되게...
                if (myPC.State == CreatureState.Moving)
                {
                    // 난 이동 중인데 서버에서 Idle로 올때가 있다. 스킬 쓰고 왓을 때.
                    // 그래서 그 때는 생략해준다.


                    //if (movePacket.PosInfo.State == CreatureState.Idle)
                    //    return;



                    // 예외 : 스킬 쓰자마자 바로 이동할때(방향이 다른 이동)는 작동 안되게 해야한다.
                    //if (movePacket.PosInfo.MoveDir != myPC.PosInfo.MoveDir)
                    //    return;

                    // 서버랑 2칸 이상 벌어지는데 간다고 하면, 중지
                    // 서버에서도 2칸 이상이면 return 하고, 신호를 준다.
                    
                    // 아니면.. 공격하자마자 이동하는거 자체를 막아야 할지도..

                    //int difX = Math.Abs(myPC.CellPos.x - myPC.TempPosInfo.PosX);
                    //int difY = Math.Abs(myPC.CellPos.y - myPC.TempPosInfo.PosY);

                    //if (difX > 2 || difY > 2)
                    //{
                    //    Managers.Map.ApplyMove(myPC.gameObject, myPC.PosInfo.PosX, myPC.PosInfo.PosY, myPC.TempPosInfo.PosX, myPC.TempPosInfo.PosY);
                    //    myPC.PosInfo.PosX = myPC.TempPosInfo.PosX;
                    //    myPC.PosInfo.PosY = myPC.TempPosInfo.PosY;
                    //    myPC.SyncPos();

                    //    return;
                    //}

                    // 뭔가 한번만 실행되기 때문에..... 계속 되게하는걸 생각해야할듯

                    // 그래서 두칸부터 이동되게 만듬
                    // 여긴 무조건 한칸임

                    //Managers.Map.ApplyMove(myPC.gameObject, myPC.PosInfo.PosX, myPC.PosInfo.PosY, myPC.TempPosInfo.PosX, myPC.TempPosInfo.PosY);
                    //myPC.PosInfo.PosX = myPC.TempPosInfo.PosX;
                    //myPC.PosInfo.PosY = myPC.TempPosInfo.PosY;

                    return;
                }


            }


            // 위치 동기화시켜주자



            return;
        }


        BaseController  bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        // 플레이어나 몬스터일 경우

        if (bc.GetType() == typeof(PlayerController) || bc.GetType() == typeof(MonsterController))
        {
            //movePacket.PosInfo.State = bc.State;
            //movePacket.PosInfo.MoveDir = bc.Dir;



            if (bc.GetType() == typeof(PlayerController))
            {
                // 플레이어인 경우 스킬 씹힐 수도 있어서 그다음부터는 이동이 안되므로, 그냥 그대로 둔다
                if (movePacket.PosInfo.State == CreatureState.Skill)
                {
                    //movePacket.PosInfo.State = bc.State;
         
                }

            }

            bc.PosHistory.Add(movePacket.PosInfo);
     


            // 위치가 같은데 방향만 다른건 바로 실행해라.

            if( bc.PosInfo.PosX == movePacket.PosInfo.PosX && bc.PosInfo.PosY == movePacket.PosInfo.PosY)
            {
                bc.PosInfo = bc.PosHistory[0];
                bc.PosHistory.RemoveAt(0);
            }

            //if (bc.PosHistory.Count > 0)
            //{
            //    bc.PosInfo = bc.PosHistory[0];
            //    Managers.Chat.ChatRPC($"유저 이동 4");
            //}

      
            return;
        }




        bc.PosInfo = movePacket.PosInfo;

        //Debug.Log("S_MoveHandler");
    }

    public static void S_TeleportHandler(PacketSession session, IMessage packet)
    {
        S_Teleport movePacket = packet as S_Teleport;
        //ServerSession serverSession = session as ServerSession;


        GameObject go = Managers.Object.FindById(movePacket.ObjectId);

        if (go == null)
            return;


        // 나의 플레이어 일 경우, 서버의 패킷을 받지 않는다. 다른 플레이어꺼만 받는다.
        // 엇갈릴 경우를 대비해 임시로 서버의 위치는 저장해놓는다. 추후 MyPlayerController에서 검증
        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
        {
            MyPlayerController myPC = go.GetComponent<MyPlayerController>();
            if (myPC == null)
                return;




            myPC.TempPosInfo = movePacket.PosInfo;

            myPC.PosInfo = movePacket.PosInfo;
            myPC.SyncPos(); //부드럽게 이동하는것 방지

            // 서버에서 받은 실제 플레이어의 위치를 저장해준다.
            myPC.UpdatePositionUI(myPC.TempPosInfo.PosX, myPC.TempPosInfo.PosY);

            // 텔레포트도 사용한거니까
            // myPC.UseSkill(3101000);
  
            return;
        }





        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;


        bc.PosInfo = movePacket.PosInfo;

        bc.SyncPos(); //부드럽게 이동하는것 방지

        bc.PosHistory.Clear();


        

        //Debug.Log("S_MoveHandler");
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;


        GameObject go = Managers.Object.FindById(skillPacket.ObjectId); // 우선 스킬쓴 놈의 아이디를 받는다.
        GameObject target = Managers.Object.FindById(skillPacket.TargetId); // 우선 스킬 받은 놈의 아이디를 받는다.

        //Debug.Log(skillPacket.TargetId);

        if (go == null)
            return;

        // 아이템 줍기라면 지워주고 리턴한다.
  

        CreatureController cc = go.GetComponent<CreatureController>();
        cc.Target = null;


        if (cc != null)
        {

            if (cc.GetType() == typeof(PlayerController) /*|| cc.GetType() == typeof(MonsterController)*/)
            {


                cc.Dir = skillPacket.Info.MoveDir;

                cc.State = CreatureState.Skill;

                // 스킬 쓴 순간부터는 끝까지
                //cc.PosHistory.Clear();

                if (cc.PosHistory.Count > 0)
                {

                    // 모든 애들의 방향을 다 최종으로 바꾼다.
                    for (int i = 0; i < cc.PosHistory.Count; i++)
                    {
                        cc.PosHistory[i].MoveDir = skillPacket.Info.MoveDir;
                    }
                }

            }

            cc.UseSkill(skillPacket.Info.SkillId);

            // 스킬 쓸때만큼은 즉각적으로 방향 전환하게 ( 이동하는 도중에 방향전환 스킬 쓴거는 바로 되게. )
            // 멈춘상태에서 방향전환후 바로 쏘는건, move 쪽에서 처리함

         
        }

        // 투척기라면
        if (skillPacket.ProjectileInfo != null)
        {

            GameObject projectile = Managers.Resource.Instantiate("Creature/Projectile");
            projectile.name = "Projectile";

            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.projectileInfo = skillPacket.ProjectileInfo;
            pc.distance = skillPacket.ProjectileInfo.Distance;

            // 타겟이 있다면
            if (target != null)
                pc.target = target;

        }



        // 스킬쿨은 UseSkill 맨 아래부분에 있다.
        MyPlayerController mc = go.GetComponent<MyPlayerController>();
        if(mc != null)
            mc.IsSkillSend = false;


        if (mc != null && skillPacket.Info.SkillId != -1)
        {
            

            string AA = DateTime.Now.ToString("ss.fffffff");
            float NowTime = float.Parse(AA);
            float BB = NowTime;

            Managers.Chat.ChatRPC($"<color=#000000>(1) : {mc.B*1000} / (2) : {BB * 1000} / 지연율 : {(BB-mc.B)*1000}</color>");

        }


        if(skillPacket.Info.SkillId != -1)
            Debug.Log("--------------------- 스킬 획득 완료");

        // 줍기라면 그냥 삭제해준다.
        if (skillPacket.Info.SkillId == 9101001)
        {
            return;
        }


        //// 단거리 공격이고 타겟이 있을때
        //if (target != null)
        //{
        //    // 타겟을 저장한다.
        //    if (cc != null)
        //    {
        //        cc.Target = target;
        //    }

        //    cc.Target.GetComponent<CreatureController>().OnDamaged(skillPacket.Damage);
        //    //Debug.Log("S_MoveHandler");

        //}

    }


    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = packet as S_ChangeHp;


        GameObject go = Managers.Object.FindById(changePacket.ObjectId); // 우선 받는 놈의 아이디를 받는다.

        if (go == null)
            return;


        CreatureController cc = go.GetComponent<CreatureController>();

        if (cc != null)
        {
            // Stat을 통해 바꿔주면 안되고, 바로 Hp 로 바꿔줘야 업데이트가 됨.
            //cc.Stat.Hp = changePacket.Hp;
            cc.Hp = changePacket.Hp;

            List<int> DamageList = new List<int>();

            for (int i = 0; i < changePacket.MultiDamage.Count; i ++)
            {
                DamageList.Add(changePacket.MultiDamage[i]);
            }

            cc.OnDamaged(changePacket.Damage,changePacket.SkillId, DamageList ,changePacket.AttackerId);

            //// TODO : UI
            //Debug.Log($"ChangeHp : {changePacket.Hp}");

        }
        //Debug.Log("S_MoveHandler");
    }




    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;


        GameObject go = Managers.Object.FindById(diePacket.ObjectId); // 우선 스킬쓴 놈의 아이디를 받는다.

        if (go == null)
            return;


        CreatureController cc = go.GetComponent<CreatureController>();

        if (cc != null)
        {
            // cc.Hp = 0;
            cc.OnDead(diePacket.Damage);

        }

    }




    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        // 서버에서 연결된것만 그냥 확인한다.
        Debug.Log("S_ConnectedHandler");

        C_Login loginPacket = new C_Login();

        string path = Application.dataPath;
        //loginPacket.UniqueId = path.GetHashCode().Tostring();// SystemInfo.deviceUniqueIdentifier; // 기기 고유의 값

        // 비밀번호 혹은 토큰까지 보내서 확인을 한번더 받는다.
        loginPacket.UniqueId = Managers.Network.AccountName;// SystemInfo.deviceUniqueIdentifier; // 기기 고유의 값
        loginPacket.Password = Managers.Network.Password;
        //loginPacket.UniqueId = loginSceneUI.AccountName_Save;//SystemInfo.deviceUniqueIdentifier; // 기기 고유의 값
        //Debug.Log("AccountName : "+ loginPacket.UniqueId);

        Managers.Network.Send(loginPacket);
    }

    // 로그인 Ok + 캐릭터 목록
    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        // 백프로 S_Login  이면 (S_Login) 하는것이 as S_Login 보다 성능이좋다,
        S_Login loginPacket = (S_Login)packet;
        Debug.Log($"Login Ok( {loginPacket.LoginOk} )");

        // TODO : 로비 UI에서 캐릭터 보여주고, 선택할 수 있도록




        // 캐릭터가 없거나 카운트가 0 이면 만들어달라는 요청
        if(loginPacket.Players == null || loginPacket.Players.Count ==0)
        {

            //TODO 캐릭터 정보를 받아와야함


            // 팝업을 띄어준다.
            UI_CreateCharacterPopup popup = Managers.UI.ShowPopupUI<UI_CreateCharacterPopup>();



            //C_CreatePlayer createPacket = new C_CreatePlayer();
            //createPacket.Name = $"PLAYER_{Random.Range(0, 10000).Tostring("0000")}";
            //Managers.Network.Send(createPacket); // 서버한테 캐릭터 만들어달라고 요청
        }
        else
        {
            // 캐릭터가 있는 경우 , // 무조건 첫번째로 로그인
            // 지금은 어차피 한 아이디당 한 플레이어만 있을거다.
            LobbyPlayerInfo info = loginPacket.Players[0];
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = info.Name;
            Managers.Network.Send(enterGamePacket);

            Debug.Log("캐릭터로 들어올거에요");



        }

    }

    public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;


        // 뭔가 어떤이유로 캐릭터가 안만들어졌을 때 (닉네임 중복으로)
        if(createOkPacket.Player == null)
        {

            // 팝업을 띄어준다.
            UI_CreateCharacterPopup popup = Managers.UI.ShowPopupUI<UI_CreateCharacterPopup>();


            //// 다시한번 만들어달라고 한다.

            //C_CreatePlayer createPacket = new C_CreatePlayer();
            //createPacket.Name = $"PLAYER_{Random.Range(0, 10000).Tostring("0000")}";

            //Managers.Network.Send(createPacket); // 서버한테 캐릭터 만들어달라고 요청
        }
        else
        {
            // 만들어지면 createOkPacket 의 Player Name 으로 enterGame 패킷을 보낸다.
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = createOkPacket.Player.Name;
            Managers.Network.Send(enterGamePacket);

        }
    }

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemList = (S_ItemList)packet;

    
        // 문제가 생기면 크래쉬
        //if (gameSceneUI == null)
        //    return;



        // 잔재들 날려주기
        Managers.Inven.Clear();

        // 메모리에 아이템 정보 적용
        foreach(ItemInfo itemInfo in itemList.Items)
        {
            //Debug.Log($"{item.TemplateId}: {item.Count}");

            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item);

            Debug.Log("아이템 " + itemInfo.Slot + ":" + itemInfo.TemplateId);
        }
        // 켜질때 무조건 UI 켜지게하는것.

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;


        //UI 에서 표시
        invenUI.gameObject.SetActive(true);
        invenUI.RefreshUI();
        //gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.


        // 아이템이 변경되었을 때 나의 캐릭터의 스텟도 변경해준다.
        if (Managers.Object.MyPlayer != null)
            Managers.Object.MyPlayer.RefreshAdditionalStat();

 


    }



    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemList = (S_AddItem)packet;

        // 중간에 아이템이 새로 들어온 것이므로 clear 하면 안됨.
        //Managers.Inven.Clear();


        // 메모리에 아이템 정보 적용
        foreach (ItemInfo itemInfo in itemList.Items)
        {
            //Debug.Log($"{item.TemplateId}: {item.Count}");

            if(itemInfo.TemplateId == 99999)
            {
                if (Managers.Object.MyPlayer != null)
                    Managers.Object.MyPlayer.Stat.Gold += itemInfo.Count;

                Managers.Chat.ChatRPC("<color=#F7D358>골드를 획득했습니다. (+" + itemInfo.Count + ")</color>");
            }
            else
            {
                Item item = Item.MakeItem(itemInfo);
                Managers.Inven.Add(item);

                Data.ItemData itemData = null;
                Managers.Data.ItemDict.TryGetValue(item.TemplateId, out itemData);

                if (itemData == null)
                    return;

                Managers.Chat.ChatRPC("<color=#F7D358>아이템을 획득했습니다. (+" + itemData.name + ")</color>");
            }



        }


        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.

        // 단축키도 리프뤠시
        UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        KeySettingUI.RefreshUI();

        // 아이템이 변경되었을 때 나의 캐릭터의 스텟도 변경해준다.
        if (Managers.Object.MyPlayer != null)
            Managers.Object.MyPlayer.RefreshAdditionalStat();
    }



    public static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        S_EquipItem equipItemOk = (S_EquipItem)packet;

        // 메모리에 아이템 정보 적용
        Item item = Managers.Inven.Get(equipItemOk.ItemDbId);
        if (item == null)
            return;

        item.Equipped = equipItemOk.Equipped;
        Debug.Log("아이템을 착용 변경!");


        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.

        // 아이템이 변경되었을 때 나의 캐릭터의 스텟도 변경해준다.
        if (Managers.Object.MyPlayer != null)
            Managers.Object.MyPlayer.RefreshAdditionalStat();

    }



    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {

        // TODO

        S_ChangeStat changePacket = (S_ChangeStat)packet;


        GameObject go = Managers.Object.FindById(changePacket.ObjectId); // 우선 받는 놈의 아이디를 받는다.

        if (go == null)
            return;


        CreatureController cc = go.GetComponent<CreatureController>();

        if (cc != null)
        {
            // Stat을 통해 바꿔주면 안되고, 바로 Hp 로 바꿔줘야 업데이트가 됨.
            //cc.Stat.Hp = changePacket.Hp;
            cc.Stat.Helmet = changePacket.StatInfo.Helmet;
            cc.Stat.RightHand = changePacket.StatInfo.RightHand;
            cc.Stat.LeftHand = changePacket.StatInfo.LeftHand;
            cc.Stat.Shirts = changePacket.StatInfo.Shirts;
            cc.Stat.Pants = changePacket.StatInfo.Pants;
            cc.Stat.Shoes = changePacket.StatInfo.Shoes;

        }

        cc.GetComponent<CharacterAnimation>().CharacterApearance_Refresh();


    }

    public static void S_PingHandler(PacketSession session, IMessage packet)
    {
        C_Pong pongPacket = new C_Pong();
        //Debug.Log("[Server] PingCheck");

        Managers.Network.Send(pongPacket);
    

    }

    public static void S_MoveMapHandler(PacketSession session, IMessage packet)
    {

        //S_MoveMap mapPacket = packet as S_MoveMap;

        //int mapId = mapPacket.StatInfo.Map;

        //if(mapId == 10100000)
        //{
        //    Managers.Sound.Play("Sounds/Bgm/1", Define.Sound.Bgm);
        //}
        //else
        //{
        //    Managers.Sound.Play("Sounds/Bgm/2", Define.Sound.Bgm);
        //}
        Managers.Sound.Play("Sounds/Etc/Portal", Define.Sound.Effect);
    }

    public static void S_LogoutHandler(PacketSession session, IMessage packet)
    {

        // 이거 패킷 오기전에  Disconnected가 되서.. 그냥 

        // UI_GameScene의 OnClickedExit 부터 로그인 창으로 가게만들었따.



        //S_Logout logoutPacekt = packet as S_Logout;

        //Debug.Log("로그아웃 되었습니다.11");
        // session._disconnected = 1 이됨. 그러면 나중에 안받음.
        //Managers.Network.DisconnectFromGame();
        //Managers.Scene.LoadScene(Define.Scene.Login);



        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;

        //        Managers.Network.DisconnectFromGame();

        //#else
        //                        Application.Quit(); // 어플리케이션 종료
        //#endif




        //// 클라이언트단에서도 서버랑 세션을 끊어줘야 다시 로그인이 가능함.
        //// 서버세션도 초기화됨
        //Managers.Network.DisconnectFromGame();


        //Managers.s_instance = null;








        // 매니저 모든 값들 초기화
        // 임시방편임..
        //GameObject.Destroy(GameObject.Find("@Managers"));
        //GameObject.Destroy(GameObject.Find("@Pool_Root"));
        //GameObject.Destroy(GameObject.Find("@Sound"));

        //// 로그인 창으로 가게 해준다.
        //Managers.Scene.LoadScene(Define.Scene.Login);
    }




    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_Chat chatPacket = packet as S_Chat;

        GameObject go = Managers.Object.FindById(chatPacket.ObjectId);

        if (go == null)
            return;


        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        //bc.PosInfo = chatPacket.PosInfo;

        Managers.Chat.ChatRPC(chatPacket.Name + " : " + chatPacket.Message);

        go.transform.Find("Canvas/ChatBox/ChatText").GetComponent<Text>().text = chatPacket.Name + " : " + chatPacket.Message;
        go.transform.Find("Canvas/ChatBox").transform.GetComponent<Image>().enabled = true;
        go.transform.Find("Canvas/ChatBox/ChatText").GetComponent<ChatDelete>().timer = 0f;

    }

    public static void S_UseItemHandler(PacketSession session, IMessage packet)
    {
        S_UseItem useItemOk = (S_UseItem)packet;

        // 메모리에 아이템 정보 적용
        Item item = Managers.Inven.Get(useItemOk.ItemDbId);
        if (item == null)
            return;

        item.Count = useItemOk.Count;
        Debug.Log($"아이템 사용! 몇개 남음 : {item.Count}개");

        if(item.ItemType == ItemType.Consumable)
        {
            Consumable A = (Consumable)item;

            if(A.ConsumableType == ConsumableType.Potion)
                Managers.Sound.Play("Sounds/Item/Potion/1", Define.Sound.Effect);
        }

    

        Debug.Log($"---------------------------------------------");

        if (item.Count <= 0)
        {
            foreach (Item t in Managers.Inven.Items.Values)
            {
                Debug.Log($"Item {t.ItemDbId} / slot : {t.Slot}");
            }

            Managers.Inven.Delete(useItemOk.ItemDbId);

            foreach (Item t in Managers.Inven.Items.Values)
            {
                Debug.Log($"Item {t.ItemDbId}  / slot : {t.Slot}");
            }
        }

       


        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.

        // 단축키도 리프뤠시
        UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        KeySettingUI.RefreshUI();

        // 아이템이 변경되었을 때 나의 캐릭터의 스텟도 변경해준다.
        if (Managers.Object.MyPlayer != null)
        {
            Managers.Object.MyPlayer.RefreshAdditionalStat();

            Managers.Object.MyPlayer.ConsumeCool();
        }

    }


    public static void S_KeySettingHandler(PacketSession session, IMessage packet)
    {
        S_KeySetting keyList = packet as S_KeySetting;

        // TODO

        // 메모리에 아이템 정보 적용
        foreach (KeySettingInfo keyInfo in keyList.KeySettingInfo)
        {
            //Debug.Log($"{item.TemplateId}: {item.Count}");

            if (keyInfo.Action != -1)
            {
                Key key = Key.MakeKey(keyInfo);
                Managers.KeySetting.Add(key);
            }
            else if (keyInfo.Action == -1)
            {
                Key key = Key.MakeKey(keyInfo);
                Managers.KeySetting.Delete(key);
            }

            Debug.Log($"추가된 단축키 = KeyDbID : {keyInfo.KeyDbId} / 단축키 : {keyInfo.Key} / 액션 : {keyInfo.Action}");
        }


        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        KeySettingUI.RefreshUI();
        //gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.



    }


    public static void S_ExpHandler(PacketSession session, IMessage packet)
    {
        S_Exp expPacket = (S_Exp)packet;

        // TODO
        // 아이템이 변경되었을 때 나의 캐릭터의 스텟도 변경해준다.
        if (Managers.Object.MyPlayer != null)
        {
            if (expPacket.LevelUp != true)
            {
                Managers.Object.MyPlayer.Stat.Exp += expPacket.Exp;

                Managers.Chat.ChatRPC("<color=#F7D358>경험치를 획득하였습니다. (+"+expPacket.Exp+ ")</color>");
            }
            else
            {


                Managers.Object.MyPlayer.Stat.Exp = 0;
                Managers.Object.MyPlayer.Stat.TotalExp = expPacket.TotalExp;
                Managers.Object.MyPlayer.Stat.Level += 1;
                Managers.Object.MyPlayer.Stat.StatPoint += 5;
            }
                
        }

        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.



    }



    public static void S_KeySettingListHandler(PacketSession session, IMessage packet)
    {


        S_KeySettingList keyList = packet as S_KeySettingList; ;

        // TODO

        // 잔재들 날려주기
        Managers.KeySetting.Clear();

        // 메모리에 아이템 정보 적용
        foreach (KeySettingInfo keyInfo in keyList.KeySettingInfo)
        {
            //Debug.Log($"{item.TemplateId}: {item.Count}");

            Key key = Key.MakeKey(keyInfo);
            Managers.KeySetting.Add(key);

            Debug.Log($"추가된 단축키 = KeyDbID : {keyInfo.KeyDbId} / 단축키 : {keyInfo.Key} / 액션 : {keyInfo.Action}");

        }

        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        KeySettingUI.RefreshUI();


    }

  
    public static void S_DropItemHandler(PacketSession session, IMessage packet)
    {


        S_DropItem dropItemOK = packet as S_DropItem; ;


        // 메모리에 아이템 정보 적용
        Item item = Managers.Inven.Get(dropItemOK.ItemDbId);
        if (item == null)
            return;

        //item.Count = dropItemOK.Count;
        //Debug.Log($"아이템 사용! 몇개 남음 : {item.Count}개");



        //Debug.Log($"---------------------------------------------");

        //if (item.Count <= 0)
        //{
        //    foreach (Item t in Managers.Inven.Items.Values)
        //    {
        //        Debug.Log($"Item {t.ItemDbId} / slot : {t.Slot}");
        //    }

        //    Managers.Inven.Delete(dropItemOK.ItemDbId);

        //    foreach (Item t in Managers.Inven.Items.Values)
        //    {
        //        Debug.Log($"Item {t.ItemDbId}  / slot : {t.Slot}");
        //    }
        //}

        // count가 0개일 경우만 지워준다.
        if(dropItemOK.Count ==0)
            Managers.Inven.Delete(dropItemOK.ItemDbId);
        else
        {
            // 0개 아닐 경우 갯수만 갱신해준다.
            item.Count = dropItemOK.Count;
        }



        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.

        // 단축키도 리프뤠시
        UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        KeySettingUI.RefreshUI();

        // 아이템이 변경되었을 때 나의 캐릭터의 스텟도 변경해준다.
        if (Managers.Object.MyPlayer != null)
            Managers.Object.MyPlayer.RefreshAdditionalStat();

    }

    public static void S_NpcHandler(PacketSession session, IMessage packet)
    {
        // NPC 대화

        S_Npc Dialogue = packet as S_Npc;

        //Debug.Log($"NPC 다이얼로그 : {Dialogue.Dialouge}");

        Data.NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(Dialogue.NpcInfo.StatInfo.TemplateId, out npcData);

        string npcChat = "";

        foreach(ChatData chatData in npcData.chats)
        {
            if (chatData.index == Dialogue.Dialogue)
            {
                //Debug.Log($"NPC 대화 : {chatData.chat}");

                npcChat = chatData.chat;
            }
        }

  

        // NPC의 기본 퀘스트 정보들을 준다.

        //List<QuestInfo> questData = new List<QuestInfo>();

        GameObject npc = Managers.Object.FindById(Dialogue.NpcInfo.ObjectId);

        NpcController npcController = npc.GetComponent<NpcController>();

        Debug.Log("npc 테스트 : " + npcController.Stat.TemplateId);




        // 엔피씨가 원래 갖고 있던 퀘스트 리스트들 초기화
        npcController.NpcQuestList.Clear();

        foreach(QuestInfo p in Dialogue.Quests)
        {
            // npc를 찾고 그 npc의 퀘스트 데이터에 이 값들을 넣어준다.

            // 이쪽에서 퀘스트 정보들을 준다.

            Data.QuestData questData = null;
            Managers.Data.QuestDict.TryGetValue(p.QuestTemplateId, out questData);


            // QuestInfo => QuestData
            QuestData A = new QuestData();

            A.npc = npcController.Stat.TemplateId;
            A.questId = p.QuestTemplateId;
            A.status = p.Status;
            A.questName = questData.questName;

            npcController.NpcQuestList.Add(A);
        }


        // 팝업을 띄어준다.
        UI_DialoguePopup popup = Managers.UI.ShowPopupUI<UI_DialoguePopup>();


        // NPC의 기본 대사를 얘기해준다.
        popup.npcInfo = Dialogue.NpcInfo;
        popup.RefreshDialogue(npcChat, Dialogue.Dialogue, npcController);


        // 맵에 있는 NPC들의 퀘스트 아이콘들을 리프레쉬 해준다
        // => UI_DialoguePopup 으로 이동

    }



    public static void S_ShopHandler(PacketSession session, IMessage packet)
    {

        S_Shop Shop = packet as S_Shop; ;
        Debug.Log(Shop.NpcInfo + ":" +Shop.Items);


        List<ItemInfo> products = new List<ItemInfo>();

        foreach(ItemInfo item in Shop.Items)
        {
            products.Add(item);
        }

        // 팝업을 띄어준다.
        UI_ShopPopup popup = Managers.UI.ShowPopupUI<UI_ShopPopup>();

        popup.npcInfo = Shop.NpcInfo;
        popup.SetProducts(products);
    }



    public static void S_StatUpHandler(PacketSession session, IMessage packet)
    {

        S_StatUp statup = packet as S_StatUp;


        if (Managers.Object.MyPlayer != null)
        {
            Managers.Object.MyPlayer.Stat.Str = statup.Stat.Str;
            Managers.Object.MyPlayer.Stat.Dex = statup.Stat.Dex;
            Managers.Object.MyPlayer.Stat.Int = statup.Stat.Int;
            Managers.Object.MyPlayer.Stat.Luk = statup.Stat.Luk;
            Managers.Object.MyPlayer.Stat.StatPoint = statup.Stat.StatPoint;


            // 캐릭터가 들어오면 그때 아이템과 스텟을 갱신해준다.

            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;

            invenUI.RefreshUI();
            gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.
        }

    }


    public static void S_SkillListHandler(PacketSession session, IMessage packet)
    {


        S_SkillList skillList = packet as S_SkillList; ;


        // 잔재들 날려주기
        Managers.Skill.Clear();

        // 메모리에 아이템 정보 적용
        foreach (SkillInfo skillInfo in skillList.SkillInfo)
        {
            Skills skill = Skills.MakeSkill(skillInfo);

            Managers.Skill.Add(skill);
        }

        // 리프레쉬
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Skill SkillUI = gameSceneUI.SkillUI;
        SkillUI.RefreshUI();

    }


    public static void S_ChangeMpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeMp changePacket = packet as S_ChangeMp;


        GameObject go = Managers.Object.FindById(changePacket.ObjectId); // 우선 받는 놈의 아이디를 받는다.

        if (go == null)
            return;


        CreatureController cc = go.GetComponent<CreatureController>();

        if (cc != null)
        {
            // Stat을 통해 바꿔주면 안되고, 바로 Hp 로 바꿔줘야 업데이트가 됨.
            cc.Mp = changePacket.Mp;

            // 리프레쉬
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;
            invenUI.RefreshUI();
            gameSceneUI.StatUI.RefreshUI(); // 스텟도 리프레쉬 해준다.


        }
        //Debug.Log("S_MoveHandler");
    }


    public static void S_QuestListHandler(PacketSession session, IMessage packet)
    {


        S_QuestList questList = packet as S_QuestList; ;

        // TODO

        // 잔재들 날려주기
        Managers.Quest.Quests.Clear();

        // 메모리에 아이템 정보 적용
        foreach(QuestInfo questInfo in questList.QuestInfo)
        {
            Quest quest = Quest.MakeQuest(questInfo);
            Managers.Quest.Add(quest);
        }

        //// 리프레쉬
        //UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        //UI_KeySetting KeySettingUI = gameSceneUI.KeySettingUI;
        //KeySettingUI.RefreshUI();

        // 갱신된 퀘스트들 중에 npc들이 같은 존에 있는 애들이라면,
        // 그 npc들의 퀘스트 상태를 바꾸어 준다.

    }

    public static void S_UsersHandler(PacketSession session, IMessage packet)
    {

        S_Users userList = packet as S_Users;


        // 팝업을 띄어준다.
        UI_UserListPopup popup = Managers.UI.ShowPopupUI<UI_UserListPopup>();

        // 팝업의 UserList 초기화
        popup.UserList.Clear();

        int i = 0;
        foreach(ObjectInfo A in userList.ObjectInfo)
        {

            MapInfoData mapData = null;
            Managers.Data.MapDict.TryGetValue(A.StatInfo.Map, out mapData);

            Debug.Log($" {i} - 접속 중인 유저 : {A.Name} / 레벨 : {A.StatInfo.Level} / 위치 : {mapData.name}");

            i++;

            popup.UserList.Add(A);

        }

        popup.RefreshUsers();




    }



    public static void S_LevelUpHandler(PacketSession session, IMessage packet)
    {
        S_LevelUp levelupPacket = packet as S_LevelUp;


        GameObject go = Managers.Object.FindById(levelupPacket.ObjectId); // 우선 받는 놈의 아이디를 받는다.

        if (go == null)
            return;


        PlayerController pc = go.GetComponent<PlayerController>();

        if (pc != null)
        {
            // Stat을 통해 바꿔주면 안되고, 바로 Hp 로 바꿔줘야 업데이트가 됨.
            //cc.Stat.Hp = changePacket.Hp;
            pc.MaxHp = levelupPacket.StatInfo.MaxHp;
            pc.MaxMp = levelupPacket.StatInfo.MaxMp;


            pc.LevelUp();


        }
        //Debug.Log("S_MoveHandler");
    }


}
