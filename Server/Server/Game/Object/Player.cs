using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }

        public VisionCube Vision { get; private set; }

        public Inventory Inven { get; private set; } = new Inventory();
        public KeySettings Keys { get; private set; } = new KeySettings();
        public SkillInventory SkillInven { get; private set; } = new SkillInventory();
        public QuestInventory QuestInven { get; private set; } = new QuestInventory();


        public int WeaponDamage { get; private set; }
        public int ArmorDefence { get; private set; }

        public override int TotalAttack { get { return Stat.Attack + itemWAtk; } }
        //public override int TotalDefence { get { return ArmorDefence; } }
        public override int TotalDefence { get { return itemWDef; } }

        public override int MaxAttack { get { return (int)((((TotalStr * 4.0) + TotalDex) /100)*itemWAtk); } }
        public override int MinAttack { get { return (int)((((TotalStr * 4*0.9*0.4) + TotalDex) / 100) * itemWAtk); } }

        public int itemStr { get; private set; }
        public int itemDex { get; private set; }
        public int itemInt { get; private set; }
        public int itemLuk { get; private set; }
        public int itemHp { get; private set; }
        public int itemMp { get; private set; }        
        public int itemWAtk { get; private set; }
        public int itemMAtk { get; private set; }
        public int itemWDef { get; private set; }
        public int itemMDef { get; private set; }
        public int itemSpeed { get; private set; }
        public int itemAtkSpeed { get; private set; }
        public int itemWPnt{ get; private set; }
        public int itemMPnt { get; private set; }


        public  int TotalStr { get { return Stat.Str + itemStr; } }
        public  int TotalDex { get { return Stat.Dex + itemDex; } }
        public  int TotalInt { get { return Stat.Int + itemInt; } }
        public  int TotalLuk { get { return Stat.Luk + itemLuk; } }
        public  int TotalHp { get { return Stat.MaxHp + itemHp; } }
        public  int TotalMp { get { return Stat.MaxMp + itemMp; } }
        //public int TotalWAtk { get { return Stat.WAtk + itemStr; } }
        //public int TotalMAtk { get { return Stat.Str + itemStr; } }
        //public int TotalWDef { get { return Stat.Str + itemStr; } }
        //public int TotalMDef { get { return Stat.Str + itemStr; } }
        public  float TotalSpeed { get { return Stat.Speed + itemSpeed; } }
        //public int TotalAtkSpeed { get { return Stat.AtkSp + itemJump; } }
        //public int TotalWPnt { get { return Stat.Str + itemStr; } }
        //public int TotalMPnt { get { return Stat.Str + itemStr; } }

 

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionCube(this);

        }


        // 실시간 속도 구하는 부분
        IJob _job;
        int updateTime = 200;


        public override void Update()
        {
            switch (State)
            {

                case CreatureState.Moving:
                    UpdateMoving();
                    break;

            }
            // 5프레임 ( 0.2초마다 한번씩 Update)

            if (Room != null)
                _job = Room.PushAfter(updateTime, Update);

        }


        // 플레이어 스킬 쿨타임
        public bool SkillCool = false;
        public bool ConsumeCool = false;
        public bool ShortKeyCool = false;
        public bool TeleportCool = false;

        // 스킬 쓴 직후 바로 이동 안되게 하는 쿨탕미

        public bool SkillWalkCool = false;


        public float yes;

        public void SkillCooltime()
        {
            SkillCool = false;
            //Console.WriteLine("끝 쿨 시간 : " + Environment.TickCount64);
            //Console.WriteLine("시간차이 : " + (Environment.TickCount64- yes));
        }
        public void ConsumeCooltime()
        {
            ConsumeCool = false;
        }
        public void ShortKeyCooltime()
        {
            ShortKeyCool = false;
        }
        public void TeleportCooltime()
        {
            TeleportCool = false;
        }

        public void SkillWalkCooltime()
        {
            SkillWalkCool = false;
        }

        protected virtual void UpdateMoving()
        {



            if (_nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(32 * 1000 / Speed);

            // Speed : 1초 동안 몇칸을 움직이냐는 개념
            // moveTick = 작을수록 빠른거임. 

            _nextMoveTick = Environment.TickCount64 + moveTick;

    
        }

        public override int OnDamaged(GameObject attacker, int damage, int skillId, int shot = 1, GameObject Owner = null)
        {

            return base.OnDamaged(attacker, damage, skillId, shot, Owner);

            //Console.WriteLine($"TODO : damage {damage}");



        }

        public override void OnDead(GameObject attacker, int damage)
        {
            //base.OnDead(attacker);

            if (Room == null)
                return;



            // 우선 죽음 패킷 보내고
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            diePacket.Damage = damage;
            Room.Broadcast(CellPos, diePacket);


            // Room 임시저장
            GameRoom room = Room;

            // 방에서 나가게한다.
            //room.LeaveGame(Id);
            //room.Push(room.LeaveGame,Id); // => JobQueue 화 : 바로 실행되지 않을 수 있어 문제가 있음. -> 처리를해줘야함. 처리를해줌
            room.LeaveGame(Id); // 따라서 바로 실행시켜준다.



            // 초기화를 시킨다
            Stat.Hp = TotalHp;
            Stat.Mp = TotalMp;

            PosInfo.State = CreatureState.Idle;

            PosInfo.MoveDir = MoveDir.Down;
            //PosInfo.PosX = 0;
            //PosInfo.PosY = 0;

            // 임시 마을로 이동
            //Stat.Map = 4;

            Data.MapInfoData mapData = null;

            // 스킬 데이터가 없으면 return 
            if (DataManager.MapDict.TryGetValue(Stat.Map, out mapData) == false)            
                return;
        
            //Stat.Map = 10100031;
            Stat.Map = mapData.townId;

            PosInfo.PosX = 1;
            PosInfo.PosY = -2;

            // 우선 이동 패킷 보내고
            S_MoveMap moveMapPacket = new S_MoveMap();
            moveMapPacket.StatInfo = Stat;
            Session.Send(moveMapPacket);

            GameLogic.Instance.Push(() =>
            {
                GameRoom destRoom = GameLogic.Instance.Find(Stat.Map);
                destRoom.Push(destRoom.EnterGame, this, false); // => JobQueue 화
            });

        }

        // 게임에서 나갈때만 저장을 해준다.
        public void OnLeaveGame()
        {
            // TODO
            // DB 연동 ?
            // 0) 피가 깎일 때마다 DB 접근할 필요가 있을까? / DB는 은근히 부하를 먹으므로 최소화하는게 좋다.
            // 1) 서버 다운되면 아직 저장되지 않은 정보 날아감.
            // 2) 코드 흐름을 다 막아버린다 !!!!!
            // => 비동기(Async) 방법 사용?
            // => 다른 스레드로 DB 일감을 던져버리면 되지 않을까?
            // (BUT) => 결과를 받아서 이어서 처리를 해야 하는 경우가 많다. ( ex. 아이템이 필드에 나오거나, 강화 결과 )
            // ex. 아이템 생성

            // 서빙 담당
            // 결제 담당

            //using (AppDbContext db = new AppDbContext())
            //{
            //    // 첫번째 캐릭터?

            //    // DB 2번 접근
            //    //PlayerDb playerDb = db.Players.Find(PlayerDbId);
            //    //playerDb.Hp = Stat.Hp;
            //    //db.SaveChanges();

            //    // DB 1번 접근
            //    PlayerDb playerDb = new PlayerDb();
            //    playerDb.PlayerDbId = PlayerDbId;
            //    playerDb.Hp = Stat.Hp;
            //    db.Entry(playerDb).State = EntityState.Unchanged; // Hp만 변경되게 해서 효율적으로 처리한다.
            //    db.Entry(playerDb).Property(nameof(playerDb.Hp)).IsModified = true; // "Hp"

            //    //db.SaveChanges();
            //    db.SaveChangesEx(); // 예외 처리

            //    // 바로 데이터를 받는다고 가정
            //    Console.WriteLine($"Hp Saved ({playerDb.Hp})"); 
            //}

            // DB Transacton 으로 Me도 바로 처리도 하고, 데이터도 Job으로 천천히 처리하고 결과도 받고.

            // All in One <-- 난 이게 더 편한것 같아서./.

            DbTransaction.SavePlayerStatus_AllInOne(this, Room);
            DbTransaction.SavePlayerPosition_AllInOne(this, Room);



            // 맵이동될때도 실행이되서 문제임. => 결국 로그아웃될때만 이거되게 해야함.여기서 뺴야함.

            //using (SharedDbContext shared = new SharedDbContext())
            //{
            //    // 토큰 업데이트하거나 추가하는 부분
            //    TokenDb tokenDb = shared.Tokens.Where(t => t.AccountName == Session.AccountName).FirstOrDefault();

            //    if (tokenDb != null)
            //    {
            //        tokenDb.IsLogin = false;
            //    }
            //    shared.SaveChangesEx();
            //}



        }

        public void HandleEquipItem(C_EquipItem equipPacket)
        {

            Item item = Inven.Get(equipPacket.ItemDbId);

            if (item == null)
                return;

            // 소비 부분 리턴
            if (item.ItemType == ItemType.Consumable)
                return;

            // 착용 요청이라면, 겹치는 부위 해제

            if (equipPacket.Equipped)
            {
                Item unequipItem = null;

                if (item.ItemType == ItemType.Weapon)
                {
                    unequipItem = Inven.Find(
                        i => i.Equipped && i.ItemType == ItemType.Weapon);
                }
                else if (item.ItemType == ItemType.Armor)
                {
                    ArmorType armorType = ((Armor)item).ArmorType;
                    unequipItem = Inven.Find(
                        i => i.Equipped && i.ItemType == ItemType.Armor
                            && ((Armor)i).ArmorType == armorType);
                }


                if (unequipItem != null)
                {

                    // DB 연동 
                    // 1. 메모리 선 적용
                    unequipItem.Equipped = false;

                    // DB에 알림
                    DbTransaction.EquipItemNoti(this, unequipItem);


                    // 클라에 통보

                    S_EquipItem equipOkItem = new S_EquipItem();
                    equipOkItem.ItemDbId = unequipItem.ItemDbId;
                    equipOkItem.Equipped = unequipItem.Equipped;
                    Session.Send(equipOkItem);
                }
            }

            {
                // DB 연동 
                // 1. 메모리 선 적용
                item.Equipped = equipPacket.Equipped;

                // DB에 알림
                DbTransaction.EquipItemNoti(this, item);


                // 장비 착용에 대한 정보는 서버에만 일단 기록하고 나중에 캐릭터가 종료하면 DB에 저장.

                if (item.Equipped == true) // 아이템 착용했을 경우
                {
                    switch (item.ItemType)
                    {
                        case ItemType.Weapon:

                            Stat.RightHand = item.TemplateId;
                            break;
                        case ItemType.Armor:

                            if (((Armor)item).ArmorType == ArmorType.Helmet)
                                Stat.Helmet = item.TemplateId;
                            else if (((Armor)item).ArmorType == ArmorType.Armor)
                                Stat.Shirts = item.TemplateId;
                            else if (((Armor)item).ArmorType == ArmorType.Boots)
                                Stat.Shoes = item.TemplateId;
                            else if (((Armor)item).ArmorType == ArmorType.Pants)
                                Stat.Pants = item.TemplateId;
                            else if (((Armor)item).ArmorType == ArmorType.Shield)
                                Stat.LeftHand = item.TemplateId;
                            break;
                    }
                }
                else if (item.Equipped == false) // 아이템 착용하지 않은 것일 경우
                {
                    switch (item.ItemType)
                    {
                        case ItemType.Weapon:

                            Stat.RightHand = -1;
                            break;
                        case ItemType.Armor:

                            if (((Armor)item).ArmorType == ArmorType.Helmet)
                                Stat.Helmet = -1;
                            else if (((Armor)item).ArmorType == ArmorType.Armor)
                                Stat.Shirts = -1;
                            else if (((Armor)item).ArmorType == ArmorType.Boots)
                                Stat.Shoes = -1;
                            else if (((Armor)item).ArmorType == ArmorType.Pants)
                                Stat.Pants = -1;
                            else if (((Armor)item).ArmorType == ArmorType.Shield)
                                Stat.LeftHand = -1;
                            break;
                    }
                }

                // 세션을 보낸 클라에 통보

                S_EquipItem equipOkItem = new S_EquipItem();
                equipOkItem.ItemDbId = equipPacket.ItemDbId;
                equipOkItem.Equipped = equipPacket.Equipped;
                Session.Send(equipOkItem);



                // 장비 변동 된 것을 '다른' 클라'들'한테 보여준다.

                S_ChangeStat changePacket = new S_ChangeStat();
                changePacket.StatInfo = new StatInfo();
                changePacket.StatInfo.Helmet = Stat.Helmet;
                changePacket.StatInfo.Shirts = Stat.Shirts;
                changePacket.StatInfo.RightHand = Stat.RightHand;
                changePacket.StatInfo.LeftHand = Stat.LeftHand;
                changePacket.StatInfo.Pants = Stat.Pants;
                changePacket.StatInfo.Shoes = Stat.Shoes;
                changePacket.ObjectId = Info.ObjectId;

                Room.Broadcast(CellPos, changePacket);
                 
            }

            // Stat을 리프레쉬????
            RefreshAdditionalStat();
        }

        public void RefreshAdditionalStat()
        {
            WeaponDamage = 0;
            ArmorDefence = 0;

            itemStr = 0;
            itemDex = 0;
            itemInt = 0;
            itemLuk = 0;
            itemHp = 0;
            itemMp = 0;
            itemWAtk = 0;
            itemMAtk = 0;
            itemWDef = 0;
            itemMDef = 0;
            itemSpeed = 0;
            itemAtkSpeed = 0;
            itemWPnt = 0;
            itemMPnt = 0;
    


            foreach (Item item in Inven.Items.Values)
            {
                if (item.Equipped == false)
                    continue;

                switch (item.ItemType)
                {
                    case ItemType.Weapon:
                        //WeaponDamage += ((Weapon)item).Damage;
                        break;
                    case ItemType.Armor:
                        //ArmorDefence += ((Armor)item).Defence;
                        break;

                }

                itemStr += item.Str;
                itemDex += item.Dex;
                itemInt += item.Int;
                itemLuk += item.Luk;
                itemHp += item.Hp;
                itemMp += item.Mp;
                itemWAtk += item.WAtk;
                itemMAtk += item.MAtk;
                itemWDef += item.WDef;
                itemMDef += item.MDef;
                itemSpeed += item.Speed;
                itemAtkSpeed += item.AtkSpeed;
                itemWPnt += item.WPnt;
                itemMPnt += item.MPnt;

            }


            


        }



        public virtual void MoveMap(int destMap, int destPosX, int destPosY)
        {
            if (Room == null)
                return;


            // Room 임시저장
            GameRoom currentRoom = Room;

            // 방에서 나가게한다.
            //room.LeaveGame(Id);
            //room.Push(room.LeaveGame,Id); // => JobQueue 화 : 바로 실행되지 않을 수 있어 문제가 있음. -> 처리를해줘야함. 처리를해줌
            currentRoom.LeaveGame(Id); // 따라서 바로 실행시켜준다.

            Player TargetPlayer = this;


            TargetPlayer.Stat.Map = destMap;
            TargetPlayer.PosInfo.PosX = destPosX;
            TargetPlayer.PosInfo.PosY = destPosY;



            // 우선 이동 패킷 보내고
            S_MoveMap moveMapPacket = new S_MoveMap();
            moveMapPacket.StatInfo = Stat;
            Session.Send(moveMapPacket);

            GameLogic.Instance.Push(() =>
            {
                GameRoom destRoom = GameLogic.Instance.Find(Stat.Map);
                destRoom.Push(destRoom.EnterGame, TargetPlayer, false); // => JobQueue 화
            });
        }



        public void HandleUseItem(C_UseItem UsePacket)
        {

            Item item = Inven.Get(UsePacket.ItemDbId);

            if (item == null)
                return;

            // 소비 부분이 아니라면 리턴
            if (item.ItemType != ItemType.Consumable)
                return;


            // 스킬 북일경우
            if (item.TemplateId >= 91000 && item.TemplateId < 99999)
            {


                // 슬롯이 꽉차있는지 확인
                int? emptySlot = SkillInven.GetEmptySlot();

                // 슬롯 꽉차있으면 리턴
                if (emptySlot == null)
                    return;


                ItemData itemData = null;
                DataManager.ItemDict.TryGetValue(item.TemplateId, out itemData);


                // 이미 갖고 있는 스킬인지 확인

                Skills A = SkillInven.Find(i => i.SkillId == itemData.Enhance);

                // 이미 갖고 있는 스킬이면 리턴
                if (A != null)
                    return;


                // 스킬을 갖고 있지 않기에, 일단 갯수 줄여주고, 스킬 얻기 시도.
                {
                    // DB 연동 
                    // 1. 메모리 선 적용
                    item.Count -= 1;

                    if (item.Count <= 0)
                    {
                        Inven.Delete(item);
                    }

                    // DB에 알림
                    DbTransaction.UseItemNoti(this, item);

                    // 클라에 통보

                    S_UseItem useOkItem = new S_UseItem();
                    useOkItem.ItemDbId = item.ItemDbId;
                    useOkItem.Count = item.Count;
                    Session.Send(useOkItem);

                    DbTransaction.GetSkill(this, Room, item);
                }

                return;
            }

            // 타겟
            GameObject target = null;



            //// 마나 올리기 전에 풀피면 리턴
            //if (Stat.Mp == TotalMp)
            //    return;

            {
                // DB 연동 
                // 1. 메모리 선 적용
                item.Count -= 1;

                ItemData itemData = null;
                DataManager.ItemDict.TryGetValue(item.TemplateId, out itemData);



                // 파란포션이라면
                if (itemData.Mp > 0)
                {

                    // 마나 올리기 전에 풀마나면 리턴
                    if (Stat.Mp == TotalMp)
                        return;

                    int MpConsume = itemData.Mp;

                    // 최대 마나보다 높아지면 최대마나로.
                    Stat.Mp = Math.Min(Stat.Mp + MpConsume, Stat.MaxMp);

                    // 브로드 캐스팅
                    S_ChangeMp changePacket = new S_ChangeMp();
                    changePacket.ObjectId = Id;
                    changePacket.Mp = Stat.Mp;
                    changePacket.Damage = MpConsume;
                    Room.Broadcast(CellPos, changePacket);
                    Console.WriteLine($"MpPotion!! + {item.Mp} ");
                }

                if (itemData.Hp > 0)
                {
                    // 체력 올리기 전에 풀체력이면 리턴
                    if (Stat.Hp == TotalHp)
                        return;

                    // Int만큼 체력을 올려준다.
                    target = this;
                    target.OnHealed(this, itemData.Hp);
                    Console.WriteLine($"HpPotion!! + {item.Hp} ");
                }

                if (item.Count <= 0)
                {
                    Inven.Delete(item);
                }

                // DB에 알림
                DbTransaction.UseItemNoti(this, item);


                // 클라에 통보

                S_UseItem useOkItem = new S_UseItem();
                useOkItem.ItemDbId = item.ItemDbId;
                useOkItem.Count = item.Count;
                Session.Send(useOkItem);


            }

            // Stat을 리프레쉬???
            RefreshAdditionalStat();


            // 마을 귀환 주문서인지 확인
            {
                ItemData itemData = null;
                DataManager.ItemDict.TryGetValue(item.TemplateId, out itemData);

                if (itemData.isTown != 0)
                {
                    switch (itemData.isTown)
                    {

                        case -1: // 마을 귀환 주문서

                            Data.MapInfoData mapData = null;

                            // 맵 데이터가 없으면 return 
                            if (DataManager.MapDict.TryGetValue(Stat.Map, out mapData) == false)
                                break;

                            // 맵이 다를 경우에만
                            if (Stat.Map != mapData.townId)
                            {
                                // 맵 이동 시킨다.
                                MoveMap(mapData.townId, 0, 0);
                            }

                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                    }

                }
            }


        }


        public void HandleKeySetting(C_KeySetting keysettingPacket)
        {

            Key key = Keys.Get_KeyValue(keysettingPacket.Key);

            Console.WriteLine($"해당 키값 : {key}");


            // 해당 키에 다른 액션이 없는 경우
            if (key == null)
            {
                // 파란포션이 아니라면 return
                if (keysettingPacket.Action == 99999 || keysettingPacket.Action <90000)
                    return;

                // key 값이 없다면 새로 생성.

                S_KeySetting AddServerkeysettingPacket = new S_KeySetting();



                // 기존에 단축키에 등록 되어있는 Action 인지 확인하고, 그 단축키는 지워준다.

                {
                    Key DeleteAction_new = null;

                    DeleteAction_new = Keys.Find(i => i.Action == keysettingPacket.Action);

                    if (DeleteAction_new != null)
                    {
                        // 메모리에서는 지워주고,
                        Keys.Delete(DeleteAction_new);

                        DeleteAction_new.Action = -1;

                        // Noti 에 가서 Action = -1 이면 지워지게 만든다,.
                        DbTransaction.KeySettingNoti(this, DeleteAction_new);

                        KeySettingInfo keysettingInfo_new = new KeySettingInfo();
                        keysettingInfo_new.Key = DeleteAction_new.KeyValue;
                        keysettingInfo_new.KeyDbId = DeleteAction_new.KeyDbId;
                        keysettingInfo_new.Type = DeleteAction_new.Type;
                        keysettingInfo_new.OwnerDbId = this.PlayerDbId;
                        keysettingInfo_new.Action = DeleteAction_new.Action;

                        // 클라에서는 -1 로 지워졋따는 표시를 주고 클라에서도 지워지게 만든다.
                        AddServerkeysettingPacket.KeySettingInfo.Add(keysettingInfo_new);
                    }
                }



                using (AppDbContext db = new AppDbContext())
                {
                    // key만 확인하는 것이 아니라 OwnerDbId 도 확인해줘야한다.
                    KeySettingDb findKeys = db.Keys
                        .Where(p => p.key == keysettingPacket.Key && p.OwnerDbId == PlayerDbId).FirstOrDefault();

                    if (findKeys != null)
                    {
                        // key가 없어서 여기온건데, 있는거라면 오류니까 로그를 뱉는다.
                        Console.WriteLine("키셋팅 오류!!");
                    }
                    else
                    {
                        KeySettingDb newKeyDb = new KeySettingDb()
                        {
                            key = keysettingPacket.Key,
                            type = keysettingPacket.Type,
                            action = keysettingPacket.Action,
                            OwnerDbId = this.PlayerDbId,
                        };

                        db.Keys.Add(newKeyDb);


                        bool success = db.SaveChangesEx();
                        if (success == false) // 제대로 안되면 return 하거나 클라한테 말해준다.
                            return;


                        // 메모리에 추가

                        Key newkey = new Key()
                        {
                            KeyDbId = newKeyDb.KeySettingDbId,
                            KeyValue = newKeyDb.key,
                            Type = newKeyDb.type,
                            Action = newKeyDb.action,

                        };


                        // 메모리에도 들고 있는다. ( 이유 : DB 접근하는 빈도를 최소화하기위해 )
                        Keys.Add(newkey);


                        // 클라이언트에 만들었따고 전송

                        KeySettingInfo keysettingInfo = new KeySettingInfo();
                        keysettingInfo.Key = newkey.KeyValue;
                        keysettingInfo.KeyDbId = newkey.KeyDbId;
                        keysettingInfo.Type = newkey.Type;
                        keysettingInfo.OwnerDbId = this.PlayerDbId;
                        keysettingInfo.Action = newkey.Action;


                        AddServerkeysettingPacket.KeySettingInfo.Add(keysettingInfo);



                    }
                }

                // 모은것 패킹
                Session.Send(AddServerkeysettingPacket);



                return;
            }

            // 이미 해당 키에 다른 액션이 있는 경우
            else
            {

                // 같은 키면 ( 그냥 그자리 그대로라면 ) 그냥 리턴한다.
                if (key.Action == keysettingPacket.Action)
                    return;


                // 파란포션이 아니라면 return
                if (keysettingPacket.Action == 99999 || keysettingPacket.Action < 90000)
                    return;


                S_KeySetting ServerkeysettingPacket = new S_KeySetting();



                // 원래 있떤 키의 Action과 Event만 지워준다.

                // 기존에 단축키에 등록 되어있는지 확인하고, 그 단축키는 지워준다.

                Key DeleteAction = null;

                DeleteAction = Keys.Find(i => i.Action == keysettingPacket.Action);

                if (DeleteAction != null)
                {
                    // 메모리에서는 지워주고,
                    Keys.Delete(DeleteAction);

                    DeleteAction.Action = -1;

                    // Noti 에 가서 Action = -1 이면 지워지게 만든다,.
                    DbTransaction.KeySettingNoti(this, DeleteAction);

                    KeySettingInfo keysettingInfo = new KeySettingInfo();
                    keysettingInfo.Key = DeleteAction.KeyValue;
                    keysettingInfo.KeyDbId = DeleteAction.KeyDbId;
                    keysettingInfo.Type = DeleteAction.Type;
                    keysettingInfo.OwnerDbId = this.PlayerDbId;
                    keysettingInfo.Action = DeleteAction.Action;

                    // 클라에서는 -1 로 지워졋따는 표시를 주고 클라에서도 지워지게 만든다.
                    ServerkeysettingPacket.KeySettingInfo.Add(keysettingInfo);
                }

                // 새로 변경된 단축키의 값을 넣어준다. ( 그냥 Action이랑 Type만 변경해준다. )
                {
                    key.Action = keysettingPacket.Action;
                    key.Type = keysettingPacket.Type;

                    // DB에 알림
                    DbTransaction.KeySettingNoti(this, key);

                    KeySettingInfo keysettingInfo = new KeySettingInfo();
                    keysettingInfo.Key = key.KeyValue;
                    keysettingInfo.KeyDbId = key.KeyDbId;
                    keysettingInfo.Type = key.Type;
                    keysettingInfo.OwnerDbId = this.PlayerDbId;
                    keysettingInfo.Action = key.Action;

                    ServerkeysettingPacket.KeySettingInfo.Add(keysettingInfo);

                }

                Session.Send(ServerkeysettingPacket);



            }

           


        }

        public void HandleDropItem(C_DropItem dropItemPacket)
        {


            Item item = Inven.Get(dropItemPacket.ItemDbId);

            // 내가 가지고 있는 아이템인지 확인
            if (item == null)
                return;

            // 장비하고 있는 상태라면 리턴
            if (item.Equipped ==true)
                return;


            // 갯수가 음수이거나 숫자가 아닌지 확인 Count 자체가 Int라서 모르겠네..  
            if (dropItemPacket.Count < 1)
                return;

            // 키셋팅 값 지워야함.
            // DB지워야 함.
            // 갯수 확인하는 부분

            int originalItemCount = item.Count;

            // 아이템 타입이 소비거나 기타일 때
            if(item.ItemType == ItemType.Consumable || item.ItemType == ItemType.Etc)
            {
                item.Count -= dropItemPacket.Count;

                // 0을 포함했어야 했다.
                if (item.Count <= 0)
                {
                    item.Count = 0;
                    Inven.Delete(item);
                }
            }
            // 포션류가 아닌 경우 무조건 서버 메모리에서 지워주고 Drop을 해준다.
            else
            {
                item.Count = 0;
                Inven.Delete(item);
            }

            int newItemCount = item.Count;

            int dropCount = (originalItemCount - newItemCount);
            // DB에서 카운트 깎아주기.
            DbTransaction.DropItem(this, item, Room, dropCount);

        }


 







    }
}
