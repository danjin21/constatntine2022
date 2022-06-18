using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{

    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        // proto 정보를 불러오기전
        //public static Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();
        public static Dictionary<int, StatInfo> StatDict { get; private set; } = new Dictionary<int, StatInfo>();
        public static Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
        public static Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
        public static Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, Data.MonsterData>();
        public static Dictionary<int, Data.PortalData> PortalDict { get; private set; } = new Dictionary<int, Data.PortalData>();
        public static Dictionary<int, Data.NpcData> NpcDict { get; private set; } = new Dictionary<int, Data.NpcData>();
        public static Dictionary<int, Data.MapInfoData> MapDict { get; private set; } = new Dictionary<int, Data.MapInfoData>();
        public static Dictionary<int, Data.QuestData> QuestDict { get; private set; } = new Dictionary<int, Data.QuestData>();


        public static void LoadData()
        {
            // proto 정보를 불러오기전
            //StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
            StatDict = LoadJson<Data.StatData, int, StatInfo>("StatData").MakeDict();
            SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();

            // 유니티 안에 있는 데이터 시트에서 불러와서 딕셔너리에 넣는다.
            ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();

            MonsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData").MakeDict();


            PortalDict = LoadJson<Data.PortalLoader, int, Data.PortalData>("PortalData").MakeDict();
            NpcDict = LoadJson<Data.NpcLoader, int, Data.NpcData>("NpcData").MakeDict();
            MapDict = LoadJson<Data.MapInfoLoader, int, Data.MapInfoData>("MapData").MakeDict();

            QuestDict = LoadJson<Data.QuestLoader, int, Data.QuestData>("QuestData").MakeDict();


        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {

       

            // 데이터를 불러오는곳
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);


        }
    }

}
