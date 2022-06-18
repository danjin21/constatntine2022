using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class VisionCube
    {
        public Player Owner { get; private set; }
        public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();

        public VisionCube (Player owner)
        {
            Owner = owner;
        }

        public HashSet<GameObject> GatherObjects()
        {
            if (Owner == null || Owner.Room == null)
                return null;
            HashSet<GameObject> objects = new HashSet<GameObject>();

            // 주변의 있는 존들을 갖고온다.
            List<Zone> zones = Owner.Room.GetAdjacentZones(Owner.CellPos);


            Vector2Int cellPos = Owner.CellPos;


            // 존들의 플레이어들을 불러온다.
            foreach(Zone zone in zones)
            {
                foreach(Player player in zone.Players)
                {
                    int dx = player.CellPos.x - cellPos.x;
                    int dy = player.CellPos.y - cellPos.y;

                    if (Math.Abs(dx) > GameRoom.VisionCells)
                        continue;
                    if (Math.Abs(dy) > GameRoom.VisionCells)
                        continue;

                    objects.Add(player);
                }
            

                foreach (Monster monster in zone.Monsters)
                {
                    int dx = monster.CellPos.x - cellPos.x;
                    int dy = monster.CellPos.y - cellPos.y;

                    if (Math.Abs(dx) > GameRoom.VisionCells)
                        continue;
                    if (Math.Abs(dy) > GameRoom.VisionCells)
                        continue;

                    objects.Add(monster);
                }
            

            
                foreach (Projectile projectile in zone.Projectiles)
                {
                    int dx = projectile.CellPos.x - cellPos.x;
                    int dy = projectile.CellPos.y - cellPos.y;

                    if (Math.Abs(dx) > GameRoom.VisionCells)
                        continue;
                    if (Math.Abs(dy) > GameRoom.VisionCells)
                        continue;

                    objects.Add(projectile);
                }



                foreach (DropItem dropItem in zone.DropItems)
                {
                    int dx = dropItem.CellPos.x - cellPos.x;
                    int dy = dropItem.CellPos.y - cellPos.y;

                    if (Math.Abs(dx) > GameRoom.VisionCells)
                        continue;
                    if (Math.Abs(dy) > GameRoom.VisionCells)
                        continue;

                    objects.Add(dropItem);
                }

                foreach (Npc npc in zone.Npcs)
                {
                    int dx = npc.CellPos.x - cellPos.x;
                    int dy = npc.CellPos.y - cellPos.y;

                    if (Math.Abs(dx) > GameRoom.VisionCells)
                        continue;
                    if (Math.Abs(dy) > GameRoom.VisionCells)
                        continue;

                    objects.Add(npc);
                }

            }

            return objects;
        }


        public void Update()
        {
            if (Owner == null || Owner.Room == null)
                return;

            HashSet<GameObject> currentObjects = GatherObjects();

            // 기존엔 없었는데 새로 생긴 애들 Spawn 처리
            List<GameObject> added = currentObjects.Except(PreviousObjects).ToList();

            if(added.Count >0)
            {
                S_Spawn spawnPacket = new S_Spawn();

                foreach(GameObject gameObject in added)
                {
                    ObjectInfo info = new ObjectInfo();
                    info.MergeFrom(gameObject.Info);
                    spawnPacket.Objects.Add(info);
                }

                Owner.Session.Send(spawnPacket);
            }


            // 기존엔 있었는데 사라진 애들은 Despawn 처리
            List<GameObject> removed = PreviousObjects.Except(currentObjects).ToList();

            if (removed.Count > 0)
            {
                S_Despawn despawnPacket = new S_Despawn();

                foreach (GameObject gameObject in removed)
                {
                    despawnPacket.ObjectIds.Add(gameObject.Id);
                }

                Owner.Session.Send(despawnPacket);
            }

            // 바꿔준다.
            PreviousObjects = currentObjects;

            //// 이거 약간 이상하다 나중에?
            //GameRoom room = Owner.Room;

            // 500으로하니까 안 사라지는 경우가 있어서..
            Owner.Room.PushAfter(100, Update); 

        }

    }
}
