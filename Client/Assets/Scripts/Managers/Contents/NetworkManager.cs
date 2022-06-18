using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
    public int AccountId { get; set; }
    public int Token { get; set; }
    public string AccountName { get; set; }
    public string Password { get; set; }


    public ServerSession _session = new ServerSession();

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void ConnectToGame(ServerInfo info)
	{
        //// DNS (Domain Name System)
        //string host = Dns.GetHostName();
        //IPHostEntry ipHost = Dns.GetHostEntry(host);
        //IPAddress ipAddr = ipHost.AddressList[1];

        IPAddress ipAddr = IPAddress.Parse(info.IpAddress);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);

        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("120.50.85.61"), 7777);
        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);
        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("182.195.210.197"), 7777);
        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.46"), 7777);

        Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}	
	}

    public void DisconnectFromGame()
    {
        _session.Disconnect();
    }

}
