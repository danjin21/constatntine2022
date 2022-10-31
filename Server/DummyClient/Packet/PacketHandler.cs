using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;

class PacketHandler
{
    //public static void S_ChatHandler(PacketSession session, IMessage packet)
    //{
    //	S_Chat chatPacket = packet as S_Chat;
    //	ServerSession serverSession = session as ServerSession;

    //	Debug.Log(chatPacket.Context);
    //}

    // Step 4
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
       


    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
      
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
      

    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;



    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
       
    }

    public static void S_TeleportHandler(PacketSession session, IMessage packet)
    {
        S_Teleport movePacket = packet as S_Teleport;
       
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;



    }


    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = packet as S_ChangeHp;


    }




    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;



    }



    // Step 1
    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {

        C_Login loginPacket = new C_Login();

        ServerSession serverSession = (ServerSession)session;

        loginPacket.UniqueId = $"DummyClient_{serverSession.DummyId.ToString("0000")}";

        serverSession.Send(loginPacket);

    }

    // Step 2
    // 로그인 Ok + 캐릭터 목록
    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        // 백프로 S_Login  이면 (S_Login) 하는것이 as S_Login 보다 성능이좋다,
        S_Login loginPacket = (S_Login)packet;

        ServerSession serverSession = (ServerSession)session;


        // 캐릭터가 없거나 카운트가 0 이면 만들어달라는 요청
        if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        {

            //TODO 캐릭터 정보를 받아와야함

            C_CreatePlayer createPacket = new C_CreatePlayer();
            createPacket.Name = $"PLAYER_{serverSession.DummyId.ToString("0000")}";
            serverSession.Send(createPacket); // 서버한테 캐릭터 만들어달라고 요청
        }
        else
        {
            // 캐릭터가 있는 경우 , // 무조건 첫번째로 로그인
            // 지금은 어차피 한 아이디당 한 플레이어만 있을거다.
            LobbyPlayerInfo info = loginPacket.Players[0];
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = info.Name;
            serverSession.Send(enterGamePacket);

        }
    }

    // Step 3
    public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;

        ServerSession serverSession = (ServerSession)session;


        // 뭔가 어떤이유로 캐릭터가 안만들어졌을 때 (닉네임 중복으로)
        if (createOkPacket.Player == null)
        {
            //생략

        }
        else
        {
            // 만들어지면 createOkPacket 의 Player Name 으로 enterGame 패킷을 보낸다.
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = createOkPacket.Player.Name;
            serverSession.Send(enterGamePacket);

        }


    }

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemList = (S_ItemList)packet;




    }



    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemList = (S_AddItem)packet;

    }



    public static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        S_EquipItem equipItemOk = (S_EquipItem)packet;


    }



    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {

        // TODO

        S_ChangeStat changePacket = (S_ChangeStat)packet;




    }

    public static void S_PingHandler(PacketSession session, IMessage packet)
    {
        C_Pong pongPacket = new C_Pong();
      

    }

    public static void S_MoveMapHandler(PacketSession session, IMessage packet)
    {

        S_MoveMap mapPacket = packet as S_MoveMap;


    }

    public static void S_LogoutHandler(PacketSession session, IMessage packet)
    {

  
    }




    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_Chat chatPacket = packet as S_Chat;

      
    }

    public static void S_UseItemHandler(PacketSession session, IMessage packet)
    {
        S_UseItem useItemOk = (S_UseItem)packet;

     

    }


    public static void S_KeySettingHandler(PacketSession session, IMessage packet)
    {
        S_KeySetting keyList = packet as S_KeySetting;

      


    }


    public static void S_ExpHandler(PacketSession session, IMessage packet)
    {
        S_Exp expPacket = (S_Exp)packet;

      



    }



    public static void S_KeySettingListHandler(PacketSession session, IMessage packet)
    {


        S_KeySettingList keyList = packet as S_KeySettingList; ;

     

    }


    public static void S_DropItemHandler(PacketSession session, IMessage packet)
    {


        S_DropItem dropItemOK = packet as S_DropItem; ;



    }

    public static void S_NpcHandler(PacketSession session, IMessage packet)
    {
        // NPC 대화

        S_Npc Dialogue = packet as S_Npc;

    }



    public static void S_ShopHandler(PacketSession session, IMessage packet)
    {

        S_Shop Shop = packet as S_Shop; ;
       
    }



    public static void S_StatUpHandler(PacketSession session, IMessage packet)
    {

        S_StatUp statup = packet as S_StatUp;



    }


    public static void S_SkillListHandler(PacketSession session, IMessage packet)
    {


        S_SkillList skillList = packet as S_SkillList; ;



    }


    public static void S_ChangeMpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeMp changePacket = packet as S_ChangeMp;


    }


    public static void S_QuestListHandler(PacketSession session, IMessage packet)
    {


        S_QuestList questList = packet as S_QuestList; ;


    }

    public static void S_UsersHandler(PacketSession session, IMessage packet)
    {

        S_Users userList = packet as S_Users;


       



    }



    public static void S_LevelUpHandler(PacketSession session, IMessage packet)
    {
        S_LevelUp levelupPacket = packet as S_LevelUp;


    }


}
