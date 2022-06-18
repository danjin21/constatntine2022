using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Data
{
    // 당장은 클라에서 정보를 들고 있을 필요가 없어서 우선 가린다.
    //#region Stat
    //[Serializable]
    //public class Stat
    //{
    //    public int level;
    //    public int maxHp;
    //    public int attack;
    //    public int totalExp;
    //}

    //[Serializable]
    //public class StatData : ILoader<int, Stat>
    //{
    //    public List<Stat> stats = new List<Stat>();

    //    public Dictionary<int, Stat> MakeDict()
    //    {
    //        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
    //        foreach (Stat stat in stats)
    //            dict.Add(stat.level, stat);
    //        return dict;
    //    }
    //}
    //#endregion






    #region Skill



    // 스킬 모델링

    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float cooldown;
        public int damage;
        public SkillType skillType;
        public ProjectileInfo projectile;
        public string desc;
        public string iconPath;
        public int mp;
        public string soundPath;

    }

    public class ProjectileInfo
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;
    }


    // 스킬 불러오는것
    [Serializable]
    public class SkillData : ILoader<int, Skill>
    {
        public List<Skill> skills = new List<Skill>();

        public Dictionary<int, Skill> MakeDict()
        {
            Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
            foreach (Skill skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }


    #endregion





    #region Item



    // 아이템 모델링

    [Serializable]
    public class ItemData
    {
        public int id;
        public string name; // 다국적으로 하려면 해당하는 문자열에 대한 ID를 넣어서
        public ItemType itemType; // 아이템 타입
        public string iconPath; // 아이콘 경로
        public int price; // 아이템 가격
        public int sellPrice; // 팔 때 가격

        public string desc;
        public int isTown = 0;
        // 아이템 정보는 때려넣지말고.. 분리해서 해야 관리하기 편하다.
        //int damage;
        //int defence;
    }

    [Serializable]
    public class WeaponData : ItemData
    {
        public WeaponType weaponType;
        public int damage;

        public int reqStr;
        public int reqDex;
        public int reqInt;
        public int reqLuk;
        public int reqLev;
        public int reqPop;

        public int UpgradeSlot;
        public int Str;
        public int Dex;
        public int Int;
        public int Luk;
        public int Hp;
        public int Mp;
        public int WAtk;
        public int MAtk;
        public int WDef;
        public int MDef;
        public int Speed;
        public int Jump;
        public int Durability;
        public int Enhance;
        public int WPnt;
        public int MPnt;
    }

    [Serializable]
    public class ArmorData : ItemData
    {
        public ArmorType armorType;
        public int defence;

        public int reqStr;
        public int reqDex;
        public int reqInt;
        public int reqLuk;
        public int reqLev;
        public int reqPop;

        public int UpgradeSlot;
        public int Str;
        public int Dex;
        public int Int;
        public int Luk;
        public int Hp;
        public int Mp;
        public int WAtk;
        public int MAtk;
        public int WDef;
        public int MDef;
        public int Speed;
        public int Jump;
        public int Durability;
        public int Enhance;
        public int WPnt;
        public int MPnt;
    }

    [Serializable]
    public class ConsumableData : ItemData
    {
        public ConsumableType consumableType;
        public int maxCount;

    }

    [Serializable]
    public class EtcData : ItemData
    {
        public EtcType etcType;
        public int maxCount;

    }


    // 아이템 불러오는것
    [Serializable]
    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<ArmorData> armors = new List<ArmorData>();
        public List<ConsumableData> consumables = new List<ConsumableData>();
        public List<EtcData> etcs = new List<EtcData>();

        // 한번에 관리하는 딕셔너리
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            // 무기
            foreach (ItemData item in weapons)
            {
                item.itemType = ItemType.Weapon;
                dict.Add(item.id, item);
            }
            // 방어구
            foreach (ItemData item in armors)
            {
                item.itemType = ItemType.Armor;
                dict.Add(item.id, item);
            }
            // 소비
            foreach (ItemData item in consumables)
            {
                item.itemType = ItemType.Consumable;
                dict.Add(item.id, item);
            }
            // 기타
            foreach (ItemData item in etcs)
            {
                item.itemType = ItemType.Etc;
                dict.Add(item.id, item);
            }

            return dict;
        }
    }


    #endregion



    #region Monster


    [Serializable]
    public class MonsterData
    {
        public int id;
        public string name;
        public StatInfo stat; // 나중에 지워도됌
        public string prefabPath;
        public string HitSoundPath;

    }


    [Serializable]
    public class MonsterLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
            {
                //stat.Hp = stat.MaxHp;
                dict.Add(monster.id, monster);
            }

            return dict;
        }
    }
    #endregion



    #region Monster


    [Serializable]
    public class MapInfoData
    {
        public int mapId;
        public string name;
        public string soundPath;
        public string townName;
        public int townId;
    }


    [Serializable]
    public class MapInfoLoader : ILoader<int, MapInfoData>
    {
        public List<MapInfoData> maps = new List<MapInfoData>();

        public Dictionary<int, MapInfoData> MakeDict()
        {
            Dictionary<int, MapInfoData> dict = new Dictionary<int, MapInfoData>();
            foreach (MapInfoData map in maps)
            {
                //stat.Hp = stat.MaxHp;
                dict.Add(map.mapId, map);
            }

            return dict;
        }
    }
    #endregion



    #region Npc


    // NPC 모델링

    [Serializable]
    public class NpcData
    {
        public int npcId;
        public string name;
        public int map;
        public int posX;
        public int posY;
        public int quest;
        public string iconPath;
        public List<ChatData> chats;
    }


    public class ChatData
    {
        public int index;
        public string chat;
    }


    // NPC 불러오는것
    [Serializable]
    public class NpcLoader : ILoader<int, NpcData>
    {
        public List<NpcData> npcs = new List<NpcData>();

        public Dictionary<int, NpcData> MakeDict()
        {
            Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
            foreach (NpcData npc in npcs)
                dict.Add(npc.npcId, npc);
            return dict;
        }
    }


    #endregion






    #region Quest



    // Quest 모델링

    [Serializable]
    public class QuestData
    {
        public int questId;
        public string questName;
        public int reqQuest;
        public int reqJob;
        public int reqLev;
        public int npc;
        public int status;
        public string desc;

        public List<DialogueData> dialogue;
    }

    [Serializable]
    public class DialogueData
    {
        public int index;
        public string script;

        public int statusChange = -999;

        public List<QuestItemData> getItem;
        public List<QuestItemData> checkItem;
        public List<QuestItemData> loseItem;
        public List<QuestItemData> reqItem;
    }

    [Serializable]
    public class QuestItemData
    {
        public int itemId;
        public int quantity;
    }


    // Quest 불러오는것
    [Serializable]
    public class QuestLoader : ILoader<int, QuestData>
    {
        public List<QuestData> quests = new List<QuestData>();

        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();

            int questIdNew;

            foreach (QuestData quest in quests)
            {
                // 퀘스트 아이디와 스테이터스를 조합해서 새롭게 아이디를 만든다.
                //questIdNew = quest.questId * 100 + quest.status;
                questIdNew = quest.questId;

                dict.Add(questIdNew, quest);
            }
            return dict;
        }
    }


    #endregion


}