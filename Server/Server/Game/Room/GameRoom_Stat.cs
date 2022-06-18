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
    public partial class GameRoom : JobSerializer   // 하나의 스레드에서 일감을 실행한다.
    {


        public void HandleStatUp(Player player, C_StatUp statupPacket)
        {
            if (player == null)
                return;

            // 스텟포인트가 1 미만이면 리턴한다.
            if (player.Stat.StatPoint < 1)
                return;

            // 스텟이 1,2,3,4 중에 없으면 리턴
            if (statupPacket.Stat != 1 && statupPacket.Stat != 2 && statupPacket.Stat != 3 && statupPacket.Stat != 4)
                return;

            // 스텟포인트 올려주기
            switch (statupPacket.Stat)
            {
                case 1: // STR

                    player.Stat.Str += 1;
                    break;

                case 2: // DEX

                    player.Stat.Dex += 1;
                    break;

                case 3: // INT

                    player.Stat.Int += 1;
                    break;

                case 4: // LUK

                    player.Stat.Luk += 1;
                    break;
            }

            // 스텟포인트 차감
            player.Stat.StatPoint -= 1;






            // DB값 수정

            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Str = player.Stat.Str;
            playerDb.Dex = player.Stat.Dex;
            playerDb.Int = player.Stat.Int;
            playerDb.Luk = player.Stat.Luk;
            playerDb.StatPoint = player.Stat.StatPoint;


            DbTransaction.Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                    db.Entry(playerDb).Property(nameof(playerDb.Str)).IsModified = true; // "Str"
                    db.Entry(playerDb).Property(nameof(playerDb.Dex)).IsModified = true; // "Dex"
                    db.Entry(playerDb).Property(nameof(playerDb.Int)).IsModified = true; // "Int"
                    db.Entry(playerDb).Property(nameof(playerDb.Luk)).IsModified = true; // "Luk"
                    db.Entry(playerDb).Property(nameof(playerDb.StatPoint)).IsModified = true; // "StatPoint"


                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (success)
                    {
                        // Me : 나한테 결과 보내는 부분
                        Push(() =>    // 바로 데이터를 받는다고 가정
                        {
                            S_StatUp statupPacket = new S_StatUp();

                            statupPacket.Stat = new StatInfo();
                            statupPacket.Stat.Str = player.Stat.Str;
                            statupPacket.Stat.Dex = player.Stat.Dex;
                            statupPacket.Stat.Int = player.Stat.Int;
                            statupPacket.Stat.Luk = player.Stat.Luk;
                            statupPacket.Stat.StatPoint = player.Stat.StatPoint;

                            // 모은것 패킹
                            player.Session.Send(statupPacket);
                        }
                        );
                    }
                }

            });


        }

   


    }




}
