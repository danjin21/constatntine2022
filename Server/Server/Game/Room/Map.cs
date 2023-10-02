using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server.Data;


namespace Server.Game
{

    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;

        public static bool operator == (Pos lhs, Pos rhs)
        {
            return lhs.Y == rhs.Y && lhs.X == rhs.X;
        }

        public static bool operator != (Pos lhs, Pos rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return (Pos)obj == this;
        }

        public override int GetHashCode()
        {
            long value = (Y << 32) | X;
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }


    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y) { this.x = x; this.y = y; }

        public static Vector2Int up { get { return new Vector2Int(0, 1); } }
        public static Vector2Int down { get { return new Vector2Int(0, -1); } }
        public static Vector2Int left { get { return new Vector2Int(-1, 0); } }
        public static Vector2Int right { get { return new Vector2Int(1, 0); } }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public float magnitude { get { return (float)Math.Sqrt(sqrMagnitude); } }
        public int sqrMagnitude{ get { return (x * x + y * y); } }

        // 좌측으로 몇번 우측으로 몇번가야 갈 수 있는지.
        public int cellIdistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }


    }



    public class Map
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public int SizeX { get { return MaxX - MinX + 1; } }
        public int SizeY { get { return MaxY - MinY + 1; } }

        bool[,] _collision;
        GameObject[,] _objects;
        PortalData[,] _portals;

        public bool CanGo(Vector2Int cellPos, bool checkObjects = true)
        {
            //if (cellPos.x < MinX || cellPos.x > MaxX)
            //    return false;
            //if (cellPos.y < MinY || cellPos.y > MaxY)
            //    return false;

            // 진형쓰룰
            if (cellPos.x < MinX || cellPos.x >= MaxX)
                return false;
            if (cellPos.y < MinY || cellPos.y >= MaxY)
                return false;


            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y-1;


            return !_collision[y, x] && ((checkObjects == false) || _objects[y, x] == null);
        }


        public PortalData IsPortal(Vector2Int cellPos)
        {



            // 진형쓰룰
            if (cellPos.x < MinX || cellPos.x >= MaxX)
                return null;
            if (cellPos.y < MinY || cellPos.y >= MaxY)
                return null;


            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y-1;


            return (_portals[y, x]);

        }



        // 좌표에 플레이어가 있는지 확인
        public GameObject Find(Vector2Int cellPos)
        {
            // 진형쓰룰
            if (cellPos.x < MinX || cellPos.x >= MaxX)
                return null;
            if (cellPos.y < MinY || cellPos.y >= MaxY)
                return null;


            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y-1;
            return _objects[y, x];
        }


        public bool ApplyLeave(GameObject gameObject)
        {
            // 예외를 처리한다.
            if (gameObject.Room == null)
                return false;
            if (gameObject.Room.Map != this)
                return false;

            PositionInfo posInfo = gameObject.PosInfo;

            // 캐릭터의 위치가 현재 맵을 벗어나 있는지 체크 | 진형쓰룰
            if (posInfo.PosX < MinX || posInfo.PosX >= MaxX)
                return false;
            if (posInfo.PosY < MinY || posInfo.PosY >= MaxY)
                return false;

            // Zone 에서 삭제처리를 함.

            Zone zone = gameObject.Room.GetZone(gameObject.CellPos);
            zone.Remove(gameObject);


            // 있는 공간에서 사라지게만듬
            {
                int x = posInfo.PosX - MinX;
                int y = MaxY - posInfo.PosY -1;
                if (_objects[y, x] == gameObject)
                    _objects[y, x] = null;
            }
            return true;
        }

        // collision : 충돌 영향을 안주겠다.
        public bool ApplyMove(GameObject gameObject, Vector2Int dest, bool checkObjects = true, bool collision = true)
        {



            // 예외를 처리한다.
            if (gameObject.Room == null)
                return false;
            if (gameObject.Room.Map != this)
                return false;



            // 목적지에 갈 수 있는지 체크
            if (CanGo(dest, checkObjects) == false)
                return false;




            // 진형쓰 수정 -> 가능한지 확인하고 이동을 시킨다.
            // 나의 위치 체크와, 공간에서 사라지게 하고 하는게 다 ApplyLeave에 들어가있다.
            //ApplyLeave(gameObject);

            PositionInfo posInfo = gameObject.Info.PosInfo;


            // 목적지에 나를 텔레포트 시킴
            if (collision)
            {

                // 있는 공간에서 사라지게만듬
                {
                    int x = posInfo.PosX - MinX;
                    int y = MaxY - posInfo.PosY - 1;
                    if (_objects[y, x] == gameObject)
                        _objects[y, x] = null;
                }

                // 현재 위치에 추가해줌.
                {
                    int x = dest.x - MinX;
                    int y = MaxY - dest.y - 1;
                    _objects[y, x] = gameObject;
                }
            }


            // 존 영역
            // 타입을 불러오는 것

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);


            // 존 삽입 ( 타입별로 )

            if (type == GameObjectType.Player)
            {
                Player player = (Player)gameObject;


                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    //if (now != null)
                    now.Players.Remove(player);
                    //if (after != null)
                    after.Players.Add(player);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = (Monster)gameObject;


                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    //if (now != null)
                    now.Monsters.Remove(monster);
                    //if (after != null)
                    after.Monsters.Add(monster);
                }
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = (Projectile)gameObject;


                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    //if (now != null)
                    now.Projectiles.Remove(projectile);
                    //if (after != null)
                    after.Projectiles.Add(projectile);
                }
            }
            else if (type == GameObjectType.DropItem)
            {
                DropItem dropItem = (DropItem)gameObject;


                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    //if (now != null)
                    now.DropItems.Remove(dropItem);
                    //if (after != null)
                    after.DropItems.Add(dropItem);
                }
            }



            // 실제 좌표 이동 | 사실 왜 바꿔주는지는 아직 모르겠음 return도 안하는데
            posInfo.PosX = dest.x;
            posInfo.PosY = dest.y;



            return true;
        }


        //public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
        //public void LoadMap(int mapId, string pathPrefix = "D:/Inflearn BackUp/20210622 Constantine_AttackEffect_2/Common/MapData")
        // 잠시 Release 때문에 경로 수정했었음.
        public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
        {

            string mapName = "Map_" + mapId.ToString("000");

            // Collision 관련 파일
            string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
            StringReader reader = new StringReader(text);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());

            int xCount = MaxX - MinX ;
            int yCount = MaxY - MinY ;
            _collision = new bool[yCount, xCount];
            _objects = new GameObject[yCount, xCount];
            _portals = new PortalData[yCount, xCount];

            for (int y = 0; y < yCount; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < xCount; x++)
                {
                    _collision[y, x] = (line[x] == '1' ? true : false);

                    if (line[x] == '2')
                    {
                        PortalData portalData = null;
                        

                        foreach(PortalData portal in DataManager.PortalDict.Values)
                        {

                            int MapX = portal.posX - MinX;
                            int MapY = MaxY - portal.posY-1;

                            //Console.WriteLine($"MapX : {MapX} / x : {x}");
                            //Console.WriteLine($"MapY : {MapY} / y : {y}" );

                            if (portal.map == mapId && MapX == x && MapY == y)
                            {
                                portalData = portal;




                                Console.WriteLine($"목적 포탈 ID : {portalData.destPortal} ");


                                // destPortal을 찾기

                                PortalData destPortalData = null;
                                DataManager.PortalDict.TryGetValue(portalData.destPortal, out destPortalData);

                                if (destPortalData == null)
                                    continue;
                                

                                // destPos 찾아주기
                                portalData.destMap = destPortalData.map;
                                portalData.destPosX = destPortalData.posX;
                                portalData.destPosY = destPortalData.posY;

                                // destPortal 포지션에 그 destPortal의 direction 을 찾아 destPos 를 정해준다.

                                switch (destPortalData.direction)
                                {
                                    case 1:
                                        portalData.destPosY -= 1;
                                        break;
                                    case 2:
                                        portalData.destPosX -= 1;
                                        break;
                                    case 3:
                                        portalData.destPosY += 1;
                                        break;
                                    case 4:
                                        portalData.destPosX += 1;
                                        break;
                                    default:                                        
                                        break;
                                }

                                Console.WriteLine($"내 포탈 ID : {portalData.portalId} / 다음 맵 : {portalData.destMap} / 목적지 포탈 ID : {destPortalData.portalId} / {portalData.destPosX} / {portalData.destPosY}");



                                Console.WriteLine("뾰로롱");
                            }
                        }

                        _portals[y, x] = portalData;
                    }

                }
            }

        }


        #region A* PathFinding

        // U D L R
        int[] _deltaY = new int[] { 1, -1, 0, 0 };
        int[] _deltaX = new int[] { 0, 0, -1, 1 };
        int[] _cost = new int[] { 10, 10, 10, 10 };


        // checkObjects == true 가 되있으면 벽 뿐만아니라, 몬스터 플레이어도 체크
        public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool checkObjects = true, int maxDist = 10)
        {
            List<Pos> path = new List<Pos>();

            // 점수 매기기
            // F = G + H
            // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
            // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

            // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
            //bool[,] closed = new bool[SizeY, SizeX]; 
            HashSet<Pos> closeList = new HashSet<Pos>(); // CloseList


            // (y, x) 가는 길을 한 번이라도 발견했는지
            // 발견X => MaxValue
            // 발견O => F = G + H
            //int[,] open = new int[SizeY, SizeX]; // OpenList
            Dictionary<Pos, int> openList = new Dictionary<Pos, int>(); // OpenList

            //for (int y = 0; y < SizeY; y++)
            //    for (int x = 0; x < SizeX; x++)
            //        open[y, x] = Int32.MaxValue;

            //Pos[,] parent = new Pos[SizeY, SizeX];

            Dictionary<Pos, Pos> parent = new Dictionary<Pos, Pos>();

            // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // CellPos -> ArrayPos
            Pos pos = Cell2Pos(startCellPos);
            Pos dest = Cell2Pos(destCellPos);

            // 시작점 발견 (예약 진행)
            //open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
            openList.Add(pos, 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)));

            pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
            parent.Add(pos, pos);
            //parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다
                //PQNode node = pq.Pop();
                PQNode pqNode = pq.Pop();
                Pos node = new Pos(pqNode.Y, pqNode.X);

                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
                if (closeList.Contains(node))
                    continue;

                // 방문한다
                closeList.Add(node);
                //closed[node.Y, node.X] = true;

                // 목적지 도착했으면 바로 종료
                if (node.Y == dest.Y && node.X == dest.X)
                    break;

                // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
                for (int i = 0; i < _deltaY.Length; i++)
                {
                    Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                    // 너무 멀면 스킵
                    if (Math.Abs(pos.Y - next.Y) + Math.Abs(pos.X - next.X) > maxDist)
                        continue;


                    // 유효 범위를 벗어났으면 스킵
                    // 벽으로 막혀서 갈 수 없으면 스킵 // checkObjects == true 가 되있으면 벽 뿐만아니라, 몬스터 플레이어도 체크
                    if (next.Y != dest.Y || next.X != dest.X)
                    {
                        if (CanGo(Pos2Cell(next), checkObjects) == false) // CellPos
                            continue;
                    }

                    // 이미 방문한 곳이면 스킵
                    if (closeList.Contains(next))
                        continue;

                    //if (closed[next.Y, next.X])
                    //    continue;

                    // 비용 계산
                    int g = 0;// node.G + _cost[i];
                    int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                    // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵

                    int value = 0;
                    if (openList.TryGetValue(next, out value) == false)
                        value = Int32.MaxValue;

                    //if (open[next.Y, next.X] < g + h)
                    //    continue;
                    if (value < g + h)
                        continue;

                    // 예약 진행
                    if (openList.TryAdd(next, g + h) == false)
                        openList[next] = g + h;

                    //open[next.Y, next.X] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });

                    if (parent.TryAdd(next, node) == false)
                        parent[next] = node;

                    // parent[next.Y, next.X] = new Pos(node.Y, node.X);
                }
            }

            return CalcCellPathFromParent(parent, dest);
        }

        List<Vector2Int> CalcCellPathFromParent(Dictionary<Pos, Pos> parent, Pos dest)
        {
            List<Vector2Int> cells = new List<Vector2Int>();


            // 길을 못찾을 경우
            if(parent.ContainsKey(dest) == false)
            {
                Pos best = new Pos();
                int bestDist = Int32.MaxValue;
                foreach(Pos pos in parent.Keys)
                {
                    // 평가거리
                    int dist = Math.Abs(dest.X - pos.X) + Math.Abs(dest.Y - pos.Y);

                    // 제일 우수한 후보를 뽑느다.
                    
                    if(dist < bestDist)
                    {
                        best = pos;
                        bestDist = dist;
                    }
                }

                dest = best;

                //return cells;
            }

            //int y = dest.Y;
            //int x = dest.X;
            {
                Pos pos = dest;
                while (parent[pos] != pos/*parent[y, x].Y != y || parent[y, x].X != x*/)
                {
                    cells.Add(Pos2Cell(pos));
                    pos = parent[pos];
                    //Pos pos = parent[y, x];
                    //y = pos.Y;
                    //x = pos.X;
                }
                cells.Add(Pos2Cell(pos));
                cells.Reverse();
            }

            return cells;
        }

        Pos Cell2Pos(Vector2Int cell)
        {
            // CellPos -> ArrayPos
            return new Pos(MaxY - cell.y, cell.x - MinX);
        }

        Vector2Int Pos2Cell(Pos pos)
        {
            // ArrayPos -> CellPos
            return new Vector2Int(pos.X + MinX, MaxY - pos.Y);
        }

        #endregion
    }

    }
