using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 서버에서 복붙

// 진짜 서버 메모리에 들어가 있는 아이템
// Proto를 참고하여 get set 을 만든다.

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


    public static Quest MakeQuest(QuestInfo questInfo)
    {

        int RealQuestTemplateId = (int)(questInfo.QuestTemplateId / 10);

        Quest quest = null;
        quest = new Quest();

        QuestData questData = null;
        Managers.Data.QuestDict.TryGetValue(questInfo.QuestTemplateId, out questData);

        if (questData == null)
            return null;

        //if(quest != null)
        //{
        //    quest.QuestDbId = questDb.QuestDbId;
        //    quest.QuestTemplateId = questDb.QuestDbTemplateId;
        //    quest.PlayerId = (int)questDb.OwnerDbId;
        //}

  
        quest.QuestDbId = questInfo.QuestDbId;
        quest.QuestTemplateId = questInfo.QuestTemplateId;
        quest.Status = questInfo.Status;

        // 엔피씨 아이디 넣어준다
        quest.NpcId = questData.npc;

        return quest;
    }


}