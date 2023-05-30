using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class Zone
    {

        public int IndexY { get; private set; }
        public int IndexX { get; private set; }

        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
        public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
        public HashSet<Projectile> Projectiles { get; set; } = new HashSet<Projectile>();
        public HashSet<DropItem> DropItems { get; set; } = new HashSet<DropItem>();
        public HashSet<Npc> Npcs { get; set; } = new HashSet<Npc>();

        public Zone(int y, int x)
        {
            IndexY = y;
            IndexX = x;
        }

        public void Remove(GameObject gameObject)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            switch(type)
            {
                case GameObjectType.Player:
                    Players.Remove((Player)gameObject);
                    break;

                case GameObjectType.Monster:
                    Monsters.Remove((Monster)gameObject);
                    break;

                case GameObjectType.Projectile:
                    Projectiles.Remove((Projectile)gameObject);
                    break;

                case GameObjectType.DropItem:
                    DropItems.Remove((DropItem)gameObject);
                    break;
                case GameObjectType.Npc:
                    Npcs.Remove((Npc)gameObject);
                    break;
            }
        }

        public Player FindOnePlayer(Func<Player,bool> condition)
        {
            foreach(Player player in Players)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }
        public List<Player> FindAllPlayers(Func<Player, bool> condition)
        {
            List<Player> findList = new List<Player>();

            foreach (Player player in Players)
            {
                if (condition.Invoke(player))
                    findList.Add(player);
            }

            return findList;
        }

        public DropItem FindOneItem(Func<DropItem, bool> condition)
        {
            List<DropItem> itemList = new List<DropItem>(DropItems);

            List<DropItem> newItemList = itemList.OrderByDescending(i => i.Id).ToList();

            foreach (DropItem item in newItemList)
            {
                if (condition.Invoke(item))
                {
                    Console.WriteLine("같은 포지션에 있는 아이템 리스트들" + item.Stat.TemplateId + "/" + item.Id);
                    return item;
                }
            }


            // DropItems를 ID에 맞게 소팅해준다.
            //itemList.OrderByDescending(p => p.Id);
            

            //foreach(DropItem item in itemList)
            //{
            //    if (condition.Invoke(item))
            //        Console.WriteLine("같은 포지션에 있는 아이템 리스트들" + item.Stat.TemplateId + "/" + item.Id);
            //}

            //foreach (DropItem item in itemList)
            //{
            //    if (condition.Invoke(item))
            //        return item;
            //}


            //foreach (DropItem item in DropItems.Reverse())
            //{
            //    if (condition.Invoke(item))
            //        Console.WriteLine("같은 포지션에 있는 아이템 리스트들" + item.Stat.TemplateId + "/" + item.Id);
            //}

            //foreach (DropItem item in DropItems.Reverse())
            //{
            //    if (condition.Invoke(item))
            //        return item;
            //}

            return null;
        }

        public Npc FindOneNpc (Func<Npc, bool> condition)
        {
            foreach(Npc npc in Npcs)
            {
                if (condition.Invoke(npc))
                    return npc;
            }

            return null;
        }




    }
}
