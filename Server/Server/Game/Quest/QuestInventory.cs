using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class QuestInventory
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
        public Quest Find(Func<Quest,bool> condition)
        {

            foreach(Quest quest in Quests.Values)
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


    }
}
