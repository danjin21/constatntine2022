using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;
using SharedDB;

namespace Server
{

	//1. GameRoom 방식의 간단한 동기화 <= OK
	//2. 더 넓은 영역 관리( 1000 명 2000명 )
	//3. 심리스 MMO

	// 1. Recv (N개)                                                          서빙
	// 2. GameRoomManager => TickRoom (1) => GameLogic 으로 하나로 만든다.    요리사
	// 2-1. Send(1개)														  요리 서빙
	// 3. Dbtransaction DB (1)                                                결제/장부

	class Program
	{
		static Listener _listener = new Listener();

		//// 타이머들 리스트 ( 중간에 멈출수 있도록 )
		//static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>(); 

		//// C# 에서 제공해주는 주기적인 함수호출
		//static void TickRoom(GameRoom room, int tick = 100)
		//{
		//	var timer = new System.Timers.Timer();
		//	timer.Interval = tick; // 인터발이 끝나면
		//	timer.Elapsed += ((s,e) => { room.Update(); });  // 실행해주겠따.

		//	// 초기화
		//	timer.AutoReset = true;
		//	timer.Enabled = true;

		//	_timers.Add(timer);
		//	//timer.Stop(); // 멈추고 싶을 때 써라
		// }

		//static void FlushRoom()
		//{
		//	JobTimer.Instance.Push(FlushRoom, 250);
		//}


		static void GameLogicTask()
        {
			while(true)
            {
				GameLogic.Instance.Update();
				Thread.Sleep(0);
            }
        }

		static void DbTask()
        {
			// TODO
			while (true)
			{
				//JobTimer.Instance.Flush();

				// 1번방을 계속 굴린다.
				//RoomManager.Instance.Find(1).Update();


				// 상단의 TickRoom 으로 대체되었습니다.
				//GameRoom room = RoomManager.Instance.Find(1);
				//room.Push(room.Update); // => JobQueue 방식

				// 프로그램 꺼지지않도록 그냥 유지시켜주는 거
				//Thread.Sleep(100);

				// 계속해서 데이터 일감을 받고 처리한다.
				DbTransaction.Instance.Flush();
				Thread.Sleep(0);
			}
		}

		static void NetworkTask()
        {
			while(true)
            {

				List<ClientSession> sessions = SessionManager.Instance.GetSessions();
				
				foreach(ClientSession session in sessions)
                {
					session.FlushSend();
                }

				Thread.Sleep(100);
            }
        }

// 로그인 정보 초기화
		static void ClearLoginInfo()
        {

			using (SharedDbContext shared = new SharedDbContext())
			{


				// 토큰 업데이트하거나 추가하는 부분
				List<TokenDb> tokenDbs = shared.Tokens.Where(t => t.IsLogin == true).ToList();

				foreach(TokenDb tokenDb in tokenDbs)
                {
                    tokenDb.IsLogin = false;
                    shared.SaveChangesEx();
                }


			}
		}


		// 주기적으로 서버의 내용이 저장된다.
		static void StartServerInfoTask()
        {
			var t = new System.Timers.Timer();
			t.AutoReset = true;
			t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
			{
				//TODO
				using (SharedDbContext shared = new SharedDbContext())
                {
					ServerDb serverDb = shared.Servers.Where(s => s.Name == Name).FirstOrDefault();

					if(serverDb != null)
                    {
						serverDb.Name = Name;
						serverDb.IpAddress = IpAddress;
						serverDb.Port = Port;
						serverDb.BusyScore = SessionManager.Instance.GetBusyScore();
						shared.SaveChangesEx();
					}
					else // 서버가 없으면 만들어준다.,
                    {
                        serverDb = new ServerDb()
                        { 
							Name = Program.Name,
							IpAddress = Program.IpAddress,
							Port = Program.Port,
							BusyScore = SessionManager.Instance.GetBusyScore()
						};

						shared.Servers.Add(serverDb);
						shared.SaveChangesEx();
                    }
                }
			});

			t.Interval = 10 * 1000;
			t.Start();
        }

		// 서버의 데이터를 넣는 부분임
		public static string Name { get; } = "인트";
		public static int Port { get; } = 7777;
		public static string IpAddress { get; set; }

