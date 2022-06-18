using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    // 서버에서 복붙

    // 진짜 서버 메모리에 들어가 있는 아이템
    // Proto를 참고하여 get set 을 만든다.

    public class Item
    {
        // 실질적으로 이것으로 관리를 하지만,
        public ItemInfo Info { get; } = new ItemInfo();

        // 여기서도 각각 통해서 갖고온다.
        public int ItemDbId
        {
            get { return Info.ItemDbId; }
            set { Info.ItemDbId = value; }
        }

        public int TemplateId
        {
            get { return Info.TemplateId; }
            set { Info.TemplateId = value; }
        }

        public int Count
        {
            get { return Info.Count; }
            set { Info.Count = value; }
        }
        

        public int Slot
        {
            get { return Info.Slot; }
            set{ Info.Slot = value;; }
        }


        public bool Equipped
        {
            get { return Info.Equipped; }
            set { Info.Equipped = value;}
        }


        public int ReqStr
        {
            get { return Info.ReqStr; }
            set { Info.ReqStr = value; }
        }
        public int ReqDex
        {
            get { return Info.ReqDex; }
            set { Info.ReqDex = value; }
        }

        public int ReqInt
        {
            get { return Info.ReqInt; }
            set { Info.ReqInt = value; }
        }

        public int ReqLuk
        {
            get { return Info.ReqLuk; }
            set { Info.ReqLuk = value; }
        }
        public int ReqLev
        {
            get { return Info.ReqLev; }
            set { Info.ReqLev = value; }
        }
        public int ReqPop
        {
            get { return Info.ReqPop; }
            set { Info.ReqPop = value; }
        }
        public int UpgradeSlot
        {
            get { return Info.UpgradeSlot; }
            set { Info.UpgradeSlot = value; }
        }
        public int Str
        {
            get { return Info.Str; }
            set { Info.Str = value; }
        }
        public int Dex
        {
            get { return Info.Dex; }
            set { Info.Dex = value; }
        }
        public int Int
        {
            get { return Info.Int; }
            set { Info.Int = value; }
        }
        public int Luk
        {
            get { return Info.Luk; }
            set { Info.Luk = value; }
        }
        public int Hp
        {
            get { return Info.Hp; }
            set { Info.Hp = value; }
        }
        public int Mp
        {
            get { return Info.Mp; }
            set { Info.Mp = value; }
        }
        public int WAtk
        {
            get { return Info.WAtk; }
            set { Info.WAtk = value; }
        }
        public int MAtk
        {
            get { return Info.MAtk; }
            set { Info.MAtk = value; }
        }
        public int WDef
        {
            get { return Info.WDef; }
            set { Info.WDef = value; }
        }
        public int MDef
        {
            get { return Info.MDef; }
            set { Info.MDef = value; }
        }
        public int Speed
        {
            get { return Info.Speed; }
            set { Info.Speed = value; }
        }
        public int AtkSpeed
    {
            get { return Info.AtkSpeed; }
            set { Info.AtkSpeed = value; }
        }
        public int Durability
        {
            get { return Info.Durability; }
            set { Info.Durability = value; }
        }
        public int Enhance
        {
            get { return Info.Enhance; }
            set { Info.Enhance = value; }
        }
        public int WPnt
        {
            get { return Info.WPnt; }
            set { Info.WPnt = value; }
        }
        public int MPnt
        {
            get { return Info.MPnt; }
            set { Info.MPnt = value; }
        }



    public ItemType ItemType { get; private set; }
        public bool Stackable { get; protected set; }

        public Item(ItemType itemType)
        {
            ItemType = itemType;
        }



        // 아이템 생성하는 부분

        public static Item MakeItem(ItemInfo itemInfo)
        {
            Item item = null;

            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(itemInfo.TemplateId, out itemData);

            if (itemData == null)
                return null;


            // ItemType 별로 생성을 해준다.
            switch (itemData.itemType)
            {
                case ItemType.Weapon:
                    item = new Weapon(itemInfo.TemplateId);
                    break;
                case ItemType.Armor:
                    item = new Armor(itemInfo.TemplateId);
                    break;
                case ItemType.Consumable:
                    item = new Consumable(itemInfo.TemplateId);
                    break;
                case ItemType.Etc:

                    item = new Etc(itemInfo.TemplateId);

                    // 골드면 생략하고,
                    if (item.TemplateId == 99999)
                    {
                        item = null;
                    }

                    // 기타템이면 만들어준다.
                    break;
        }

            // ItemDbId 도 넣어준다. & Count도 넣어준다
            if (item != null)
            {
                item.ItemDbId = itemInfo.ItemDbId;
                item.Count = itemInfo.Count;
                item.Slot = itemInfo.Slot;
                item.Equipped = itemInfo.Equipped;

                item.ReqStr = itemInfo.ReqStr;
                item.ReqDex = itemInfo.ReqStr;
                item.ReqInt = itemInfo.ReqInt;
                item.ReqLuk = itemInfo.ReqLuk;
                item.ReqLev = itemInfo.ReqLev;
                item.ReqPop = itemInfo.ReqPop;

                item.UpgradeSlot = itemInfo.UpgradeSlot;
                item.Str = itemInfo.Str;
                item.Dex = itemInfo.Dex;
                item.Int = itemInfo.Int;
                item.Luk = itemInfo.Luk;
                item.Hp = itemInfo.Hp;
                item.Mp = itemInfo.Mp;
                item.WAtk = itemInfo.WAtk;
                item.MAtk = itemInfo.MAtk;
                item.WDef = itemInfo.WDef;
                item.MDef = itemInfo.MDef;
                item.Speed = itemInfo.Speed;
                item.AtkSpeed = itemInfo.AtkSpeed;
                item.Durability = itemInfo.Durability;
                item.Enhance = itemInfo.Enhance;
                item.WPnt = itemInfo.WPnt;
                item.MPnt = itemInfo.MPnt;
            }

            return item;
        }

    }




    // 무기도 따로 만들어준다. 아이템 상속받음
    public class Weapon : Item
    {
        public WeaponType WeaponType { get; private set; }
        public int Damage { get; private set; } // 캐싱용도 랜덤데미지

        // 생성자를 만들어준다.
        public Weapon(int templateId) : base(ItemType.Weapon)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

            // 심각한 문제이나 리턴을 때린다.
            if (itemData.itemType != ItemType.Weapon)
                return;

            WeaponData data = (WeaponData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                WeaponType = data.weaponType;
                Damage = data.damage;
                Stackable = false; // 겹쳐지지 않는다.
            }
        }

    }




    // 방어구도 따로 만들어준다. 아이템 상속받음
    public class Armor : Item
    {
        public ArmorType ArmorType { get; private set; }
        public int Defence { get; private set; } // 캐싱용도 랜덤데미지

        // 생성자를 만들어준다.
        public Armor(int templateId) : base(ItemType.Armor)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

            // 심각한 문제이나 리턴을 때린다.
            if (itemData.itemType != ItemType.Armor)
                return;

            ArmorData data = (ArmorData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                ArmorType = data.armorType;
                Defence = data.defence;
                Stackable = false; // 겹쳐지지 않는다.
            }
        }

    }





    // 소비도 따로 만들어준다. 아이템 상속받음
    public class Consumable : Item
    {
        public ConsumableType ConsumableType { get; private set; }
        public int MaxCount { get; set; } // 캐싱용도 랜덤데미지

        // 생성자를 만들어준다.
        public Consumable(int templateId) : base(ItemType.Consumable)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

            // 심각한 문제이나 리턴을 때린다.
            if (itemData.itemType != ItemType.Consumable)
                return;

            ConsumableData data = (ConsumableData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                MaxCount = data.maxCount;
                ConsumableType = data.consumableType;
                Stackable = (data.maxCount > 1); // 겹쳐지지 않는다.
            }
        }

    }



    // 기타도 따로 만들어준다. 아이템 상속받음
    public class Etc : Item
    {
        public EtcType EtcType { get; private set; }
        public int MaxCount { get; set; } // 캐싱용도 랜덤데미지

        // 생성자를 만들어준다.
        public Etc(int templateId) : base(ItemType.Etc)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

            // 심각한 문제이나 리턴을 때린다.
            if (itemData.itemType != ItemType.Etc)
                return;

            EtcData data = (EtcData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                MaxCount = data.maxCount;
                EtcType = data.etcType;
                Stackable = (data.maxCount > 1); // 겹쳐진다.
            }
        }

    }








