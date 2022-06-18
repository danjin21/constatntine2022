using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace ServerCore
{
	public class Connector
	{
		Func<Session> _sessionFactory;

		public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				// 휴대폰 설정
				Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_sessionFactory = sessionFactory;

				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += OnConnectCompleted;
				args.RemoteEndPoint = endPoint;
				args.UserToken = socket;

				RegisterConnect(args);
			}
		}

		void RegisterConnect(SocketAsyncEventArgs args)
		{
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return;

			bool pending = socket.ConnectAsync(args);
			if (pending == false)
				OnConnectCompleted(null, args);
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();

                //// 진형쓰룰 : 연결되었으니 session 의 _disconnected = 1 되어있는걸 0 으로 해준다. 그래야 재통신이 가능
                //session._disconnected = 0;

                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);

            }
			else
			{
				Debug.Log($"OnConnectCompleted Fail: {args.SocketError}");
			}
		}
	}
}