		static void Main(string[] args)
		{

			//using(SharedDbContext shared = new SharedDbContext())
   //         {

   //         }

			// 설정파일을 읽는다.
			ConfigManager.LoadConfig();
			DataManager.LoadData();


			// 아이템 넣어주기
			// ItemTEST();



			//// DB Test 불러올대마다 쓴다.
			//using (AppDbContext db = new AppDbContext())
			//{
			//	db.Accounts.Add(new AccountDb() { AccountName = "TestAccount"});
			//	db.SaveChanges();
			//}


			// 맵을 생성
			GameLogic.Instance.Push(() =>
			{
				GameLogic.Instance.Add(1);
				GameLogic.Instance.Add(2);
				GameLogic.Instance.Add(3);
				GameLogic.Instance.Add(4);

				GameLogic.Instance.Add(10100000);
				GameLogic.Instance.Add(10100010);
				GameLogic.Instance.Add(10100020);
				GameLogic.Instance.Add(10100030);
				GameLogic.Instance.Add(10100031);
				GameLogic.Instance.Add(10100032);
				GameLogic.Instance.Add(10100040);
				GameLogic.Instance.Add(10100041);
				GameLogic.Instance.Add(10100042);
				GameLogic.Instance.Add(10100043);
				GameLogic.Instance.Add(10100044);
				GameLogic.Instance.Add(10100050);
				GameLogic.Instance.Add(10100051);
				GameLogic.Instance.Add(10100060);
				GameLogic.Instance.Add(10100061);
				GameLogic.Instance.Add(10100070);

				GameLogic.Instance.Add(20100000);
				GameLogic.Instance.Add(20100001);
				GameLogic.Instance.Add(20100002);
				GameLogic.Instance.Add(20100003);
				GameLogic.Instance.Add(20100004);

				GameLogic.Instance.Add(20100005);
				GameLogic.Instance.Add(20100006);
				GameLogic.Instance.Add(20100007);


			});


			////var d = DataManager.StatDict;

			//// 방생성 ( 1번 - 유니티에서 맵과 같음)
			//GameRoom room = GameLogic.Instance.Add(1);
			////TickRoom(room, 50); // 게임에게 TickRoom 을 실시한다.

			//// 방생성 ( 2번 - 유니티에서 맵과 같음)
			//GameRoom room2 = GameLogic.Instance.Add(2);
			////TickRoom(room2, 50); // 게임에게 TickRoom 을 실시한다.




			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			//IPAddress ipAddr = ipHost.AddressList[1];
			IPAddress ipAddr = ipHost.AddressList[1];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, /*7777*/Port);

            IpAddress = ipAddr.ToString();



            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening.../"+IpAddress);
		
			// 주기적으로 서버 정보를 보내준다.
			StartServerInfoTask();

			// 처음 그냥 로그인 정보 초기화
			ClearLoginInfo();


			// DbTask
			{
				Thread t = new Thread(DbTask);
				t.Name = "DB";
				t.Start();
            }

			// NetworkTask
			{
				Thread t = new Thread(NetworkTask);
				t.Name = "Network Send";
				t.Start();
			}


			//GameLogic
			Thread.CurrentThread.Name = "GameLogic";
			GameLogicTask();


			//// GameLogicTask
			//{
			//	// 직원채용
			//	Task gameLogicTask = new Task(GameLogicTask, TaskCreationOptions.LongRunning);
			//	// 일시키기
			//	gameLogicTask.Start();
   //         }


			//// NetworkTask ( 브로드 캐스트)
			//{
			//	// 직원채용
			//	Task networkTask = new Task(NetworkTask, TaskCreationOptions.LongRunning);
			//	// 일시키기
			//	networkTask.Start();
			//}





			//FlushRoom();
			//JobTimer.Instance.Push(FlushRoom);


		}



		//public void ItemTEST()
  //      {
		//	// TEST CODE
		//	using (AppDbContext db = new AppDbContext())
		//	{
		//		PlayerDb player = db.Players.FirstOrDefault(); // First로 하고나서, 플레이어 정보가 없으면 Exception이 일어남
		//		if (player != null)
		//		{
		//			db.Items.Add(new ItemDb()
		//			{
		//				TemplateId = 1,
		//				Count = 1,
		//				Slot = 0,
		//				Owner = player
		//			});
		//			db.Items.Add(new ItemDb()
		//			{
		//				TemplateId = 100,
		//				Count = 1,
		//				Slot = 1,
		//				Owner = player
		//			});
		//			db.Items.Add(new ItemDb()
		//			{
		//				TemplateId = 101,
		//				Count = 1,
		//				Slot = 2,
		//				Owner = player
		//			});
		//			db.Items.Add(new ItemDb()
		//			{
		//				TemplateId = 200,
		//				Count = 10,
		//				Slot = 5,
		//				Owner = player
		//			});

		//			db.SaveChangesEx();
		//		}
		//	}
		//}




	}
}
