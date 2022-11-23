using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Data;
using Server.DB;
using SharedDB;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public PlayerServerState ServerState { get; private set; }
		= PlayerServerState.ServerStateLogin; // Default 는 로그인상태


		public Player MyPlayer { get; set; }
		public int SessionId { get; set; }

		object _lock = new object();
		List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();


		// 패킷 모아 보내기

		int _reservedSendBytes = 0;
		long _lastSendTick = 0;

		long _pingpongTick = 0;
		public void Ping()
        {
			// 끊겨진 상태라면 그냥 둔다
			if (_disconnected == 1)
				return;

			if (_pingpongTick > 0)
            {
				long delta = (System.Environment.TickCount64 - _pingpongTick);

				Console.WriteLine($"User : {SessionId} delta : {delta}");
				if (delta > 60* 1000) // 50초
                {
					SaveAndDisconnect();
					return;
                }
			}

			S_Ping pingPacket = new S_Ping();
			Send(pingPacket);

			GameLogic.Instance.PushAfter(5000, Ping);  // 5초마다 한번씩 핑 체크
			Console.WriteLine("Ping~");
		}

		public void HandlePong()
        {
			_pingpongTick = System.Environment.TickCount64;
        }

		public void SaveAndDisconnect()
        {
			// 캐릭터 생성전이라면 넘어간다.
			if (MyPlayer != null)
			{

				// 팅겼을때만 작동한다.
				DbTransaction.SavePlayerStatus_AllInOne(MyPlayer, MyPlayer.Room);
				DbTransaction.SavePlayerPosition_AllInOne(MyPlayer, MyPlayer.Room);
			}

			// 팅겼을 때 로그인도 없애준다. # 20211031 수정

			TokenDb tokenDb = new TokenDb();
			tokenDb.TokenDbId = TokenDbId;
			tokenDb.IsLogin = false;


			// 로그아웃되니까 로그인 정보 false로 바꿔준다.  # 20211031 수정
			using (SharedDbContext shared = new SharedDbContext())
			{

				shared.Entry(tokenDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
				shared.Entry(tokenDb).Property(nameof(tokenDb.IsLogin)).IsModified = true; // "Hp"

				//db.SaveChanges();
				bool success = shared.SaveChangesEx(); // 예외 처리

				if (success)
				{
					Console.WriteLine("IsLogin = false =  Success");

					// 항상작동한다
					Console.WriteLine("Disconnected by PingCheck");
					Disconnect();
				}

			}



		}



        #region Network

		// 예약만하고 보내지는 않음.
        public void Send (IMessage packet)
        {

			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);


			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			lock (_lock)
			{
				_reserveQueue.Add(sendBuffer);
				_reservedSendBytes += sendBuffer.Length;
			}
			//Send(new ArraySegment<byte>(sendBuffer));
		}

		// 예약한걸 실행해주되, 예약껄 갖고오고, 걔네는 초기화.
		// 실제 Network IO 보내주는 부분임.
		public void FlushSend()
        {
			List<ArraySegment<byte>> sendList = null;

			lock(_lock)
            {
				// 한텀 기다리고 모아보내기 20221031 추가
				// 0.1초가 지났거나, 너무 패킷이 많이 모일 때 ( 1만 바이트)
				long delta = (System.Environment.TickCount64 - _lastSendTick);
                
				if (delta < 1500 && _reservedSendBytes < 10000)
                    return;

                // 패킷 모아 보내기

                _reservedSendBytes = 0;
				_lastSendTick = System.Environment.TickCount64;

				// 값이 없으면 리턴
				//if (_reserveQueue.Count == 0)
				//	return;

				sendList = _reserveQueue;
				_reserveQueue = new List<ArraySegment<byte>>();
            }
			Send(sendList);
        }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");


			// 서버에서 연결이 되었다고 패킷을 보냄
            {
				S_Connected connectedPacket = new S_Connected();
				Send(connectedPacket);
            }


			// 핑을 0초 있다가 실행시켜주세요.
			// 들어오자마자 바로 나가는 경우 때문임
			GameLogic.Instance.PushAfter(0, Ping);


	


			// client Session_Pregame.cs 로 이동됨
			// HadnleEnterGame


			// PROTO Test

			//S_Chat chat = new S_Chat()
			//{
			//	Context = "단진형 천재"
			//};

			//Send(chat);

		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{



			GameLogic.Instance.Push(() =>
			{
				// 크래쉬 뜨는거 방지
				if (MyPlayer == null)
					return;

				if (MyPlayer.Room == null)
					return;

				//RoomManager.Instance.Find(1).LeaveGame(MyPlayer.Info.ObjectId);
				GameRoom room = GameLogic.Instance.Find(MyPlayer.Room.RoomId);


				room.Push(room.LeaveGame, MyPlayer.Info.ObjectId); // => JobQueue화
			});

            // 나갈떄도 로그인 처리해주기.
            SaveAndDisconnect();

            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
        #endregion
    }
}
