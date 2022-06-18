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
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CSkill, MakePacket<C_Skill>);
		_handler.Add((ushort)MsgId.CSkill, PacketHandler.C_SkillHandler);		
		_onRecv.Add((ushort)MsgId.CLogin, MakePacket<C_Login>);
		_handler.Add((ushort)MsgId.CLogin, PacketHandler.C_LoginHandler);		
		_onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_EnterGame>);
		_handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.CCreatePlayer, MakePacket<C_CreatePlayer>);
		_handler.Add((ushort)MsgId.CCreatePlayer, PacketHandler.C_CreatePlayerHandler);		
		_onRecv.Add((ushort)MsgId.CEquipItem, MakePacket<C_EquipItem>);
		_handler.Add((ushort)MsgId.CEquipItem, PacketHandler.C_EquipItemHandler);		
		_onRecv.Add((ushort)MsgId.CPong, MakePacket<C_Pong>);
		_handler.Add((ushort)MsgId.CPong, PacketHandler.C_PongHandler);		
		_onRecv.Add((ushort)MsgId.CLogout, MakePacket<C_Logout>);
		_handler.Add((ushort)MsgId.CLogout, PacketHandler.C_LogoutHandler);		
		_onRecv.Add((ushort)MsgId.CChat, MakePacket<C_Chat>);
		_handler.Add((ushort)MsgId.CChat, PacketHandler.C_ChatHandler);		
		_onRecv.Add((ushort)MsgId.CUseItem, MakePacket<C_UseItem>);
		_handler.Add((ushort)MsgId.CUseItem, PacketHandler.C_UseItemHandler);		
		_onRecv.Add((ushort)MsgId.CKeySetting, MakePacket<C_KeySetting>);
		_handler.Add((ushort)MsgId.CKeySetting, PacketHandler.C_KeySettingHandler);		
		_onRecv.Add((ushort)MsgId.CDropItem, MakePacket<C_DropItem>);
		_handler.Add((ushort)MsgId.CDropItem, PacketHandler.C_DropItemHandler);		
		_onRecv.Add((ushort)MsgId.CNpc, MakePacket<C_Npc>);
		_handler.Add((ushort)MsgId.CNpc, PacketHandler.C_NpcHandler);		
		_onRecv.Add((ushort)MsgId.CPurchase, MakePacket<C_Purchase>);
		_handler.Add((ushort)MsgId.CPurchase, PacketHandler.C_PurchaseHandler);		
		_onRecv.Add((ushort)MsgId.CSell, MakePacket<C_Sell>);
		_handler.Add((ushort)MsgId.CSell, PacketHandler.C_SellHandler);		
		_onRecv.Add((ushort)MsgId.CStatUp, MakePacket<C_StatUp>);
		_handler.Add((ushort)MsgId.CStatUp, PacketHandler.C_StatUpHandler);		
		_onRecv.Add((ushort)MsgId.CShortKey, MakePacket<C_ShortKey>);
		_handler.Add((ushort)MsgId.CShortKey, PacketHandler.C_ShortKeyHandler);		
		_onRecv.Add((ushort)MsgId.CSlotChange, MakePacket<C_SlotChange>);
		_handler.Add((ushort)MsgId.CSlotChange, PacketHandler.C_SlotChangeHandler);		
		_onRecv.Add((ushort)MsgId.CUsers, MakePacket<C_Users>);
		_handler.Add((ushort)MsgId.CUsers, PacketHandler.C_UsersHandler);
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