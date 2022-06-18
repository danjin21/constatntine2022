using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Quest
    {


        public QuestInfo Info { get; } = new QuestInfo();

        public int QuestDbId
        {
            get { return Info.QuestDbId; }
            set { Info.QuestDbId = value; }
        }

        public int NpcId
        {
            get { return Info.NpcId; }
            set { Info.NpcId = value; }
        }
        public int QuestTemplateId
        {
            get { return Info.QuestTemplateId; }
            set { Info.QuestTemplateId = value; }
        }

        public int PlayerId
        {
            get { return Info.PlayerId; }
            set { Info.PlayerId = value; }
        }

        public int Status
        {
            get { return Info.Status; }
            set { Info.Status = value; }
        }

        public static Quest MakeQuest(QuestDb questDb)
        {

       
            Quest quest = null;
            quest = new Quest();

            QuestData questData = null;
            DataManager.QuestDict.TryGetValue(questDb.QuestDbTemplateId, out questData);

            if (questData == null)
                return null;

            //if(quest != null)
            //{
            //    quest.QuestDbId = questDb.QuestDbId;
            //    quest.QuestTemplateId = questDb.QuestDbTemplateId;
            //    quest.PlayerId = (int)questDb.OwnerDbId;
            //}

            quest.QuestDbId = questDb.QuestDbId;
            quest.QuestTemplateId = questDb.QuestDbTemplateId;
            quest.PlayerId = (int)questDb.OwnerDbId;
            quest.Status = questDb.Status;

            return quest;
        }

        public static Quest MakeQuestFromId(Player player, int questId)
        {
            Quest quest = null;
            quest = new Quest();

            QuestData questData = null;
            DataManager.QuestDict.TryGetValue(questId, out questData);

            if (questData == null)
                return null;

            quest.QuestTemplateId = questData.questId;
            quest.PlayerId = (int)player.PlayerDbId;
            quest.Status = 2;

            return quest;
        }

    }
}
