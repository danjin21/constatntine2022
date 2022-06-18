using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}



public class DataManager
{
    //public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();
    public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.MapInfoData> MapDict { get; private set; } = new Dictionary<int, Data.MapInfoData>();
    public Dictionary<int, Data.NpcData> NpcDict { get; private set; } = new Dictionary<int, Data.NpcData>();
    public Dictionary<int, Data.QuestData> QuestDict { get; private set; } = new Dictionary<int, Data.QuestData>();


    public void Init()
    {
        //StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
        ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        MonsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        MapDict = LoadJson<Data.MapInfoLoader, int, Data.MapInfoData>("MapData").MakeDict();
        NpcDict = LoadJson<Data.NpcLoader, int, Data.NpcData>("NpcData").MakeDict();
        QuestDict = LoadJson<Data.QuestLoader, int, Data.QuestData>("QuestData").MakeDict();

    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}
}
