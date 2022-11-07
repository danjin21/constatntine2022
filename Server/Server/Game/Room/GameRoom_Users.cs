using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;


namespace Server.Game
{
    public partial class GameRoom : JobSerializer // 하나의 스레드에서 일감을 실행한다.
    {

     
        public void HandleUsers(Player player, C_Users usersPacket)
        {
            if (player == null)
                return;

            GameRoom room = player.Room;

            if (room == null)
                return;

            S_Users ServerUsersPacket = new S_Users();

            //foreach(Player t in room._players.Values)
            //{
            //    ServerUsersPacket.ObjectInfo.Add(t.Info);               
            //}

            List<ClientSession> A = SessionManager.Instance.GetSessions();

            foreach (ClientSession session in A)
            {
                // session 중에 유저가 없으면 리턴
                // 캐릭터 생성중일것임
                if (session.MyPlayer == null)
                    return;

                ServerUsersPacket.ObjectInfo.Add(session.MyPlayer.Info);
            }

            player.Session.Send(ServerUsersPacket);

            //ServerUsersPacket.ObjectInfo.Add
        }

    }
}
