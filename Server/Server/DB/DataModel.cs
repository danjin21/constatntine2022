using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Server.DB
{
    [Table("Account")]
    public class AccountDb   // Enity라는걸 알리기 위해 Db를 붙인다.
    {
        public int AccountDbId {get;set;} // Id를 넣으면 주키가 된다고함.
        public string AccountName { get; set; } // (Unique) 보통 유저가 게임시작할때 입력하거나, 휴대폰의 기기값을 여기에 넣는다.
        public ICollection<PlayerDb> Players { get; set; } // 1 대 다수
    }


    [Table("Player")]
    public class PlayerDb   // Enity라는걸 알리기 위해 Db를 붙인다.
    {
        public int PlayerDbId { get; set; } // Id를 넣으면 Primary Key가된다고함.
        public string PlayerName { get; set; } // (Unique) 보통 유저가 게임시작할때 입력
        
        [ ForeignKey("Account")] // AccountDbId 랑 Account랑 짝이라는거를 얘기해준다.
        public int AccountDbId { get; set; }
        public AccountDb Account { get; set; } // 계정값을 갖고 있는다.

        // 소지하고 있는 아이템들
        public ICollection<ItemDb> Items { get; set; }
        public ICollection<KeySettingDb> KeySettings { get; set; }
        public ICollection<SkillDb> Skills { get; set; }

        public int Level { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }
        public int Map { get; set; } = 1;
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Hair { get; set; }
        public int HairColor { get; set; }
        public int Face { get; set; }
        public int Skin { get; set; }
        public int Gender { get; set; }
        public int Mp { get; set; }
        public int MaxMp { get; set; }
        public int Exp { get; set; }
        public int Gold { get; set; }
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }
        public int Luk { get; set; }
        public int Def { get; set; }
        public int StatPoint { get; set; }
        //public int atkSpeed { get; set; }
        //public int wAtk { get; set; }
        //public int mAtk { get; set; }
        //public int wDef { get; set; }
        //public int mDef { get; set; }
        //public int wPnt { get; set; }
        //public int mPnt { get; set; }


        //public int Helmet { get; set; }
        //public int RightHand { get; set; }
        //public int LeftHand { get; set; }
        //public int Shirts { get; set; }
        //public int Pants { get; set; }
        //public int Shoes { get; set; }

    }

    [Table("Item")]
    public class ItemDb
    {
        public int ItemDbId { get; set; } // 데이터베이스에서 지정해주는
        public int TemplateId { get; set; } // 어떤 아이템 시트의 몇번이다.
        public int Count { get; set; } // 갯수
        public int Slot { get; set; } // 슬롯 인덱스 ( 인벤토리에서 아이템 배치 할때 그 위치 )
        public bool Equipped { get; set; } = false; // 장착여부
        // 활용 : 0~10 까지는 착용 / 11~40 은 아이템창 / 41~60 까지는 창고 등등

        public int ReqStr { get; set; } = 0;
        public int ReqDex { get; set; } = 0;
        public int ReqInt { get; set; } = 0;
        public int ReqLuk { get; set; } = 0;
        public int ReqLev { get; set; } = 0;
        public int ReqPop { get; set; } = 0;

        public int UpgradeSlot { get; set; } = 7;
        public int Str { get; set; } = 0;
        public int Dex { get; set; } = 0;
        public int Int { get; set; } = 0;
        public int Luk { get; set; } = 0;
        public int Hp { get; set; } = 0;
        public int Mp { get; set; } = 0;
        public int WAtk { get; set; } = 0;
        public int MAtk { get; set; } = 0;
        public int WDef { get; set; } = 0;
        public int MDef { get; set; } = 0;
        public int Speed { get; set; } = 0;
        public int AtkSpeed { get; set; } = 0;
        public int Durability { get; set; } = -1;
        public int Enhance { get; set; } = 0;
        public int WPnt { get; set; } = 0;
        public int MPnt { get; set; } = 0;




        [ForeignKey("Owner")] // Owner 랑 연동이 되어있다고 말을 한다.
        public int? OwnerDbId { get; set; } // 땅바닥에 있으면 주인이 없을 수도 있어서 ? 를 붙인다.
        public PlayerDb Owner { get; set; }


    }




    [Table("KeySetting")]
    public class KeySettingDb   // Enity라는걸 알리기 위해 Db를 붙인다.
    {

        public int KeySettingDbId { get; set; }

        public int key { get; set; } = -1;
        public int type { get; set; } = -1;
        public int action { get; set; } = -1;


        [ForeignKey("Owner")] // Owner 랑 연동이 되어있다고 말을 한다.
        public int? OwnerDbId { get; set; } 
        public PlayerDb Owner { get; set; }



        

    }

    [Table("Skill")]
    public class SkillDb
    {
        public int SkillDbId { get; set; }
        public int SkillTemplateId { get; set; }
        public int SkillLevel { get; set; }
        public int Slot { get; set; }

        [ForeignKey("Owner")]
        public int? OwnerDbId { get; set; }
        public PlayerDb Owner { get; set; }

    }


    [Table("QuestStatus")]
    public class QuestDb
    {
        public int QuestDbId { get; set; }
        public int QuestDbTemplateId { get; set; }
        public int Status { get; set; }

        [ForeignKey("Owner")]
        public int? OwnerDbId { get; set; }
        public PlayerDb Owner { get; set; }

    }


    [Table("QuestMobCount")]
    public class QuestMobCountDb
    {
        public int QuestMobCountDbId { get; set; }
        public int MobId { get; set; }
        public int Count { get; set; }

        [ForeignKey("Quest")]
        public int? QuestDbId { get; set; }
        public QuestDb QuestDb { get; set; }
    }



}
