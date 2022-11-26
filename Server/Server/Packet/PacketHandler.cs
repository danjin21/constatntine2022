using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.DB;
using Server.Game;
using ServerCore;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PacketHandler
{
	//public static void C_ChatHandler(PacketSession session, IMessage packet)
	//{
	//	S_Chat chatPacket = packet as S_Chat;
	//	ClientSession serverSession = session as ClientSession;

	//	Console.WriteLine(chatPacket.Context);
	//}


	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX},{movePacket.PosInfo.PosY})");

		// 외부에서 빼올때는 lock 이 상관없다 항상 로컬저장

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		//room.HandleMove(player, movePacket);
		room.Push(room.HandleMove, player, movePacket); // => JobQueue 화
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

        Console.WriteLine("스킬 들어옴");
		//room.HandleSkill(player, skillPacket);
		room.Push(room.HandleSkill, player, skillPacket); // => JobQueue화
	}



	public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
		C_Login loginPacket = packet as C_Login;
		ClientSession clientSession = session as ClientSession;

		// ClientSession_PreGame.cs 에 있는 것을 실행시킨다.
		clientSession.HandleLogin(loginPacket);

		Console.WriteLine("HandleLogin!");

	}

	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
	{
		C_EnterGame enterGamePacket = (C_EnterGame)packet;
		ClientSession clientSession = (ClientSession)session;

		clientSession.HandleEnterGame(enterGamePacket);

        Console.WriteLine("HandleEnterGame!");


	}

	public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		C_CreatePlayer createPlayerPacket = (C_CreatePlayer)packet;
		ClientSession clientSession = (ClientSession)session;

		int Hair_End = 3;
		int Face_End = 5;
		int Skin_End = 1;

		// 값 이상하게 보내면 실행안한다.
		if (createPlayerPacket.Face < 0 || createPlayerPacket.Face > Face_End ||
			createPlayerPacket.Hair < 0 || createPlayerPacket.Hair > Hair_End ||
			createPlayerPacket.Skin < 0 || createPlayerPacket.Skin > Skin_End ||
				createPlayerPacket.Gender < 1 || createPlayerPacket.Gender > 2)
			return;




		// ClientSession_PreGame.cs 에 있는 것을 실행시킨다.
		clientSession.HandleCreatePlayer(createPlayerPacket);
	}


	public static void C_EquipItemHandler(PacketSession session, IMessage packet)
	{
		C_EquipItem equipPacket = (C_EquipItem)packet;
		ClientSession clientSession = (ClientSession)session;


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		//room.HandleSkill(player, skillPacket);
		room.Push(room.HandleEquipItem, player, equipPacket); // => JobQueue화
	}

	public static void C_PongHandler(PacketSession session, IMessage packet)
	{
		// 이건 문제가 안생길 가능성이 높으므로 Lock을 걸지 않는다.
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandlePong();
	}



	public static void C_LogoutHandler(PacketSession session, IMessage packet)
	{
		// 이건 문제가 안생길 가능성이 높으므로 Lock을 걸지 않는다.
		C_Logout logoutPacket = (C_Logout)packet;
		ClientSession clientSession = (ClientSession)session;

		clientSession.HandleLogout(logoutPacket);


	}


	public static void C_ChatHandler(PacketSession session, IMessage packet)
	{

		C_Chat chatPacket = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;


		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		//room.HandleMove(player, movePacket);
		room.Push(room.HandleChat, player, chatPacket); // => JobQueue 화


	}

	public static void C_UseItemHandler(PacketSession session, IMessage packet)
	{
		C_UseItem usePacket = (C_UseItem)packet;
		ClientSession clientSession = (ClientSession)session;


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		//room.HandleSkill(player, skillPacket);
		room.Push(room.HandleUseItem, player, usePacket); // => JobQueue화
	}


	public static void C_KeySettingHandler(PacketSession session, IMessage packet)
	{
		C_KeySetting keysettingPacket = (C_KeySetting)packet;
		ClientSession clientSession = (ClientSession)session;


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		if (keysettingPacket.Action >= 1000000)
		{

			keysettingPacket.Type = 2;

		}
		else if (keysettingPacket.Action >= 90000 && keysettingPacket.Action < 1000000)
		{

			keysettingPacket.Type = 1;
		}

		Console.WriteLine($"단축키 정보 전달 : 키 = {keysettingPacket.Key} / 타입 = {keysettingPacket.Type} / 액션 = {keysettingPacket.Action}");

		room.Push(room.HandleKeySetting, player, keysettingPacket); // => JobQueue화

	}


	public static void C_DropItemHandler(PacketSession session, IMessage packet)
	{
		C_DropItem dropItemPacket = (C_DropItem)packet;
		ClientSession clientSession = (ClientSession)session;


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		//room.HandleSkill(player, skillPacket);
		room.Push(room.HandleDropItem, player, dropItemPacket); // => JobQueue화

	}


	public static void C_NpcHandler(PacketSession session, IMessage packet)
	{
		C_Npc npcPacket = (C_Npc)packet;
		ClientSession clientSession = (ClientSession)session;

		Console.WriteLine("NPC" + npcPacket.ObjectId);


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		// 첫 대화 시도가 아니라, 퀘스트 버튼을 눌러서 온 패킷이라면
		if (npcPacket.Quest != 0)
		{
			// HandleQuest 로 가게 만든다.
			room.Push(room.HandleQuest, player, npcPacket); // => JobQueue화

		}
		else
			room.Push(room.HandleNpc, player, npcPacket); // => JobQueue화

	}


	public static void C_PurchaseHandler(PacketSession session, IMessage packet)
    {
		C_Purchase purchasePacket = (C_Purchase)packet;
		ClientSession clientSession = (ClientSession)session;

		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandlePurchase, player, purchasePacket); // => JobQueue화

	}



	public static void C_SellHandler(PacketSession session, IMessage packet)
	{
		C_Sell sellPacket = (C_Sell)packet;
		ClientSession clientSession = (ClientSession)session;

		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSell, player, sellPacket); // => JobQueue화

	}

	public static void C_StatUpHandler(PacketSession session, IMessage packet)
    {
		C_StatUp statupPacket = (C_StatUp)packet;
		ClientSession clientSession = (ClientSession)session;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleStatUp, player, statupPacket); // => JobQueue화

	}


	public static void C_ShortKeyHandler(PacketSession session, IMessage packet)
	{
		C_ShortKey shortKeyPacket = (C_ShortKey)packet;
		ClientSession clientSession = (ClientSession)session;


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		if (player.ShortKeyCool == true)
			return;

        //// 쿨타임 주기.
        //player.ShortKeyCool = true;
        //player.Room.PushAfter(50, player.ShortKeyCooltime);

        //Console.WriteLine("#5");
        room.Push(room.HandleShortKey, player, shortKeyPacket); // => JobQueue화



	}

	public static void C_SlotChangeHandler(PacketSession session, IMessage packet)
	{
		C_SlotChange slotChangePacket = (C_SlotChange)packet;
		ClientSession clientSession = (ClientSession)session;


		// 로비가 아니므로. Room 으로 

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

        Console.WriteLine("주문표 : 바꾸고 싶은 아이템 : " + slotChangePacket.ItemDbId + "/ 바뀔 슬롯 : " + slotChangePacket.Slot);

		room.Push(room.HandleSlotChange, player, slotChangePacket);

	}

	public static void C_UsersHandler(PacketSession session, IMessage packet)
    {
		C_Users usersPacket = (C_Users)packet;
		ClientSession clientSession = (ClientSession)session;


		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		// 멀티스레드에서 null로 될수 있기에 항상 로컬저장
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleUsers, player, usersPacket);
	}

}
