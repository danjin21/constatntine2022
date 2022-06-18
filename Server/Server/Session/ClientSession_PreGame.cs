using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
		public int AccountDbId { get; private set; }
		public string AccountName { get; set; }
		public int TokenDbId { get; set; }

		public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

        public void HandleLogin (C_Login loginPacket)
        {
			//Console.WriteLine($"UniqueId({loginPacket.UniqueId})");

			// TODO : 이런 저런 보안 체크 
			// 일단 로그인 상태가 아니면 이상하다 생각하고 리턴을 한다.
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;


			// 한번더 비밀번호 대조 했는데 틀리면 리턴하기.
			using (SharedDbContext shared = new SharedDbContext())
			{
				TokenDb tokenDb = shared.Tokens.Where(t => t.AccountName == loginPacket.UniqueId).FirstOrDefault();

				// 이미 접속된 유저 아닌지 확인
				if (tokenDb != null && tokenDb.IsLogin == true)
				{
					// 접속 바로 끊어주기
					Disconnect();
					// S_Message 같은 패킷으로 이미 로그인 중이라고 말해주기
					return;
				}

				if (tokenDb.Password != loginPacket.Password)
					return;
				else
					TokenDbId = tokenDb.TokenDbId;

			}



			// TODO : 문제가 있긴 있다.
			// - 동시에 다른 사람이 같은 이름(UniqueId)을 보낸다면 ? ( 해커가 같은 유니크 아이디를 동시에 )
			// - 악의적으로 여러번 보낸다면? (한번에 수천번) ( 원래 여기서 퀘스트나 업적같은것도 한번에 불러와서 버거움)
			// - 쌩뚱맞은 타이밍에 그냥 이 패킷을 보낸다면? ( 사냥중인데 갑자기 보낸다면? )

			LobbyPlayers.Clear();

			using (AppDbContext db = new AppDbContext())
			{
				// UniqueID는 기기용인데, 나주엥는 그냥 ID값으로 찾으면 될듯
				// FirstOrDefault는 처음 입력한것인지 찾는거임.
				AccountDb findAccount = db.Accounts
					.Include(a=>a.Players)
					.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();


				if (findAccount != null)
				{

					// AccountDbId 메모리에 기억
					AccountDbId = findAccount.AccountDbId;
					AccountName = findAccount.AccountName;

					// 있는 아이디면 로그인 시켜준다.
					S_Login loginOk = new S_Login() { LoginOk = 1 };

					// 디비에 접속해서 플레이어들의 정보를 받아온다.
					foreach(PlayerDb playerDb in findAccount.Players)
                    {

						LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
						{
							PlayerDbId = playerDb.PlayerDbId,
							Name = playerDb.PlayerName,
							StatInfo = new StatInfo()
							{
								Level = playerDb.Level,
								Hp = playerDb.Hp,
								MaxHp = playerDb.MaxHp,
								Attack = playerDb.Attack,
								Speed = playerDb.Speed,
								TotalExp = playerDb.TotalExp,
								Map = playerDb.Map,
								PosX = playerDb.PosX,
								PosY = playerDb.PosY,
								Face = playerDb.Face,
								Hair = playerDb.Hair,
								Skin = playerDb.Skin,
								Gender = playerDb.Gender,
								Mp = playerDb.Mp,
								MaxMp = playerDb.MaxMp,
								Exp = playerDb.Exp,
								Gold = playerDb.Gold,
								Def = playerDb.Def,
								Str = playerDb.Str,
								Dex = playerDb.Dex,
								Int = playerDb.Int,
								Luk = playerDb.Luk,
								Helmet = -1,
								RightHand = -1,
								LeftHand = -1,
								Shirts = -1,
								Pants = -1,
								Shoes = -1,
								StatPoint = playerDb.StatPoint,
								HairColor = playerDb.HairColor

                            }
						};

						// 메모리에도 들고 있는다. ( 이유 : DB 접근하는 빈도를 최소화하기위해 )
						LobbyPlayers.Add(lobbyPlayer);

						// 패킷에 넣어준다.
						loginOk.Players.Add(lobbyPlayer);

                    }
					
					// 너는 로그인 되었으니까, 로비로 넘어가세요~
					Send(loginOk);

					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
				else
				{
					// 없는 아이디면 새로 만들어준다.
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					//db.SaveChanges(); // TODO : Exception ( DB 저장이 안될경우)
					bool success = db.SaveChangesEx();

					if (success == false) // 제대로 안되면 return 하거나 클라한테 말해준다.
						return;

					// AccountDbId 메모리에 기억
					AccountDbId = newAccount.AccountDbId;


					// 새로 만들어주고 로그인 시켜준다.
					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);


					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;


				}
			}
		}

		// 캐릭터 하나 골라서 입장하겠다는거임
		public void HandleEnterGame(C_EnterGame enterGamePacket)
		{

			// ☆★ 로비가 아닌데 들어가려고 했다면 리턴한다,.
			// ☆★ 혹시 어떤 게임에서 다른 게임으로 넘어간다고 한다면, 그건 처리해줘야함
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			// 로비에 플레이어있는지 확인
			LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
			if (playerInfo == null)
				return;

			// TODO : 로비에서 캐릭터 선택했을 때 해주는거임
			// playerManager에 플레이어를 추가하는 동시에 MyPlayer에 넣는다.
			MyPlayer = ObjectManager.Instance.Add<Player>();
			{
				// 플레이어의 아이디를 추가해줌 : 이제 DB에서도 내가 어떤 플레이어 ID를 갖고있는지 알수있음.
				MyPlayer.PlayerDbId = playerInfo.PlayerDbId;

				MyPlayer.Info.Name = playerInfo.Name; //MyPlayer.Info.Name = $"플레이어_{MyPlayer.Info.ObjectId}";
				MyPlayer.Info.PosInfo.State = CreatureState.Idle;
				MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;

				MyPlayer.Info.PosInfo.PosX = playerInfo.StatInfo.PosX;
				MyPlayer.Info.PosInfo.PosY = playerInfo.StatInfo.PosY;
				// 캐릭터의 정보를 stat에 머지한다? 데이터 시트를 통해 설정이 된다.
				//StatInfo stat = null;
				//DataManager.StatDict.TryGetValue(1, out stat);
				//MyPlayer.Stat.MergeFrom(stat);

				MyPlayer.Stat.MergeFrom(playerInfo.StatInfo);

				MyPlayer.Session = this;

				// 아이템 리스트 패킷을 생성한다.
				S_ItemList itemListPacket = new S_ItemList();

				// 아이템 목록을 갖고 온다.
				using (AppDbContext db = new AppDbContext())
				{
					List<ItemDb> items = db.Items
						.Where(i => i.OwnerDbId == playerInfo.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
						.ToList();

					foreach (ItemDb itemDb in items)
					{
						// 아이템 디비정보를 이용해서 아이템을 만들어준다.
						Item item = Item.MakeItem(itemDb);

						// 아무 문제가 없으면
						if (item != null)
						{
							// 이 정보를 인벤토리에 넣어주고.
							MyPlayer.Inven.Add(item);

							// 패킷에 해당 아이템 정보를 넣어준다.
							ItemInfo info = new ItemInfo();

							info.MergeFrom(item.Info);

							itemListPacket.Items.Add(info);


							// 장비 착용에 대한 정보는 서버에만 일단 기록하고 나중에 캐릭터가 종료하면 DB에 저장.

							if (item.Equipped == true) // 아이템 착용했을 경우
							{
								switch (item.ItemType)
								{
									case ItemType.Weapon:

										MyPlayer.Stat.RightHand = item.TemplateId;
										break;
									case ItemType.Armor:

										if (((Armor)item).ArmorType == ArmorType.Helmet)
											MyPlayer.Stat.Helmet = item.TemplateId;
										else if (((Armor)item).ArmorType == ArmorType.Armor)
											MyPlayer.Stat.Shirts = item.TemplateId;
										else if (((Armor)item).ArmorType == ArmorType.Boots)
											MyPlayer.Stat.Shoes = item.TemplateId;
										else if (((Armor)item).ArmorType == ArmorType.Pants)
											MyPlayer.Stat.Pants = item.TemplateId;
										else if (((Armor)item).ArmorType == ArmorType.Shield)
											MyPlayer.Stat.LeftHand = item.TemplateId;
										break;
								}
							}
	


						}
					}

				}


				// 정상적으로 아이템 리스트를 보낸다.
				Send(itemListPacket);



				// 키셋팅 리스트 패킷을 생성한다.
				S_KeySettingList keyListPacket = new S_KeySettingList();

				// 키셋팅 목록을 갖고 온다.
				using (AppDbContext db = new AppDbContext())
				{
					List<KeySettingDb> keys = db.Keys
						.Where(i => i.OwnerDbId == playerInfo.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
						.ToList();

					foreach (KeySettingDb keyDb in keys)
					{
						// 키셋팅 디비정보를 이용해서 키들을 만들어준다.
						Key key = Key.MakeKey(keyDb);

						// 아무 문제가 없으면
						if (key != null)
						{
							// 이 정보를 인벤토리에 넣어주고.
							MyPlayer.Keys.Add(key);

							// 패킷에 해당 아이템 정보를 넣어준다.
							KeySettingInfo info = new KeySettingInfo();
							info.MergeFrom(key.Info);
							keyListPacket.KeySettingInfo.Add(info);
						}
					}
				}

				// 정상적으로 키셋팅 리스트를 보낸다.
				Send(keyListPacket);


				// 스킬 없으면 새로 만들어준다.

				using (AppDbContext db = new AppDbContext())
                {
					List<SkillDb> skills = db.Skills
						.Where(i => i.OwnerDbId == playerInfo.PlayerDbId)
						.ToList();

					// 스킬이 아무것도 없다면 기본 스킬을 추가해준다.
					if(skills.Count == 0)
                    {
						// 기본 공격 스킬 넣어주기.
						SkillDb newSkillDb_Attack = new SkillDb()
						{
							OwnerDbId = playerInfo.PlayerDbId,
							SkillTemplateId = 9001000,
							SkillLevel = 1,
							Slot = 0,
						};

						// 기본 줍기 스킬 넣어주기.
						SkillDb newSkillDb_Drop = new SkillDb()
						{
							OwnerDbId = playerInfo.PlayerDbId,
							SkillTemplateId = 9001001,
							SkillLevel = 1,
							Slot = 1,
						};

						db.Skills.Add(newSkillDb_Attack);
						db.Skills.Add(newSkillDb_Drop);

						db.SaveChangesEx();


						// 단축키 셋팅에도 넣어주기.
                        C_KeySetting keysettingPacket_9001000 = new C_KeySetting()
                        {
							Key = 10,
							Type = 2,
							Action = 9001000
						};

						C_KeySetting keysettingPacket_9001001 = new C_KeySetting()
						{
							Key = 11,
							Type = 2,
							Action = 9001001
						};

						MyPlayer.HandleKeySetting(keysettingPacket_9001000);
						MyPlayer.HandleKeySetting(keysettingPacket_9001001);
					}

					// 혹은 9001000 이랑 9001001 없으면 넣어주는걸로 대체해도 좋을듯?

				}


				// 스킬 리스트 패킷을 생성한다.
				S_SkillList skillListPacket = new S_SkillList();

				// 스킬 목록을 갖고 온다.
				using (AppDbContext db = new AppDbContext())
				{
					List<SkillDb> skills = db.Skills
						.Where(i => i.OwnerDbId == playerInfo.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
						.ToList();

					foreach (SkillDb skillDb in skills)
					{
						// 스킬을 생성하고,
						Skills skill = Skills.MakeSkill(skillDb);

						// 정보를 메모리에도 넣어주고,
						MyPlayer.SkillInven.Add(skill);

						// 해당 스킬 정보를 패킷정보로 만든후,
						SkillInfo skillInfo = new SkillInfo()
						{
							SkillId = skill.SkillId,
							SkillDbId = skill.SkillDbId,
							Slot = skill.Slot,
							SkillLevel = skill.SkillLevel,
						};

						// 패킷에 넣어준다.
						skillListPacket.SkillInfo.Add(skillInfo);
	
					}

				}

				// 정상적으로 스킬 리스트를 보낸다.
				Send(skillListPacket);



                // 퀘스트 정보를 보내준다.

                S_QuestList questListPacket = new S_QuestList();

                using (AppDbContext db = new AppDbContext())
                {
                    List<QuestDb> quests = db.Quests
                        .Where(i => i.OwnerDbId == playerInfo.PlayerDbId) // 메모리에 
                        .ToList();

          

					foreach( QuestDb questDb in quests)
                    {

						Console.WriteLine("quest1 : " + questDb.QuestDbTemplateId);

						// 퀘스트 디비정보를 이용해 퀘스트들을 만들어준다.
						Quest quest = Quest.MakeQuest(questDb);

						Console.WriteLine("quest2 : " + quest.QuestTemplateId);

						// 아무 문제가 없으면
						if (quest != null)
						{
							// 이 정보를 퀘스트에 넣어주고,
							MyPlayer.QuestInven.Add(quest);

							// 패킷에 해당 아이템 정보를 넣어준다.
							QuestInfo info = new QuestInfo();
							info.MergeFrom(quest.Info);
							questListPacket.QuestInfo.Add(info);
                        }
                    }
                }

				// 정상적으로 키셋팅 리스트를 보낸다.
                Send(questListPacket);



                // 친구 정보를 보내준다.

                // 파티 정보를 보내준다.

                // 길드 정보를 보내준다.



            }




			// 입장했으니까 State를 바꿔준다.
			ServerState = PlayerServerState.ServerStateGame;


			// 게임 진입!!!!!!

			// TODO : 입장 요청 들어올 때 ( 로비에서 필드로 들어올때 ) 해주는거임
			// 1번방에 MyPlayer를 넣는다.
			//RoomManager.Instance.Find(1).EnterGame(MyPlayer);



			





			GameLogic.Instance.Push(() =>
			{
			GameRoom room = GameLogic.Instance.Find(playerInfo.StatInfo.Map);
			room.Push(room.EnterGame, MyPlayer, false); // => JobQueue 화
			room.Push(room.IsLoginTrue, MyPlayer, false); // => JobQueue 화
			});


	




		}

		public void HandleCreatePlayer(C_CreatePlayer createPacket)
        {
			// TODO : 이런 저런 보안 체크 
			// 일단 로그인 상태가 아니면 이상하다 생각하고 리턴을 한다.
			// '로비'상태인지 확인
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			using (AppDbContext db = new AppDbContext())
            {
				PlayerDb findPlayer = db.Players
					.Where(p => p.PlayerName == createPacket.Name).FirstOrDefault();

				if(findPlayer != null)
                {
					// 이름이 겹친다. 텅 빈값으로 보낸다.
					Send(new S_CreatePlayer());
                }
				else
                {
					// 1레벨 스탯 정보 추출
					StatInfo stat = null;
					DataManager.StatDict.TryGetValue(1, out stat);

					// DB에 플레이어 만들어줘야 함
					PlayerDb newPlayerDb = new PlayerDb()
					{
						PlayerName = createPacket.Name,
						Level = stat.Level,
						Hp = stat.Hp,
						MaxHp = stat.MaxHp,
						Attack = stat.Attack,
						Speed = stat.Speed,
						TotalExp = stat.TotalExp,
						AccountDbId = AccountDbId,
						Map = stat.Map,
						PosX = stat.PosX,
						PosY = stat.PosY,
						Face = createPacket.Face,
						Hair = createPacket.Hair,
						Skin = createPacket.Skin,
						Gender = createPacket.Gender,
						Mp = stat.Mp,
						MaxMp = stat.MaxMp,
						Exp = stat.Exp,
						Gold = stat.Gold,
						Def = stat.Def,
						Str = stat.Str,
						Dex = stat.Dex,
						Int = stat.Int,
						Luk = stat.Luk,
						//Helmet = -1,
						//RightHand = -1,
						//LeftHand = -1,
						//Shirts = -1,
						//Pants = -1,
						//Shoes = -1,
						StatPoint = 0,

					};

					db.Players.Add(newPlayerDb);
					//db.SaveChanges();  // TODO : Exception Handling 찰나의 순간에 다른사람에게 먹힐수도 있어서



					bool success = db.SaveChangesEx();
					if (success == false) // 제대로 안되면 return 하거나 클라한테 말해준다.
						return;


					// 메모리에 추가
					LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
					{
						PlayerDbId = newPlayerDb.PlayerDbId,
						Name = createPacket.Name,
						StatInfo = new StatInfo()
						{
							Level = stat.Level,
							Hp = stat.Hp,
							MaxHp = stat.MaxHp,
							Attack = stat.Attack,
							Speed = stat.Speed,
							TotalExp = stat.TotalExp,
							Map = stat.Map,
							PosX = stat.PosX,
							PosY = stat.PosY,
							Face = createPacket.Face,
							Hair = createPacket.Hair,
							Skin = createPacket.Skin,
							Gender = createPacket.Gender,
							Mp = stat.Mp,
							MaxMp = stat.MaxMp,
							Exp = stat.Exp,
							Gold = stat.Gold,
							Def = stat.Def,
							Str = stat.Str,
							Dex = stat.Dex,
							Int = stat.Int,
							Luk = stat.Luk,
                            Helmet = -1,
                            RightHand = -1,
                            LeftHand = -1,
                            Shirts = -1,
                            Pants = -1,
                            Shoes = -1,
							StatPoint = 0,

						}
					};

					// 메모리에도 들고 있는다. ( 이유 : DB 접근하는 빈도를 최소화하기위해 )
					LobbyPlayers.Add(lobbyPlayer);

					// 클라이언트에 만들었따고 전송
					S_CreatePlayer newPlayer = new S_CreatePlayer() { Player = new LobbyPlayerInfo() };
					newPlayer.Player.MergeFrom(lobbyPlayer);

					Send(newPlayer);




				}
            }

		}


		public void HandleLogout(C_Logout logoutPacket)
        {




			TokenDb tokenDb = new TokenDb();
			tokenDb.TokenDbId = TokenDbId;
			tokenDb.IsLogin = false;


			// 로그아웃되니까 로그인 정보 false로 바꿔준다.
			using (SharedDbContext shared = new SharedDbContext())
			{

				shared.Entry(tokenDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
				shared.Entry(tokenDb).Property(nameof(tokenDb.IsLogin)).IsModified = true; // "Hp"


				//db.SaveChanges();
				bool success = shared.SaveChangesEx(); // 예외 처리

				if (success)
				{
					ServerState = PlayerServerState.ServerStateLogin;


					S_Logout slogoutPacket = new S_Logout();
					Send(slogoutPacket);

					SaveAndDisconnect();
				}


				//// 토큰 업데이트하거나 추가하는 부분
				//TokenDb tokenDb = shared.Tokens.Where(t => t.AccountName == AccountName).FirstOrDefault();

				//if (tokenDb != null)
				//{
				//	tokenDb.IsLogin = false;
				//}
				//shared.SaveChangesEx();
			}

		}


	}
}
