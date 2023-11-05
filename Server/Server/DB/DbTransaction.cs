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

        // 유일한 DB관리자 : 캐셔
        public static DbTransaction Instance { get; } = new DbTransaction();


        // 1. 올인원 버전

        // 웨이터한테 카드 주고, 웨이터가 결제를 하러 가고, 웨이터가 와서 영수증과 카드를 주는.
        // Me (GameRoom) -> You(Db) -> Me (GameRoom)
        public static void SavePlayerStatus_AllInOne(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            // Me (GameROom)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.Stat.Hp;
            playerDb.Mp = player.Stat.Mp;

            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                    db.Entry(playerDb).Property(nameof(playerDb.Hp)).IsModified = true; // "Hp"
                    db.Entry(playerDb).Property(nameof(playerDb.Mp)).IsModified = true; // "Mp"

                    //db.SaveChanges();
                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (success)
                    {
                        // Me : 나한테 결과 보내는 부분
                        room.Push(() =>    // 바로 데이터를 받는다고 가정
                        Console.WriteLine($"② Hp Saved ({playerDb.Hp})")
                        );
                    }
                }
            });

        }






        // 2. 단계별 버전

        // 웨이터한테 카드 주고, 웨이터가 결제를 하러 가고, 웨이터가 와서 영수증과 카드를 주는.
        // step 1. Me (GameRoom) ->
        public static void SavePlayerStatus_Step1(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            // Me (GameROom)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.Stat.Hp;
            playerDb.Mp = player.Stat.Mp;
            Instance.Push<PlayerDb, GameRoom>(SavePlayerStatus_Step2, playerDb, room);


        }

        // 구개발에서는 이거를 DB서버를 만들어서 처리하지만.. 요즘은 그렇게 안한다.
        // step 2. You(Db) -> 
        public static void SavePlayerStatus_Step2(PlayerDb playerDb, GameRoom room)
        {
            using (AppDbContext db = new AppDbContext())
            {

                db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                db.Entry(playerDb).Property(nameof(playerDb.Hp)).IsModified = true; // "Hp"

                //db.SaveChanges();
                bool success = db.SaveChangesEx(); // 예외 처리

                if (success)
                {
                    room.Push(SavePlayerStatus_Step3, playerDb.Hp);
                }
            }
        }


        // step 3. -> Me (GameRoom)
        public static void SavePlayerStatus_Step3(int hp)
        {
            // 바로 데이터를 받는다고 가정
            Console.WriteLine($"Hp Saved ({hp})");


        }


        // 보상하는 부분
        public static bool RewardPlayer(Player player, RewardData rewardData, GameRoom room , int multiSlot = -1)
        {
            if (player == null || rewardData == null || room == null)
                return false;

            // TODO : 살짝 문제가 있음. 슬롯이 꽉 차 있으면 리턴 때려버리기.

            // 1) DB에다가 저장 요청
            // 2) DB 저장 OK
            // 3) 메모리에적용
            // 타이밍 이슈 -> 동시에 같은 슬롯에 생기는 문제가 생길수 있다.

            if (rewardData.itemId == 99999)
            {

                int gold = rewardData.count;

                // Me (GameROom)
                PlayerDb playerDb = new PlayerDb();
                playerDb.PlayerDbId = player.PlayerDbId;
                player.Stat.Gold += gold;
                playerDb.Gold = player.Stat.Gold;

                // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
                // 장부는 Program.cs 에서 만들어줘야함.
                Instance.Push(() =>
                {
                    using (AppDbContext db = new AppDbContext())
                    {

                        db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                        db.Entry(playerDb).Property(nameof(playerDb.Gold)).IsModified = true; // "Hp"

                        //db.SaveChanges();
                        bool success = db.SaveChangesEx(); // 예외 처리

                        if (success)
                        {
                            // Me : 나한테 결과 보내는 부분
                            room.Push(() =>    // 바로 데이터를 받는다고 가정
                            {
                                S_AddItem itemPacket = new S_AddItem();
                                ItemInfo itemInfo = new ItemInfo();
                                itemInfo.TemplateId = 99999;
                                itemInfo.Count = gold;
                                itemPacket.Items.Add(itemInfo);

                                player.Session.Send(itemPacket);
                            }
                            );
                        }
                    }
                });
                return true;
            }

            // itemType 갖고오기.

            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(rewardData.itemId, out itemData);

            ItemType ItemType = itemData.itemType;

            // 90000 > ? 
            // 중복되는 아이템이 아니고, 슬롯이 없으면 false ( 중복이되면 갯수 증가로 될수 있기때문)
            int? slot = player.Inven.GetEmptySlot();

            if (slot == null && (ItemType == ItemType.Consumable || ItemType == ItemType.Etc))
            {
                // 이거 값 안받아 와지면 오류뜬다. 그래서 무조건 위에 slot 그거 중복 GetSlotFrom 사용해야함
                // 슬롯이 없는데, 중복이되는 포션일 경우에는 그 포션이 있는 슬롯을 가지고 온다.
                slot = player.Inven.GetSlotFromTemplateId(rewardData.itemId);

                // 그럼에도 불구하고 없으면 false
                if (slot == null)
                    return false;
                else
                {

                }

            }
            else if (slot == null && (ItemType == ItemType.Consumable || ItemType == ItemType.Etc) == false)
                return false;


            // multiSlot 일 경우 ( 동시에 아이템 많이주는 것일 경우
            // 슬롯을 지금부터 미리 바꿔준다.

            if(multiSlot != -1)
            {
                slot = multiSlot;
            }




            ItemDb itemDb = new ItemDb()
            {
                TemplateId = rewardData.itemId,
                Count = rewardData.count,
                Slot = slot.Value, // 이거 값 안받아 와지면 오류뜬다. 그래서 무조건 위에 slot 그거 중복 GetSlotFrom 사용해야함
                OwnerDbId = player.PlayerDbId,

                ReqStr = rewardData.itemInfo.ReqStr,
                ReqDex = rewardData.itemInfo.ReqDex,
                ReqInt = rewardData.itemInfo.ReqInt,
                ReqLuk = rewardData.itemInfo.ReqLuk,
                ReqLev = rewardData.itemInfo.ReqLev,
                ReqPop = rewardData.itemInfo.ReqPop,
                UpgradeSlot = rewardData.itemInfo.UpgradeSlot,
                Str = rewardData.itemInfo.Str,
                Dex = rewardData.itemInfo.Dex,
                Int = rewardData.itemInfo.Int,
                Luk = rewardData.itemInfo.Luk,
                Hp = rewardData.itemInfo.Hp,
                Mp = rewardData.itemInfo.Mp,
                WAtk = rewardData.itemInfo.WAtk,
                MAtk = rewardData.itemInfo.MAtk,
                WDef = rewardData.itemInfo.WDef,
                MDef = rewardData.itemInfo.MDef,
                Speed = rewardData.itemInfo.Speed,
                AtkSpeed = rewardData.itemInfo.AtkSpeed,
                Durability = rewardData.itemInfo.Durability,
                Enhance = rewardData.itemInfo.Enhance,
                WPnt = rewardData.itemInfo.WPnt,
                MPnt = rewardData.itemInfo.MPnt,


            };


            //// 서버쪽에서 확인을 해줘야한다.

            //Item checkItem = null;
            //checkItem = player.Inven.Get_template(rewardData.itemId);

            //if(checkItem == null)
            //{
            //    Item newItem = Item.MakeItem(itemDb);
            //    player.Inven.Add(newItem);
            //}
            //else
            //{
            //    checkItem.Count += itemDb.Count;
            //}

            //Item newItem = Item.MakeItem(itemDb);
            //player.Inven.Add(newItem);

            //Item newItem = Item.MakeItem(itemDb);
            //player.Inven.Add(newItem);

            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    List<ItemDb> items = db.Items
                            .Where(i => i.TemplateId == itemDb.TemplateId && i.OwnerDbId == player.PlayerDbId) // 템플릿 아이디가 같은거
                            .ToList();


                    //foreach (ItemDb item in items)
                    //{

                    //    Console.WriteLine($"item {item.ItemDbId} of {item.OwnerDbId} slot : {item.Slot} template :{item.TemplateId}");
                    //}

                    // itemType 갖고오기.

                    ItemData itemData = null;
                    DataManager.ItemDict.TryGetValue(rewardData.itemId, out itemData);

                    ItemType ItemType = itemData.itemType;

                    // 아이템이 없거나 template id 가 90000 미만일 경우 그냥 갯수 추가 ㄴㄴ
                    if (items.Count <= 0 || (ItemType == ItemType.Consumable || ItemType == ItemType.Etc) == false)
                    {
                        Console.WriteLine("새로운 아이템");

                        // 여기는 변경해주는 것이 아니라 새로 만들어주는거라서 아래와 같이 한다.

                        // 한번더 슬롯 확인한다. ( 그 사이에 여러 아이템 획득할수도 있기 떄문임. ) 

                        db.Items.Add(itemDb);
                        bool success = db.SaveChangesEx(); // 예외 처리

                        if (success)
                        {
                            // Me : 나한테 결과 보내는 부분
                            room.Push(() =>    // 바로 데이터를 받는다고 가정
                            {
                                // 이제서야 아이템을 넣어준다.
                                Item newItem = Item.MakeItem(itemDb);
                                player.Inven.Add(newItem);


                                // Client 알림 ( 패킷전송 )
                                {
                                    S_AddItem itemPacket = new S_AddItem();
                                    ItemInfo itemInfo = new ItemInfo();
                                    itemInfo.MergeFrom(newItem.Info);
                                    itemPacket.Items.Add(itemInfo);

                                    player.Session.Send(itemPacket);


                                }
                            });
                        }
                    }
                    else if(items.Count > 0 && (ItemType == ItemType.Consumable || ItemType == ItemType.Etc) == true)
                    {


                        // 한번더 슬롯 확인한다. ( 그 사이에 여러 아이템 획득할수도 있기 떄문임. ) 

                        Console.WriteLine("이미있네요.");
                        Console.WriteLine($"item {items[0].ItemDbId} of {items[0].OwnerDbId} slot : {items[0].Slot} template :{items[0].TemplateId}");

                        items[0].Count += itemDb.Count;

                        bool success = db.SaveChangesEx(); // 예외 처리
                        if (success)
                        {
                            // Me : 나한테 결과 보내는 부분
                            room.Push(() =>    // 바로 데이터를 받는다고 가정
                            {

                                //S_IncreaseItem Packet을 만들고, 더하는식으로 하거나 교체하는 패킷으로 해야할듯.

                                // 갱신된 아이템을 넣어준다.
                                Item newItem = Item.MakeItem(items[0]);
                                player.Inven.Add(newItem);

                                // Client 알림 ( 패킷전송 )
                                {
                                    S_AddItem itemPacket = new S_AddItem();
                                    ItemInfo itemInfo = new ItemInfo();
                                    itemInfo.MergeFrom(newItem.Info);
                                    itemPacket.Items.Add(itemInfo);

                                    player.Session.Send(itemPacket);
                                }
                            }
                            );
                        }

                    }






                }
            });

            return true;
        }



        // 경험치 올려주는 부분
        public static void ExpPlayer(Player player, int exp, GameRoom room)
        {
            if (player == null || room == null)
                return;

            bool LevelUp = false;


            player.Stat.Exp += exp;

            // 레벨업 했을 때
            if (player.Stat.Exp >= player.Stat.TotalExp)
            {

                // 1레벨 스탯 정보 추출
                StatInfo stat = null;


     
                DataManager.StatDict.TryGetValue(player.Stat.Level, out stat);

                if (stat == null)
                {

                    Console.WriteLine("Stat 정보가 없습니다.");
                    player.Stat.Exp = 0;
                    return;
                  
                }


                player.Stat.Level += 1;
                player.Stat.Exp = 0;
                LevelUp = true;
                player.Stat.TotalExp = stat.TotalExp;

                // 스텟 포인트 추가

                player.Stat.StatPoint += 5;

                // 전체 체력 및 마나 회복

                player.Stat.MaxHp += 23;
                player.Stat.MaxMp += 11;

                // 레벨업한 사실과, MaxHp, MaxMp 변동 된 것을 '다른' 클라'들'한테 보여준다.

                S_LevelUp levelupPacket = new S_LevelUp();
                levelupPacket.ObjectId = player.Info.ObjectId;
                levelupPacket.StatInfo = new StatInfo();
                levelupPacket.StatInfo.MaxHp = player.Stat.MaxHp;
                levelupPacket.StatInfo.MaxMp = player.Stat.MaxMp;

                room.Broadcast(player.CellPos, levelupPacket);

            }


            // Me (GameROom)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Exp = player.Stat.Exp;
            playerDb.Level = player.Stat.Level;
            playerDb.TotalExp = player.Stat.TotalExp;
            playerDb.StatPoint = player.Stat.StatPoint;

            playerDb.MaxHp = player.Stat.MaxHp;
            playerDb.MaxMp = player.Stat.MaxMp;



            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                    db.Entry(playerDb).Property(nameof(playerDb.Exp)).IsModified = true; // "Hp"
                    db.Entry(playerDb).Property(nameof(playerDb.Level)).IsModified = true; // "Level"
                    db.Entry(playerDb).Property(nameof(playerDb.TotalExp)).IsModified = true; // "TotalExp"
                    db.Entry(playerDb).Property(nameof(playerDb.StatPoint)).IsModified = true; // "StatPoint"

                    db.Entry(playerDb).Property(nameof(playerDb.MaxHp)).IsModified = true; // "TotalExp"
                    db.Entry(playerDb).Property(nameof(playerDb.MaxMp)).IsModified = true; // "StatPoint"


                    //db.SaveChanges();
                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (success)
                    {
                        // Me : 나한테 결과 보내는 부분
                        room.Push(() =>    // 바로 데이터를 받는다고 가정
                        {
                            S_Exp expPacket = new S_Exp();
                            expPacket.Exp = exp;
                            expPacket.LevelUp = LevelUp;
                            expPacket.TotalExp = player.Stat.TotalExp;

                            player.Session.Send(expPacket);

                            // 스텟도 올려주자


                  

                        }
                        );
                    }
                }
            });



        }



        // 콜백을 하지 않기에 room도 받지 않는다.
        public static void DropItem(Player player, Item item, GameRoom room, int dropCount)
        {
            if (player == null || item == null || room == null)
                return;


            ItemDb itemDb = new ItemDb()
            {
                ItemDbId = item.ItemDbId,
                Count = item.Count

            };


            // ★☆먼저 DB에서 버릴 아이템의 진짜 정보를 가지고 온다.
            // ★☆수정하는 동안은 정보를 가져올수 없기 때문이다.

            ItemDb realDb = new ItemDb();

            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    ItemDb item = db.Items.Where(t => t.ItemDbId == itemDb.ItemDbId).FirstOrDefault();

                    realDb = item;

                }
            });


            // DB에서 아이템 삭제
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(itemDb).State = EntityState.Unchanged;
                    db.Entry(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;


                    // 그냥 바로 없애준다. 
                    List<ItemDb> items = db.Items
                        .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                        .ToList();



                    foreach (ItemDb dbt in items)
                    {
                        foreach (Item t in player.Inven.Items.Values)
                        {

                            // 아무 문제가 없으면
                            if (dbt.ItemDbId == t.ItemDbId)
                            {
                                Console.WriteLine("변경전 슬롯 :" + dbt.Slot);
                                dbt.Slot = t.Slot;
                                Console.WriteLine("변경후 슬롯 :" + t.Slot);
                            }
                        }
                    }





                    // 갯수가 0 이면 DB에서 지워주는 작업. ( 단축키도 )

                    if (itemDb.Count == 0)
                    {
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
                    }


                    // 공통 DB 저장
                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (!success)
                    {
                        // 실패했으면 Kick or 로그를 남긴다. 
                    }
                    else
                    {

                        // Me : 나한테 결과 보내는 부분
                        room.Push(() =>    // 바로 데이터를 받는다고 가정
                        {



                            // 클라에 통보
                            S_DropItem DropOkItem = new S_DropItem();
                            DropOkItem.ItemDbId = item.ItemDbId;
                            DropOkItem.Count = itemDb.Count;
                            player.Session.Send(DropOkItem);

                            // 필드에 아이템 떨구기.
                            GameRoom A = room;

                            if (A == null)
                                return;


                            // 실제 버렸던 아이템의 정보를 DB에서 불러온다.

                            //Console.WriteLine("아이템의 정보 : " + temporaryDb.TemplateId);
                            //Item newItem = Item.MakeItem(findItem);


                            // 아이템 드랍

                            DropItem dropItems = ObjectManager.Instance.Add<DropItem>();
                            RewardData rewardData = new RewardData();
                            rewardData.itemId = item.TemplateId;
                            rewardData.count = dropCount;

                            // DB에 있던 아이템 정보를 RewardData에 계승한다.
                            //rewardData.itemInfo = newItem.Info;

                            //Item newItem = Item.MakeItem(temporaryDb);

                            //rewardData.itemInfo.MergeFrom(newItem.Info);


                            //// 원래 아이템의 업그레이드,스텟업 등도 계승
                            Item newItem = Item.MakeItem(realDb);
                            rewardData.itemInfo = newItem.Info;


                            dropItems.Init(player, rewardData, A, 0, throwing: false) ;
                            dropItems.CellPos = new Vector2Int(player.CellPos.x, player.CellPos.y);

                            A.Push(A.EnterGame, dropItems, false); // true 는 랜덤값

                            Console.WriteLine($"아이템 생성 위치 : ({dropItems.CellPos.x},{dropItems.CellPos.y}) 아이템 레이어 {dropItems.Order}");


                            // 스킬(아이템버리는) 패킷보내주기
                            S_Skill skill = new S_Skill() { Info = new SkillInfo() }; // Info도 클래스이기 때문에 새로 만들어주어야한다.

                            skill.Info.SkillId = 9001001;
                            skill.ObjectId = player.Info.ObjectId;
                            skill.Info.MoveDir = player.Info.PosInfo.MoveDir; // 스킬쓴 방향 저장 
                            skill.PosInfo = player.Info.PosInfo; // 스킬쓴 위치 저장

                            A.Broadcast(player.CellPos, skill);

                        });
                    }





                }
            });




        }



        public static void SellItem(Player player, Item item, GameRoom room)
        {
            if (player == null || item == null || room == null)
                return;

            ItemDb itemDb = new ItemDb()
            {
                ItemDbId = item.ItemDbId,
                Count = item.Count,

            };

            Instance.Push(() =>
            {

                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(itemDb).State = EntityState.Unchanged;
                    db.Entry(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;

                    // 그냥 바로 없애준다. 
                    List<ItemDb> items = db.Items
                        .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                        .ToList();

                    // 서버 메모리의 슬롯을 DB에도 갱신
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

                    // 갯수가 0 이면 DB에서 지워준다 ( 단축키도 )
                    if (itemDb.Count == 0)
                    {
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

                        bool success = db.SaveChangesEx(); // 예외 처리

                        if (!success)
                        {
                            // 실패했으면 Kick or 로그를 남긴다. 
                        }
                        else
                        {
                            // 클라에 아이템 버린것 통보
                            S_DropItem DropOkItem = new S_DropItem();
                            DropOkItem.ItemDbId = item.ItemDbId;
                            DropOkItem.Count = item.Count;
                            player.Session.Send(DropOkItem);
                        }


                    }
                    else  // 갯수가 0이 아닐경우 DB 저장만 하고 보내준다.
                    {

                        bool success = db.SaveChangesEx(); // 예외 처리

                        if (!success)
                        {
                            // 실패했으면 Kick or 로그를 남긴다. 
                        }
                        else
                        {
                            // 클라에 아이템 버린것 통보
                            S_DropItem DropOkItem = new S_DropItem();
                            DropOkItem.ItemDbId = item.ItemDbId;
                            DropOkItem.Count = item.Count;
                            player.Session.Send(DropOkItem);
                        }

                    }



                }


            });

        }




        // 스킬을 넣어주는 부분




        public static void GetSkill(Player player, GameRoom room, Item item)
        {
            if (player == null || room == null || item == null)
                return;


            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(item.TemplateId, out itemData);

            if (itemData == null)
                return;

            // 스킬은 SKillData 아니고 Skill
            Skill skillData = null;
            DataManager.SkillDict.TryGetValue(itemData.Enhance, out skillData);

            if (skillData == null)
                return;

            // 추가하기전 한번더 "스킬" 슬롯이 꽉차있는지 확인
            int? emptySlot = player.SkillInven.GetEmptySlot();

            // 추가하기전 한번더 "스킬" 슬롯 꽉차있으면 리턴
            if (emptySlot == null)
                return;

            // 추가하기전 한번더 "스킬" 이미 갖고 있는 스킬인지 확인

            Skills A = player.SkillInven.Find(i => i.SkillId == item.Enhance);

            // 추가하기전 한번더 "스킬" 이미 갖고 있는 스킬이면 리턴
            if (A != null)
                return;

            // 데이터 만들기
            SkillDb skillDb = new SkillDb()
            {
                SkillTemplateId = skillData.id,
                SkillLevel = 1,
                Slot = emptySlot.Value,
                OwnerDbId = player.PlayerDbId,
            };




            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Skills.Add(skillDb);
                    bool success = db.SaveChangesEx();

                    if (success)
                    {

                        // 중복 안되는거면 일단 서버 메모리에 넣어주기.

                        Skills skill = Skills.MakeSkill(skillDb);
                        player.SkillInven.Add(skill);

                        // 클라한테 스킬 창 초기화

                        List<SkillDb> skills = db.Skills
                                                .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                                                .ToList();

                        // 스킬 리스트 패킷을 생성한다.
                        S_SkillList skillListPacket = new S_SkillList();

                        foreach (SkillDb skillDb in skills)
                        {
                            // 스킬을 생성하고,
                            Skills skill_2 = Skills.MakeSkill(skillDb);

                            // 해당 스킬 정보를 패킷정보로 만든후,
                            SkillInfo skillInfo = new SkillInfo()
                            {
                                SkillId = skill_2.SkillId,
                                SkillDbId = skill_2.SkillDbId,
                                Slot = skill_2.Slot,
                                SkillLevel = skill_2.SkillLevel,
                            };

                            // 패킷에 넣어준다.
                            skillListPacket.SkillInfo.Add(skillInfo);
                        }

                        // 정상적으로 스킬 리스트를 보낸다.
                        player.Session.Send(skillListPacket);


                    }
                }
            });

        }



        // 퀘스트 넣어주는 부분


        public static bool GetQuest(Player player, GameRoom room, Quest quest, bool Complete)
        {
            if (player == null || room == null || quest == null)
                return false;

            QuestDb questDb = new QuestDb()
            {
                //QuestDbTemplateId = quest.QuestTemplateId,
                // 퀘스트ID도 +1 해준다.
                QuestDbTemplateId = quest.QuestTemplateId + 1,
                //Status = quest.Status,
                // 일단 status도 +1 해준다.
                Status = quest.Status + 1,
                OwnerDbId = player.PlayerDbId
            };

            // Complete 퀘스트라면 바로 컴플리트를 해준다.
            if (Complete == true)
            {
                questDb.QuestDbTemplateId = quest.QuestTemplateId;
                questDb.Status = -1;
            }




            bool success = false;
            Instance.Push(() =>
            {

                using (AppDbContext db = new AppDbContext())
                {
                    db.Quests.Add(questDb);
                    success = db.SaveChangesEx();

                    if (success)
                    {
                        // 중복 안되는거면 일단 서버 메모리에 넣엊기.

                        Quest quest = Quest.MakeQuest(questDb);
                        player.QuestInven.Add(quest);

                        // 클라이언트 퀘스트 창 초기화

                        List<QuestDb> quests = db.Quests
                                                .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                                                .ToList();

                        // 퀘스트 리스트 패킷을 생성한다.
                        S_QuestList questListPacket = new S_QuestList();

                        foreach (QuestDb questDb in quests)
                        {
                            // 퀘스트를 생성하고,
                            Quest quest_2 = Quest.MakeQuest(questDb);

                            // 해당 퀘스트 정보를 패킷 정보로 만든 후,
                            QuestInfo questInfo = new QuestInfo()
                            {
                                NpcId = quest_2.NpcId,
                                QuestTemplateId = quest_2.QuestTemplateId,
                                Status = quest_2.Status,
                                QuestDbId = quest_2.QuestDbId,
                            };

                            // 패킷에 넣어준다.
                            questListPacket.QuestInfo.Add(questInfo);
                        }

                        // 정상적으로 퀘스트 리스트를 보낸다.
                        player.Session.Send(questListPacket);


                    }


                };

            });
            return true;


        }

        public static void ChangeQuest (Player player, GameRoom room, Quest quest, int statusChange, int newTemplateId)
        {

            if (player == null || room == null || quest == null)
                return;

            QuestDb questDb = new QuestDb();
            questDb.QuestDbId = quest.QuestDbId;
            questDb.Status = statusChange;
            questDb.QuestDbTemplateId = newTemplateId;


            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(questDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                    db.Entry(questDb).Property(nameof(questDb.QuestDbTemplateId)).IsModified = true; // "TemplateId"
                    db.Entry(questDb).Property(nameof(questDb.Status)).IsModified = true; // "Status"

                    //db.SaveChanges();
                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (success)
                    {
                        room.Push(() =>
                        {
                            // 서버 메모리에도 저장을 해주고,
                            Quest PlayerQuest = player.QuestInven.Find(i => i.QuestTemplateId == quest.QuestTemplateId);
                            PlayerQuest.QuestTemplateId = newTemplateId;
                            PlayerQuest.Status = statusChange;

                            // 클라이언트한테도 갱신을 해준다.
               
                            // 퀘스트 리스트 패킷을 생성한다.
                            S_QuestList questListPacket = new S_QuestList();

                            // 대신 DB의 정보를 줄수가없어서, 서버메모리의 있는 퀘스트 리스트를 보내주자.

                            foreach (Quest t in player.QuestInven.Quests.Values)
                            {
                                QuestInfo questInfo = t.Info;

                                // 패킷에 넣어준다.
                                questListPacket.QuestInfo.Add(questInfo);
                            }

                            // 정상적으로 퀘스트 리스트를 보낸다.
                            player.Session.Send(questListPacket);


                        });
                    }
                }
            });




        }



        // 맵 저장하는 부분

        // 웨이터한테 카드 주고, 웨이터가 결제를 하러 가고, 웨이터가 와서 영수증과 카드를 주는.
        // Me (GameRoom) -> You(Db) -> Me (GameRoom)
        public static void SavePlayerPosition_AllInOne(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;


            // Me (GameROom)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.Stat.Hp;
            playerDb.Mp = player.Stat.Mp;

            // Room 이 없는 경우도 생겨서
            //playerDb.Map = player.Room.RoomId;
            playerDb.Map = room.RoomId;
            playerDb.PosX = player.CellPos.x;
            playerDb.PosY = player.CellPos.y;




            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(playerDb).Property(nameof(playerDb.Map)).IsModified = true; // "Map"
                    db.Entry(playerDb).Property(nameof(playerDb.PosX)).IsModified = true; // "PosX"
                    db.Entry(playerDb).Property(nameof(playerDb.PosY)).IsModified = true; // "PosY"





                    //db.SaveChanges();
                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (success)
                    {
                        // Me : 나한테 결과 보내는 부분
                        room.Push(() =>    // 바로 데이터를 받는다고 가정
                        Console.WriteLine($"① Map Saved ({playerDb.Map}(,{playerDb.PosX},{playerDb.PosY}))")
                        );
                    }
                }
            });

        }




        public static void SlotChange(Player player, GameRoom room, Item itemA, Item itemB)
        {
            if (player == null || itemA == null || itemB == null || room == null)
                return;

            ItemDb itemDb_A = new ItemDb()
            {
                ItemDbId = itemA.ItemDbId,
                Slot = itemA.Slot
            };

            ItemDb itemDb_B = new ItemDb()
            {
                ItemDbId = itemB.ItemDbId,
                Slot = itemB.Slot
            };


            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    //// 슬롯만 바꾼다.
                    //db.Entry(itemDb_A).State = EntityState.Unchanged;
                    //db.Entry(itemDb_A).Property(nameof(ItemDb.Count)).IsModified = true;

                    //db.Entry(itemDb_B).State = EntityState.Unchanged;
                    //db.Entry(itemDb_B).Property(nameof(ItemDb.Count)).IsModified = true;

                    // 그냥 바로 없애준다. 
                    List<ItemDb> items = db.Items
                        .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                        .ToList();

                    foreach (ItemDb dbt in items)
                    {
                        if (dbt.ItemDbId == itemDb_A.ItemDbId)
                        {
                            dbt.Slot = itemDb_A.Slot;
                        }
                        else if (dbt.ItemDbId == itemDb_B.ItemDbId)
                        {
                            dbt.Slot = itemDb_B.Slot;
                        }
                    }

                    // 공통 DB 저장
                    bool success = db.SaveChangesEx(); // 예외 처리




                    if (!success)
                    {
                        // 실패했으면 Kick or 로그를 남긴다. 
                    }
                    else
                    {

                        //// 클라한테 패킷 보내기
                        //S_ItemList itemListPacket = new S_ItemList();



                        //foreach (ItemDb itemDb in items)
                        //{
                        //    // 아이템 디비정보를 이용해서 아이템을 만들어준다.
                        //    Item item = Item.MakeItem(itemDb);
                        //    // 아무 문제가 없으면
                        //    if (item != null)
                        //    {


                        //        // 패킷에 해당 아이템 정보를 넣어준다.
                        //        ItemInfo info = new ItemInfo();

                        //        info.MergeFrom(item.Info);

                        //        itemListPacket.Items.Add(info);
                        //    }
                        //}

                        //room.Push(() =>
                        //{

                        //    player.Session.Send(itemListPacket);

                        //});
                    }
                }
            });

        }





        public static void SlotChange_Single(Player player, GameRoom room, Item itemA)
        {
            if (player == null || itemA == null  || room == null)
                return;

            ItemDb itemDb_A = new ItemDb()
            {
                ItemDbId = itemA.ItemDbId,
                Slot = itemA.Slot
            };

            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    //// 슬롯만 바꾼다.
                    //db.Entry(itemDb_A).State = EntityState.Unchanged;
                    //db.Entry(itemDb_A).Property(nameof(ItemDb.Count)).IsModified = true;

                    //db.Entry(itemDb_B).State = EntityState.Unchanged;
                    //db.Entry(itemDb_B).Property(nameof(ItemDb.Count)).IsModified = true;

                    // 그냥 바로 없애준다. 
                    List<ItemDb> items = db.Items
                        .Where(i => i.OwnerDbId == player.PlayerDbId) // 메모리에 들고 있는 플레이어 ID. 해킹걱정X
                        .ToList();

                    foreach (ItemDb dbt in items)
                    {
                        if (dbt.ItemDbId == itemDb_A.ItemDbId)
                        {
                            dbt.Slot = itemDb_A.Slot;
                        }
                    }

                    // 공통 DB 저장
                    bool success = db.SaveChangesEx(); // 예외 처리


                    if (!success)
                    {
                        // 실패했으면 Kick or 로그를 남긴다. 
                    }
                    else
                    {

                      
                    }
                }
            });

        }





        // 경험치 올려주는 부분
        public static void GetJob(Player player, int jobCode, GameRoom room)
        {
            if (player == null || room == null)
                return;

            bool LevelUp = false;


            player.Stat.Job = jobCode;

            // 레벨업한 사실과, MaxHp, MaxMp 변동 된 것을 '다른' 클라'들'한테 보여준다.

            S_LevelUp levelupPacket = new S_LevelUp();
            levelupPacket.ObjectId = player.Info.ObjectId;
            levelupPacket.StatInfo = new StatInfo();
            levelupPacket.StatInfo.Job = player.Stat.Job;
            levelupPacket.StatInfo.MaxHp = player.Stat.MaxHp;
            levelupPacket.StatInfo.MaxMp = player.Stat.MaxMp;

            room.Broadcast(player.CellPos, levelupPacket);

            // Me (GameROom)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Job = player.Stat.Job;


            // You : 야 장부담당, 니가 해주고, 마지막 결과만 나한테 보내줘
            // 장부는 Program.cs 에서 만들어줘야함.
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {

                    db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
                    db.Entry(playerDb).Property(nameof(playerDb.Job)).IsModified = true; // "Hp"


                    //db.SaveChanges();
                    bool success = db.SaveChangesEx(); // 예외 처리

                    if (success)
                    {
                        // Me : 나한테 결과 보내는 부분
                        room.Push(() =>    // 바로 데이터를 받는다고 가정
                        {


                        }
                        );
                    }
                }
            });



        }








    }




}




