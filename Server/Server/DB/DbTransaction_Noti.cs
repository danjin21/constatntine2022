using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DB
{
    // 일련의 DB작업을 해준다.

    public partial class DbTransaction : JobSerializer
    {

        // 콜백을 하지 않기에 room도 받지 않는다.
        public static void EquipItemNoti(Player player, Item item)
        {
            if (player == null || item == null)
                return;


            ItemDb itemDb = new ItemDb()
            {
                ItemDbId = item.ItemDbId,
                Equipped = item.Equipped

            };

            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(itemDb).State = EntityState.Unchanged;
                    db.Entry(itemDb).Property(nameof(ItemDb.Equipped)).IsModified = true;

                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (!success)
                    {
                        // 실패했으면 Kick or 로그를 남긴다. 
                    }
                }
            });

        }





        // 콜백을 하지 않기에 room도 받지 않는다.
        public static void UseItemNoti(Player player, Item item)
        {
            if (player == null || item == null)
                return;


            ItemDb itemDb = new ItemDb()
            {
                ItemDbId = item.ItemDbId,
                Count = item.Count

            };

            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    //db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                    //db.Entry(playerDb).Property(nameof(playerDb.Hp)).IsModified = true; // "Hp"



                    db.Entry(itemDb).State = EntityState.Unchanged;
                    db.Entry(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;

                    if (item.Count <= 0)
                    {
                        List<ItemDb> items = db.Items
                            .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                            .ToList();

                        //foreach (ItemDb item in items)
                        //{
                        //    Console.WriteLine($"item {item.ItemDbId} of {item.OwnerDbId} slot : {item.Slot}");
                        //}

                        //Console.WriteLine($"----------------------------------------------------------------------");

                        //foreach (Item t in player.Inven.Items.Values)
                        //{
                        //    Console.WriteLine($"servermemory : {t.ItemDbId}of { player.PlayerDbId} + {t.Slot}");

                        //}

                        foreach (Item t in player.Inven.Items.Values)
                        {
                            foreach (ItemDb itemDb in items)
                            {

                                // 아무 문제가 없으면
                                if (itemDb.ItemDbId == t.ItemDbId)
                                {
                                    itemDb.Slot = t.Slot;
                                }
                            }
                        }

                        db.Remove(itemDb);


                        // 단축키에서도 삭제시켜준다.
                        // 플레이어도 같이 확인해줘야 한다.

                        List<KeySettingDb> key = db.Keys
                            .Where(i => i.OwnerDbId == player.PlayerDbId && i.action == item.TemplateId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                            .ToList();

                        if (key.Count != 0)
                        {
                            // 메모리
                            Key deleteKey = player.Keys.Get(key[0].KeySettingDbId);
                            player.Keys.Delete(deleteKey);

                            // 서버에서 삭제
                            db.Remove(key[0]);

                            // 클라에게 지우라고 보내주기.

                            S_KeySetting ServerkeysettingPacket = new S_KeySetting();
                            KeySettingInfo keysettingInfo = new KeySettingInfo();
                            keysettingInfo.Key = deleteKey.KeyValue;
                            keysettingInfo.KeyDbId = deleteKey.KeyDbId;
                            keysettingInfo.Type = deleteKey.Type;
                            keysettingInfo.OwnerDbId = player.PlayerDbId;
                            keysettingInfo.Action = -1;

                            ServerkeysettingPacket.KeySettingInfo.Add(keysettingInfo);

                            // 패킷 보내주기
                            player.Session.Send(ServerkeysettingPacket);
                        }


                        //foreach (ItemDb item in items)
                        //{
                        //    Console.WriteLine($"item {item.ItemDbId} of {item.OwnerDbId} slot : {item.Slot}");
                        //}

                        //Console.WriteLine($"----------------------------------------------------------------------");

                    }




                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (!success)
                    {
                        // 실패했으면 Kick or 로그를 남긴다. 
                    }
                }
            });
        }




        // 콜백을 하지 않기에 room도 받지 않는다.
        public static void KeySettingNoti(Player player, Key key)
        {
            if (player == null || key == null)
                return;



            KeySettingDb keyDb = new KeySettingDb()
            {
                KeySettingDbId = key.KeyDbId,
                action = key.Action,
                type = key.Type,
                key = key.KeyValue,

            };



            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(keyDb).State = EntityState.Unchanged;
                    db.Entry(keyDb).Property(nameof(KeySettingDb.action)).IsModified = true;
                    db.Entry(keyDb).Property(nameof(KeySettingDb.type)).IsModified = true;
                    db.Entry(keyDb).Property(nameof(KeySettingDb.key)).IsModified = true;




                    if (keyDb.action == -1)
                    {
                        db.Remove(keyDb);
                    }

                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (!success)
                    {
                        // 실패했으면 Kick or 로그를 남긴다. 
                    }
                }
            });
        }






    }
}
