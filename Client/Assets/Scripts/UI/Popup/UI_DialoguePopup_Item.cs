using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DialoguePopup_Item : UI_Base
{
    public UI_DialoguePopup UI_DialoguePopup;

    public QuestData Quest { get; set; }

    enum Buttons
    {
        SelectQuestButton
    }

    enum Texts
    {
        QuestNameText
    }


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SelectQuestButton).gameObject.BindEvent(OnClickButton);

    }

    public void RefreshUI()
    {
        if (Quest == null)
            return;

        GetText((int)Texts.QuestNameText).text = Quest.questName;
    }

    void OnClickButton(PointerEventData evt)
    {
        QuestDialogueStart();

    }

    void QuestDialogueStart()
    {
        // 만약 퀘스트를 시작하는데, id의 status와 status가 다르다면,
        // 진행중인 퀘스트이므로, 가장 마지막 스크립트를 보여주고 끝낸다.



        Data.QuestData questData = null;
        //Managers.Data.QuestDict.TryGetValue(Quest.questId*100+Quest.status, out questData);
        Managers.Data.QuestDict.TryGetValue(Quest.questId, out questData);


        int temp = Quest.questId % 10;

        if (temp != Quest.status)
        {
            // 가장 마지막 다이얼로그를 보여준다.

            int finalIndex = questData.dialogue.Count-1;

            UI_DialoguePopup.RefreshDialogue(questData.dialogue[finalIndex].script, questData.dialogue[finalIndex].index, UI_DialoguePopup.Npc);
            UI_DialoguePopup.CurrentQuest = questData;

            // 퀘스트 버튼들은 모두 지워준다.
            UI_DialoguePopup.CloseQuests();

            return;
        }


        UI_DialoguePopup.RefreshDialogue(questData.dialogue[0].script, questData.dialogue[0].index, UI_DialoguePopup.Npc);

        UI_DialoguePopup.CurrentQuest = questData;

        // 퀘스트 버튼들은 모두 지워준다.
        UI_DialoguePopup.CloseQuests();
    }

}
