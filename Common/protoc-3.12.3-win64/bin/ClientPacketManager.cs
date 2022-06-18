using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SConnected, MakePacket<S_Connected>);
		_handler.Add((ushort)MsgId.SConnected, PacketHandler.S_ConnectedHandler);		
		_onRecv.Add((ushort)MsgId.SLogin, MakePacket<S_Login>);
		_handler.Add((ushort)MsgId.SLogin, PacketHandler.S_LoginHandler);		
		_onRecv.Add((ushort)MsgId.SCreatePlayer, MakePacket<S_CreatePlayer>);
		_handler.Add((ushort)MsgId.SCreatePlayer, PacketHandler.S_CreatePlayerHandler);		
		_onRecv.Add((ushort)MsgId.SItemList, MakePacket<S_ItemList>);
		_handler.Add((ushort)MsgId.SItemList, PacketHandler.S_ItemListHandler);		
		_onRecv.Add((ushort)MsgId.SAddItem, MakePacket<S_AddItem>);
		_handler.Add((ushort)MsgId.SAddItem, PacketHandler.S_AddItemHandler);		
		_onRecv.Add((ushort)MsgId.SEquipItem, MakePacket<S_EquipItem>);
		_handler.Add((ushort)MsgId.SEquipItem, PacketHandler.S_EquipItemHandler);		
		_onRecv.Add((ushort)MsgId.SChangeStat, MakePacket<S_ChangeStat>);
		_handler.Add((ushort)MsgId.SChangeStat, PacketHandler.S_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.SPing, MakePacket<S_Ping>);
		_handler.Add((ushort)MsgId.SPing, PacketHandler.S_PingHandler);		
		_onRecv.Add((ushort)MsgId.SMoveMap, MakePacket<S_MoveMap>);
		_handler.Add((ushort)MsgId.SMoveMap, PacketHandler.S_MoveMapHandler);		
		_onRecv.Add((ushort)MsgId.SLogout, MakePacket<S_Logout>);
		_handler.Add((ushort)MsgId.SLogout, PacketHandler.S_LogoutHandler);		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SUseItem, MakePacket<S_UseItem>);
		_handler.Add((ushort)MsgId.SUseItem, PacketHandler.S_UseItemHandler);		
		_onRecv.Add((ushort)MsgId.SKeySetting, MakePacket<S_KeySetting>);
		_handler.Add((ushort)MsgId.SKeySetting, PacketHandler.S_KeySettingHandler);		
		_onRecv.Add((ushort)MsgId.SExp, MakePacket<S_Exp>);
		_handler.Add((ushort)MsgId.SExp, PacketHandler.S_ExpHandler);		
		_onRecv.Add((ushort)MsgId.SKeySettingList, MakePacket<S_KeySettingList>);
		_handler.Add((ushort)MsgId.SKeySettingList, PacketHandler.S_KeySettingListHandler);		
		_onRecv.Add((ushort)MsgId.SDropItem, MakePacket<S_DropItem>);
		_handler.Add((ushort)MsgId.SDropItem, PacketHandler.S_DropItemHandler);		
		_onRecv.Add((ushort)MsgId.SNpc, MakePacket<S_Npc>);
		_handler.Add((ushort)MsgId.SNpc, PacketHandler.S_NpcHandler);		
		_onRecv.Add((ushort)MsgId.SShop, MakePacket<S_Shop>);
		_handler.Add((ushort)MsgId.SShop, PacketHandler.S_ShopHandler);		
		_onRecv.Add((ushort)MsgId.SStatUp, MakePacket<S_StatUp>);
		_handler.Add((ushort)MsgId.SStatUp, PacketHandler.S_StatUpHandler);		
		_onRecv.Add((ushort)MsgId.SSkillList, MakePacket<S_SkillList>);
		_handler.Add((ushort)MsgId.SSkillList, PacketHandler.S_SkillListHandler);		
		_onRecv.Add((ushort)MsgId.STeleport, MakePacket<S_Teleport>);
		_handler.Add((ushort)MsgId.STeleport, PacketHandler.S_TeleportHandler);		
		_onRecv.Add((ushort)MsgId.SChangeMp, MakePacket<S_ChangeMp>);
		_handler.Add((ushort)MsgId.SChangeMp, PacketHandler.S_ChangeMpHandler);		
		_onRecv.Add((ushort)MsgId.SQuestList, MakePacket<S_QuestList>);
		_handler.Add((ushort)MsgId.SQuestList, PacketHandler.S_QuestListHandler);		
		_onRecv.Add((ushort)MsgId.SUsers, MakePacket<S_Users>);
		_handler.Add((ushort)MsgId.SUsers, PacketHandler.S_UsersHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}