using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class ObjectManager
    {

        public static ObjectManager Instance { get; } = new ObjectManager();

        object _lock = new object();

        // 귓속말 보낼 용도로 만든거임. List들은 GameRoom에 다 만들어져있음.
        Dictionary<int, Player> _players = new Dictionary<int, Player>();


        // [ UNUSED(1) ] [ TYPE(7) ] [ ID(24) ]
        // [........ | ........|........|........]

        int _counter = 0; 



        public T Add<T>() where T : GameObject, new()  // GameObject여야 하고, New로 할당한 아이만
        {
            T gameObject = new T();


            lock(_lock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if(gameObject.ObjectType == GameObjectType.Player)
                {
                    _players.Add(gameObject.Id, gameObject as Player);
                }
            }


            return gameObject;

        }

        int GenerateId(GameObjectType type)
        {
            lock(_lock)
            {
                return ((int)type << 24) | (_counter++);  // Type과 Counter를 추가한다
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }


        //public Player Add()
        //{
        //    Player player = new Player();

        //    lock (_lock)
        //    {
        //      //  player.Info.ObjectId = _playerId;

        //       // _players.Add(_playerId, player);
        //       // _playerId++;

        //    }

        //    return player;
        //}

        public bool Remove(int objectId)
        {

            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if(objectType == GameObjectType.Player)
                return _players.Remove(objectId);
            }

            return false;
        }

        public Player Find(int objectId)
        {


            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {

                if (objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(objectId, out player))
                        return player;

                }

            }
            return null;
        }




    }
}
