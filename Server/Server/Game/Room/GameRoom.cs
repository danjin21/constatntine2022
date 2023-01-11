using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer   // 하나의 스레드에서 일감을 실행한다.
    {

        // 비젼 셀의 크기
        public const int VisionCells = 14; // 반지름임 1280 에서는 여유있게 20으로 함

        public int RoomId { get; set; }

        Dictionary<int,Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        Dictionary<int, DropItem> _dropitems = new Dictionary<int, DropItem>();
        Dictionary<int, Npc> _npcs = new Dictionary<int, Npc>();

        public Zone[,] Zones { get; private set; }
        public int ZoneCells { get; private set; }

        public Map Map { get; private set; } = new Map();

        public int TotalMonster;
        public int MapId;
        public int spawnTime;

        // □□□   (0,0) (0,1) (0,2)
        // □□□   (-1,0) (-1,1) (-1,2)
        // □□□

        public Zone GetZone(Vector2Int cellPos)
        {
            int x = (cellPos.x - Map.MinX) / ZoneCells;
            int y = (Map.MaxY - cellPos.y) / ZoneCells;

            return GetZone(y, x);
        }

        public Zone GetZone(int indexY, int indexX)
        {
            if (indexX < 0 || indexX >= Zones.GetLength(1))
                return null;

            if (indexY < 0 || indexY >= Zones.GetLength(0))
                return null;

            return Zones[indexY, indexX];

        }


        public void Init(int mapId, int zoneCells)
        {
            MapId = mapId;
            Map.LoadMap(mapId);

            // Zone

            ZoneCells = zoneCells; //10
            // 1~10 칸 = 1존
            // ( 10~ 19) / 10 = 1~1.9 = > 1
            // 11~20칸 = 2존
            // 21~30칸 = 3존

            int countY = (Map.SizeY+zoneCells-1) / zoneCells;
            int countX = (Map.SizeX+zoneCells-1) / zoneCells;
            Zones = new Zone[countY, countX];
            for(int y= 0; y<countY; y++)
            {
                for(int x = 0; x <countX; x++)
                {
                    Zones[y, x] = new Zone(y, x);
                }
            }




            // NPC 데이터에서 해당 맵에 있는 NPC들을 소환한다.

            foreach (NpcData t in DataManager.NpcDict.Values)
            {
                if (t.map == RoomId)
                {
                    Npc npcs = ObjectManager.Instance.Add<Npc>();
                    npcs.Init(t.npcId);
                    Push(EnterGame, npcs, false);
                }
            }


            //TotalMonster = 15;
            SummonMonsters(MapId);





            //    // TEMP 몬스터 생성
            //    Monster monster = ObjectManager.Instance.Add<Monster>();

            //    monster.Init(1); // 종류를 정해줌.

            //    // 몬스터 위치
            //    //monster.CellPos = new Vector2Int(5, 5);

            //    //EnterGame(monster);


            //    Push(EnterGame, monster, true); // => JobQueue 화 시켜줌.



            //TestTimer();

        }


        //// TEST
        //void TestTimer()
        //{
        //    Console.WriteLine("TestTImer");
        //    PushAfter(100, TestTimer);
        //}


        // MMO  ( 50ms )
        // LOL (0.1초)
        // FPS 

        // 누군가가 주기적으로 호출해줘야 한다.
        public void Update()
        {
            //lock(_lock)
            //{
                //foreach(Monster monster in _monsters.Values)
                //{
                //    monster.Update();
                //}

                //foreach(Projectile projectile in _projectiles.Values)
                //{
                //    projectile.Update();
                //}

                Flush();
            //}
        }

        Random _rand = new Random();
        public void EnterGame(GameObject gameObject, bool randomPos)
        {
            if (gameObject == null)
                return;

            if (randomPos)
            {
                //소환시 랜덤으로 위치생성
                Vector2Int respawnPos;
                while (true) // 꽉차면 문제가생김.
                {
                    respawnPos.x = _rand.Next(Map.MinX, Map.MaxX);
                    respawnPos.y = _rand.Next(Map.MinY, Map.MaxY);

                    // 갈수 있는 곳이거나,포탈이 아니여야 된다.
                    if (Map.Find(respawnPos) == null && Map.CanGo(respawnPos) == true && Map.IsPortal(respawnPos) == null)
                    {
                        gameObject.CellPos = respawnPos;
                        break;
                    }
                }
            }

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
            //Console.WriteLine("Type= " +type + "/" + gameObject.Info.Name );
            //lock (_lock)
            //{

            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;

                _players.Add(gameObject.Id, player);
                player.Room = this;


                // 플레이어의 스탯 갱신
                player.RefreshAdditionalStat();


                //// ★☆ 충돌체의 정보를 주는 곳  ★☆
                //Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));


                // 진형쓰룰
                int countX = 0;
                int countY = 0;
                // ★☆ 충돌체의 정보를 주는 곳  ★☆
                while (Map.ApplyMove(player, new Vector2Int(player.CellPos.x + countX, player.CellPos.y + countY)) == false)
                {

                    countX++;

                    // 진형쓰룰
                    if (player.CellPos.x + countX < Map.MinX || player.CellPos.x + countX >= Map.MaxX)
                    {
                        countX = 0;
                        countY++;
                    }


                    if (player.CellPos.y + countY < Map.MinY || player.CellPos.y + countY >= Map.MaxY)
                        break;
                }

                // 존에다가 플레이어 삽입.
                GetZone(player.CellPos).Players.Add(player);


                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);


                    // 아래 코드를 대체함. (Update)
                    player.Vision.Update();

                    //S_Spawn spawnPacket = new S_Spawn();
                    //foreach (Player p in _players.Values)
                    //{
                    //    if (player != p)
                    //        spawnPacket.Objects.Add(p.Info);
                    //}

                    //// 죽고나서 다시 태어날때 몬스터 정보 갱신
                    //foreach(Monster m in _monsters.Values)
                    //{
                    //    spawnPacket.Objects.Add(m.Info);
                    //}

                    //// 죽고나서 다시 태어날때 투사체 정보 갱신
                    //foreach (Projectile p in _projectiles.Values)
                    //{
                    //    spawnPacket.Objects.Add(p.Info);
                    //}

                    //player.Session.Send(spawnPacket);

                }

                // 플레이어의 Update를 실행
                player.Update();



            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;

                // 이미지를 넣기위해 templateId는 넣어주자.
                gameObject.Stat.TemplateId = monster.TemplateId;


                // 존에다가 몬스터 삽입.
                GetZone(monster.CellPos).Monsters.Add(monster);


                //// ★☆ 충돌체의 정보를 주는 곳  ★☆
                //Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));


                // 진형쓰룰
                int countX = 0;
                int countY = 0;

                // ★☆ 충돌체의 정보를 주는 곳  ★☆
                while (Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x + countX, monster.CellPos.y + countY)) == false)
                {

                    countX++;

                    // 진형쓰룰
                    if (monster.CellPos.x + countX < Map.MinX || monster.CellPos.x + countX >= Map.MaxX)
                    {
                        countX = 0;
                        countY++;
                    }


                    if (monster.CellPos.y + countY < Map.MinY || monster.CellPos.y + countY >= Map.MaxY)
                        break;
                }

                // 몬스터의 Update를 실행
                monster.Update();
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;

                // 존에다가 투사체 삽입.
                GetZone(projectile.CellPos).Projectiles.Add(projectile);


                //최초 Arromw Update실행
                //Push(projectile.Update);
                // GameRoom의 잡큐에서 실행되기때문에 그냥 함수 실행해도 된다..


                projectile.Update();
            }
            else if (type == GameObjectType.DropItem)
            {
                DropItem dropItem = gameObject as DropItem;
                _dropitems.Add(gameObject.Id, dropItem);
                dropItem.Room = this;

                // 이미지를 넣기위해 templateId는 넣어주자.
                gameObject.Stat.TemplateId = dropItem.RewardData.itemId;


                // 존에다가 드롭아이템 삽입.
                GetZone(dropItem.CellPos).DropItems.Add(dropItem);


                //// ★☆ 충돌체의 정보를 주는 곳  ★☆
                //Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));


                // 진형쓰룰
                int countX = 0;
                int countY = 0;

                // ★☆ 충돌체의 정보를 주는 곳  ★☆
                // 위에 올라갈 수 있게 해주고, 그곳에 뭐가 있어도 그냥 올려준다.
                Map.ApplyMove(dropItem, new Vector2Int(dropItem.CellPos.x + countX, dropItem.CellPos.y + countY), false, false);



            }
            else if (type == GameObjectType.Npc)
            {
                Npc npc = gameObject as Npc;
                _npcs.Add(gameObject.Id, npc);
                npc.Room = this;

                // 이미지를 넣기위해 templateId는 넣어주자.
                gameObject.Stat.TemplateId = npc.TemplateId;

                npc.CellPos = new Vector2Int(npc.Stat.PosX, npc.Stat.PosY);

                // 존에다가 Npc 삽입.
                GetZone(npc.CellPos).Npcs.Add(npc);


                //// ★☆ 충돌체의 정보를 주는 곳  ★☆
                //Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));


                // 진형쓰룰
                int countX = 0;
                int countY = 0;

                // ★☆ 충돌체의 정보를 주는 곳  ★☆
                while (Map.ApplyMove(npc, new Vector2Int(npc.CellPos.x + countX, npc.CellPos.y + countY)) == false)
                {

                    countX++;

                    // 진형쓰룰
                    if (npc.CellPos.x + countX < Map.MinX || npc.CellPos.x + countX >= Map.MaxX)
                    {
                        countX = 0;
                        countY++;
                    }


                    if (npc.CellPos.y + countY < Map.MinY || npc.CellPos.y + countY >= Map.MaxY)
                        break;
                }


            }


            // 타인한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                Broadcast(gameObject.CellPos, spawnPacket);
            }


            //// 타인한테 정보 전송
            //{
            //    S_Spawn spawnPacket = new S_Spawn();
            //    spawnPacket.Objects.Add(gameObject.Info);

            //    //플레이어의 경우 자신 정보는본인한테 정보 전송으로 보내기 때문에 예외를 시킴.
            //    foreach(Player p in _players.Values)
            //    {
            //        if (p.Id != gameObject.Id)
            //            p.Session.Send(spawnPacket);
            //    }
            //}

            //}
        }

        public void LeaveGame(int objectId)
        {

            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);


            Vector2Int cellPos;

            //lock (_lock)
            //{
            if (type == GameObjectType.Player)
            {
                Player player = null;

                // 먼저 없애고, false 라면 원래부터 없는 애니까 return
                if (_players.Remove(objectId, out player) == false)
                    return;

                cellPos = player.CellPos;

                //// 존에다가 플레이어 삭제
                //GetZone(player.CellPos).Players.Remove(player);

                // 플레이어가 나갈때 OnLeaveGame() 을 통해 DB 갱신을 해준다.
                player.OnLeaveGame();

                Map.ApplyLeave(player); // 맵에서도 충돌체로서 없앤다.
                player.Room = null; // 순서 조정

                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;

                cellPos = monster.CellPos;

                //// 존에다가 플레이어 삭제
                //GetZone(monster.CellPos).Monsters.Remove(monster);


                Map.ApplyLeave(monster); // 맵에서도 충돌체로서 없앤다.
                monster.Room = null; // 순서 조정

                // 몬스터 전체 수를 관리하고, 0이 되면 6초 후에 전체 맵에 뿌려준다.
                TotalMonster -= 1;

                if (TotalMonster == 0)
                {
                    PushAfter(spawnTime*1000, SummonMonsters, MapId);
                }

            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                cellPos = projectile.CellPos;

                //// 존에다가 플레이어 삭제
                //GetZone(projectile.CellPos).Projectiles.Remove(projectile);
               
                Map.ApplyLeave(projectile); // 맵에서도 충돌체로서 없앤다.
                projectile.Room = null;
                // 화살은 충돌체가 아니라서 _map.ApplyLeave를 하지 않는다.
            }
            else if (type == GameObjectType.DropItem)
            {
                DropItem dropItem = null;
                if (_dropitems.Remove(objectId, out dropItem) == false)
                    return;

                cellPos = dropItem.CellPos;

                //// 존에다가 플레이어 삭제
                //GetZone(monster.CellPos).Monsters.Remove(monster);


                Map.ApplyLeave(dropItem); // 맵에서도 충돌체로서 없앤다.
                dropItem.Room = null; // 순서 조정
            }
            else
            {
                return;
            }



            // 타인한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                Broadcast(cellPos, despawnPacket);
            }



                //// 타인한테 정보 전송
                //{
                //    S_Despawn despawnPacket = new S_Despawn();
                //    despawnPacket.ObjectIds.Add(objectId);
                //    foreach(Player p in _players.Values)
                //    {
                //        //플레이어의 경우 자신 정보는본인한테 정보 전송으로 보내기 때문에 예외를 시킴.
                //        if (p.Id != objectId)
                //            p.Session.Send(despawnPacket);
                //    }
                //}


            //}
        }



        // TODO GameRoom 내부에서 update로 돌아가기때문에 괜찮으나,
        // 그러지 않고, 외부에서 호출되면 버그가 생길 수 있다.
        Player FindPlayer(Func<GameObject,bool> condition)
        {
            foreach(Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        // 살짝 부담스러운 함수
        // 길이 없으면 또 다른 유저를 찾아나설거임.
        public Player FindClosestPlayer(Vector2Int pos, int range)
        {
            List<Player> players = GetAdjacentPlayers(pos, range);

            players.Sort((left, right) =>
            {
                int leftDist = (left.CellPos - pos).cellIdistFromZero;
                int rightDist = (right.CellPos - pos).cellIdistFromZero;

                return leftDist - rightDist;
            });

            foreach(Player player in players)
            {
                List<Vector2Int> path = Map.FindPath(pos, player.CellPos, checkObjects: true);

                if (path.Count < 2 || path.Count > range)
                    continue;

                return player;

            }

            return null;
        }


        // Room 내부에서 쓰이기 때문에 괜찮다(?)고 하는데..
        // => 다른 함수들은 Room 을 불러와서 그 함수의 함수를 실행하나.
        // Broadcast는 Room 을 불러오는게 아니라, 자체적인 Room에서 실행하는 것을 알 수있따.
        // 모든 참조를 봐보시길. 그래서 Push를 하지 않아도 된다고함.
        // 실시간으로 긁어와서 하는게 안됨. 버그가 생길 수 있다.
        public void Broadcast(Vector2Int pos , IMessage packet)
        {
            List<Zone> zones = GetAdjacentZones(pos);
            //foreach(Zone zone in zones)
            //{
            //    foreach(Player p in zone.Players)
            //    {
            //        p.Session.Send(packet);
            //    }
            //}

            // 간편 소스
            foreach (Player p in zones.SelectMany(z => z.Players))
            {

                int dx = p.CellPos.x - pos.x;
                int dy = p.CellPos.y - pos.y;

                if (Math.Abs(dx) > GameRoom.VisionCells)
                    continue;
                if (Math.Abs(dy) > GameRoom.VisionCells)
                    continue;

                 p.Session.Send(packet);

               // Console.WriteLine("보냈다잉" + packet.ToString() + "@@@@@@@@@@" + packet.GetType());
            }

            // 예약만하고, 바로 나온다.

            //lock(_lock)
            //{
            //foreach(Player p in _players.Values)
            //{
            //    p.Session.Send(packet);
            //}
            //}
        }


        public List<Player> GetAdjacentPlayers(Vector2Int pos, int range)
        {
            List<Zone> zones = GetAdjacentZones(pos, range);
            return zones.SelectMany(z => z.Players).ToList();

        }


        // zone 들이 있음
        // ㅁㅁㅁㅁㅁㅁ
        // ㅁㅁㅁㅁㅁㅁ
        // ㅁㅁㅁㅁㅁㅁ
        // ㅁㅁㅁㅁㅁㅁ
        // 가운데에서, 사각형으로 4꼭지를 잡고,
        // 그 4꼭지가 있는 존들을 가져온다.
        // #### 범위를 더 크게 가져올 수 있다.

        public List<Zone> GetAdjacentZones(Vector2Int cellPos,int range = GameRoom.VisionCells)
        {
            HashSet<Zone> zones = new HashSet<Zone>();

            int maxY = cellPos.y + range;
            int minY = cellPos.y - range;
            int maxX = cellPos.x + range;
            int minX = cellPos.x - range;

            // 좌측 상단
            Vector2Int leftTop = new Vector2Int(minX, maxY);

            int minIndexY = (Map.MaxY - leftTop.y) / ZoneCells;
            int minIndexX = (leftTop.x - Map.MinX) / ZoneCells;
        

            // 우측 하단
            Vector2Int rightBottom = new Vector2Int(maxX, minY);

            int maxIndexY = (Map.MaxY - rightBottom.y) / ZoneCells;
            int maxIndexX = (rightBottom.x - Map.MinX) / ZoneCells;

            for (int x = minIndexX; x <= maxIndexX; x++)
            {
                for(int y = minIndexY; y<=maxIndexY; y++)
                {
                    Zone zone = GetZone(y, x);

                    if (zone == null)
                        continue;

                    zones.Add(zone);
                }
            }

            // 4방향을 모두 갖는 자료구조 
            //int[] delta = new int[2] { -range, +range };
            //foreach(int dy in delta)
            //{
            //    foreach(int dx in delta)
            //    {
            //        int y = cellPos.y + dy;
            //        int x = cellPos.x + dx;
            //        Zone zone = GetZone(new Vector2Int(x, y));

            //        if (zone == null)
            //            continue;

            //        zones.Add(zone);
            //    }
            //}

            return zones.ToList();
        }


        public void IsLoginTrue(GameObject gameObject, bool randomPos)
        {
            // 게임오브젝트를 플레이어로 인식.
            Player A = gameObject as Player;

            // 클라에서 연결 되면 그제서야 tokenDB를 true로 해준다. 핑되서 검사할 수 있을때부터.

            TokenDb tokenDb = new TokenDb();
            tokenDb.TokenDbId = A.Session.TokenDbId;
            tokenDb.IsLogin = true;

            using (SharedDbContext shared = new SharedDbContext())
            {

                shared.Entry(tokenDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                shared.Entry(tokenDb).Property(nameof(tokenDb.IsLogin)).IsModified = true; // "Hp"

                //db.SaveChanges();
                bool success = shared.SaveChangesEx(); // 예외 처리

                if (success)
                {
                    Console.WriteLine("IsLogin = true =  Success");
                }

            }
        }


        public void SummonMonsters(int mapId)
        {

            Data.MapInfoData mapData = null;

            // 스킬 데이터가 없으면 return 
            if (DataManager.MapDict.TryGetValue(mapId, out mapData) == false)
                return;


            if (mapData.monsters == null)
                return;

            spawnTime = mapData.spawnTime;


            foreach(MapInfoMonsterData monstersData in mapData.monsters)
            {
                TotalMonster += monstersData.count;

                for(int i = 0; i < monstersData.count; i ++)
                {
                    Monster monsters = ObjectManager.Instance.Add<Monster>();

                    monsters.Init(monstersData.monsterId);
                    Push(EnterGame, monsters, true);
                }

            }



            //TotalMonster = 15;

            //for (int i = 0; i < TotalMonster; i++)
            //{
            //    // 마을임
            //    if (RoomId == 4)
            //        return;

            //    // TEMP 몬스터 생성
            //    Monster monsters = ObjectManager.Instance.Add<Monster>();

            //    monsters.Init(1); // 종류를 정해줌.
            //    Push(EnterGame, monsters, true); // => JobQueue 화 시켜줌.
            //                                     // true : randomSpawn
            //}


        }


        public GameObject adjacentZoneObject(Player player, int ObjectId)
        {
            GameObject targetNpc = null;

            List<Zone> adjacentZones = GetAdjacentZones(player.CellPos);

            foreach (Zone t in adjacentZones)
            {

                // 이미 존재한다면 넘긴다.
                if (targetNpc != null)
                    continue;

                // 같은 오브젝트가 같은 존에 있는지 확인
                targetNpc = t.FindOneNpc(i => i.Id == ObjectId);
            }

            return targetNpc;
        }



    }



}
