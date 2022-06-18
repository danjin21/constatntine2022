using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QuestManager : MonoBehaviour
{


    public Dictionary<int, Quest> Quests { get; } = new Dictionary<int, Quest>();

    public void Add(Quest quest)
    {
        Quests.Add(quest.QuestDbId, quest);
    }

    public Quest Get(int questDbId)
    {
        Quest quest = null;
        Quests.TryGetValue(questDbId, out quest);
        return quest;
    }

    // 아이템을 찾는다.
    public Quest Find(Func<Quest, bool> condition)
    {

        foreach (Quest quest in Quests.Values)
        {
            if (condition.Invoke(quest))
                return quest;
        }

        return null;
    }


    // 퀘스트를 지운다.

    public void Delete(Quest quest)
    {
        Quests.Remove(quest.QuestDbId);
    }

    // 이미 가지고 있는 퀘스트 인지


    // 아이템 template ID 를 통해 아이템의 정보를 불러온다.
    public Quest Get_template(int templateId)
    {
        Quest quest = null;

        // 밸류값으로 키값 찾기.

        int Key = Quests.FirstOrDefault(x => x.Value.QuestTemplateId == templateId).Key;
        Quests.TryGetValue(Key, out quest);

        return quest;

    }


    public void Clear()
    {
        Quests.Clear();
    }

    // 같은 퀘스트인지 확인
    public int Check_AlreadyQuest(int templateId)
    {
        Quest quest = null;

        int Key = Quests.FirstOrDefault(x => x.Value.QuestTemplateId / 100 == templateId / 100).Key;
        Quests.TryGetValue(Key, out quest);

        int A = -1;

        // 매치되는 퀘스트가 있다면 현재 상태의 Status를 가져와 준다.
        if (quest != null)
            A = quest.Status;

        return A;
    }


}