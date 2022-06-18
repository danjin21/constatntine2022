using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class GameLogic : JobSerializer
    {

        public static GameLogic Instance { get; } = new GameLogic();

        //object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        //int _roomId = 1;

        public void Update()
        {

            Flush();


            foreach (GameRoom room in _rooms.Values)
            {
                room.Update();
            }


            
        }


        public GameRoom Add(int mapId)
        {
            GameRoom gameRoom = new GameRoom();
            //gameRoom.Init(mapId);
            gameRoom.Push(gameRoom.Init, mapId, 40 /*Zone 크기 visionCell의 2배 여야한다.*/); // => JobQueue 방식





            //gameRoom.RoomId = _roomId;
            //_rooms.Add(_roomId, gameRoom);
            //_roomId++;


            // 이제부터 mapID 가 곧 room의 Id이다.
            gameRoom.RoomId = mapId;
            _rooms.Add(mapId, gameRoom);
            //_roomId++;


            return gameRoom;
        }

        public bool Remove (int roomId)
        {
            //lock(_lock)
            //{
                return _rooms.Remove(roomId);
            //}
        }

        public GameRoom Find(int roomId)
        {
            //lock(_lock)
            //{
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;
                

                return null;
            //}
        }



    }
}
