using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DialoguePopup : UI_Popup
{

    public ObjectInfo npcInfo;

    public List<UI_DialoguePopup_Item> Items { get; } = new List<UI_DialoguePopup_Item>();

    public NpcController Npc;

    public int Index;

    public QuestData CurrentQuest;

    public SpriteRenderer A;

    public  Coroutine CoTyping;

    int TempNum = 0;
    int Status = 0;
    int Order = 0;
    int Button = 0;

    enum Buttons
    {
        CloseBtn,
        YesBtn,
        NoBtn,
        NextBtn,
        NpcSprite,
    }

    enum Texts
    {
        NpcNameText,
        DialogueText
    }

    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.CloseBtn).gameObject.BindEvent(OnClickCloseButton);
        GetImage((int)Buttons.YesBtn).gameObject.BindEvent(OnClickYesButton);
        GetImage((int)Buttons.NoBtn).gameObject.BindEvent(OnClickNoButton);
        GetImage((int)Buttons.NextBtn).gameObject.BindEvent(OnClickNextButton);

        GetImage((int)Buttons.CloseBtn).gameObject.SetActive(true);
        GetImage((int)Buttons.YesBtn).gameObject.SetActive(false);
        GetImage((int)Buttons.NoBtn).gameObject.SetActive(false);
        GetImage((int)Buttons.NextBtn).gameObject.SetActive(false);

        Bind<Text>(typeof(Texts));
    }

    public void closeAllButtons()
    {
        GetImage((int)Buttons.CloseBtn).gameObject.SetActive(false);
        GetImage((int)Buttons.YesBtn).gameObject.SetActive(false);
        GetImage((int)Buttons.NoBtn).gameObject.SetActive(false);
        GetImage((int)Buttons.NextBtn).gameObject.SetActive(false);
    }

    void OnClickCloseButton(PointerEventData evt)
    {

        Managers.UI.ClosePopupUI(this);

    }

    void OnClickYesButton(PointerEventData evt)
    {

        Order += 1;
        RefreshDialogue(CurrentQuest.dialogue[Order].script, CurrentQuest.dialogue[Order].index, Npc);

    }

    void OnClickNoButton(PointerEventData evt)
    {
        // 거절은 +1 이다.
        Order += 0;
        RefreshDialogue(CurrentQuest.dialogue[Order].script, CurrentQuest.dialogue[Order].index, Npc);

    }

    void OnClickNextButton(PointerEventData evt)
    {
        // 다음 대화창 나오게 하기.

        RefreshDialogue(CurrentQuest.dialogue[Order].script, CurrentQuest.dialogue[Order].index, Npc);

        //UI_DialoguePopup.RefreshDialogue(questData.dialogue[0].script, questData.dialogue[0].index, UI_DialoguePopup.Npc);


    }

    public void RefreshDialogue(string text, int index, NpcController npc )
    {

        // index 의 상태에 따라 버튼의 형태를 만들어준다.
        SetButton(index);

        // 대화창의 순서를 주정한다.

        Index = index;

        // 바로 뜨는것이 아니라, 하나씩 뜨게 한다.
        //GetText((int)Texts.DialogueText).text = text;

        if (CoTyping != null)
            StopCoroutine(CoTyping);

        CoTyping = StartCoroutine(_typing(text));

        if(npcInfo !=null)
            GetText((int)Texts.NpcNameText).text = npcInfo.Name;

        List<QuestData> questDatas = new List<QuestData>();


        Data.NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(npcInfo.StatInfo.TemplateId, out npcData);

        Sprite icon = Managers.Resource.Load<Sprite>(npcData.iconPath);
        GetImage((int)Buttons.NpcSprite).gameObject.GetComponent<Image>().sprite = icon;




        // 첫 시작할때만 (0일때만) 퀘스트를 만들어준다. 그외에는 텍스트만
        if ((index / 100) < 1)
        {
            Npc = npc;

            IEnumerable<QuestData> npcquestListReverse = npc.NpcQuestList;

            // npc 의 퀘스트 목록을 불러온다.
            // 거꾸로 불러와야 아예 없을때 첫번째 퀘스트를 보여주게 된다.
            foreach (QuestData p in npcquestListReverse.Reverse())
            {

                // 나의 퀘스트 현황에 따라 보여주는 퀘스트가 다르다.
                Quest A = Managers.Quest.Get_template(p.questId);

                if( A  == null)
                {
                    // 최후의 보루까지 왔을때도 0이라면
                    if (questDatas.Count == 0)
                    {
                        // 같은 퀘스트를 갖고 있는데, status가 다른 것인지 확인한다.
                        int AlreadyQuest = Managers.Quest.Check_AlreadyQuest(p.questId);

                        // 진행중(수락한)인 퀘스트 라면,
                        if (AlreadyQuest != -1)
                        {
                            //// 현재의 Status의 직전 퀘스트를 보여주고,
                            //QuestData pastQuest = npc.NpcQuestList[AlreadyQuest - 2];

                            //// 그 퀘스트의 다이얼로그 중 가장 마지막 script를 보여주게한다.
                            //pastQuest.status = AlreadyQuest;

                            //questDatas.Add(pastQuest);



                            // 현재의 Status의 직전 퀘스트를 보여주고,
                            //QuestData pastQuest = npc.NpcQuestList[AlreadyQuest - 2];
                            QuestData pastQuest = null;

                            // 예를들어 지금 1000003 이면, Already Quest 가 3일거고, 직전(AlreadyQuest-1) 은 3-1 = 2 이니까,
                            // 그 2랑 같은 stauts를 갖고 있는 quest를 가져온다. 
                            foreach (QuestData t in npc.NpcQuestList)
                            {
                                if ((AlreadyQuest - 1) == t.status)
                                {
                                    pastQuest = t;
                                }
                            }

                            if (pastQuest != null)
                            {
                                // 그 퀘스트의 다이얼로그 중 가장 마지막 script를 보여주게한다.
                                pastQuest.status = AlreadyQuest;

                                questDatas.Add(pastQuest);
                            }
                            continue;
                        }


                        // status가 진행중인 것의 -1로 보여주되, 진행중인 status의
                        // dialogue 를 보여준다.

                        // Quest가 status 1인 것만 보내준다.
                        if (p.status == 1)
                        {
                            questDatas.Add(p);
                        }

                    }

                }
                else
                {
                    // Quest가 내 Quest의 Status랑 같은 것만 보내준다.
                    if(A.Status == p.status)
                    {
                        questDatas.Add(p);
                    }
                    // Quest가 진행중이어서 status가 다르고, 직전 Status일때
                    else if (A.Status != p.status &&  (A.Status-1 == p.status ))
                    {
                        // 다만 npc가 같으면 넣어주지 않는다. 같은 npc인 경우에는 둘다 가지면 안되기 때문이다.

                        if (A.NpcId != p.npc)
                        {
                            // 끝의 글을 보여주기 위하여 교체를 해준다.
                            p.status = A.Status;
                            questDatas.Add(p);
                        }
                    }
                }               

                // 버튼형 텍스트를 만들고 넣어준다.

                // 다이얼로그 버튼을 누르면 그에 따라 스크립트가 계속 나올 수 있게 한다.



            }

            SetQuests(questDatas);
        }



        SendAction();


    }

    public void SendAction()
    {
        // 값이 있는지 확인
        if (CurrentQuest == null)
            return;

        // 다이얼로그가 들어가있는지 확인
        if (CurrentQuest.dialogue.Count == 0)
            return;


        // statusChange 요청
        if (CurrentQuest.dialogue[Order-1].statusChange != -999)
        {

            // 팝업 닫기
            Managers.UI.ClosePopupUI(this);

            Debug.Log("퀘스트 Status 변경요청");

            C_Npc npcPacket = new C_Npc();

            npcPacket.ObjectId = npcInfo.ObjectId;
            npcPacket.Quest = CurrentQuest.questId;
            npcPacket.Order = Order - 1;
            //if (Status >= 1)
            //    npcPacket.Action = QuestAction.Success;
            //else
            //    npcPacket.Action = QuestAction.Get;

            Managers.Network.Send(npcPacket);

        }


        if (CurrentQuest.dialogue[Order].getItem != null && CurrentQuest.dialogue[Order].getItem.Count != 0)
            Debug.Log("퀘스트 getItem 변경 요청");

        if (CurrentQuest.dialogue[Order].loseItem != null && CurrentQuest.dialogue[Order].loseItem.Count != 0)
            Debug.Log("퀘스트 loseItem 변경 요청");

        if (CurrentQuest.dialogue[Order].checkItem != null && CurrentQuest.dialogue[Order].checkItem.Count != 0)
            Debug.Log("퀘스트 checkItem 변경 요청");

        if (CurrentQuest.dialogue[Order].reqItem != null && CurrentQuest.dialogue[Order].reqItem.Count != 0)
            Debug.Log("퀘스트 reqItem 변경 요청");
    }

    public void SetButton(int index)
    {

        closeAllButtons();



        TempNum = 0;

        Status = index / 100;
        TempNum = index % 100;

        Order = TempNum / 10;
        TempNum = TempNum % 10;

        Button = TempNum;

        switch (Button)
        {
            case 0:
                // Next
                GetImage((int)Buttons.NextBtn).gameObject.SetActive(true);
                break;
            case 1:
                // Close
                GetImage((int)Buttons.CloseBtn).gameObject.SetActive(true);
                break;
            case 2:
                // Yes or No
                GetImage((int)Buttons.YesBtn).gameObject.SetActive(true);
                GetImage((int)Buttons.NoBtn).gameObject.SetActive(true);
                break;
            
        }

    

       

    }


    public void SetQuests(List<QuestData> quests)
    {



        Items.Clear();

        // 다 지워주고
        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        // 다 생성해주고,

        for( int i = 0; i < quests.Count; i ++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_DialoguePopup_Item", grid.transform);
            UI_DialoguePopup_Item item = go.GetOrAddComponent<UI_DialoguePopup_Item>();
            Items.Add(item);

            // 팝업한테 얘 정보를 준다.
            item.UI_DialoguePopup = this;

            item.Quest = quests[i];
        }

        RefreshUI();

    }

    public void RefreshUI()
    {

        if (Items.Count == 0)
            return;


        foreach (var item in Items)
        {
            item.RefreshUI();
        }

    }

    public void CloseQuests()
    {
        foreach(var item in Items)
        {
            Destroy(item.gameObject);

        }

    }



    void OnClickLogoutButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
    }

    IEnumerator _typing(string DialogueText)
    {

        for(int i  = 0; i <= DialogueText.Length; i++)
        {
            GetText((int)Texts.DialogueText).text = DialogueText.Substring(0, i);

            yield return new WaitForSeconds(0.02f);

        }
    }



}
