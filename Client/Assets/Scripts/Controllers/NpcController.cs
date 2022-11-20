using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class NpcController : CreatureController
{

    public ObjectInfo npcInfo;

    // 내 상태에 따라 바뀌는 퀘스트
    public List<QuestData> NpcQuestList = new List<QuestData>();

    // NPC 가 가지고 있는 모든퀘스트
    public List<QuestData> Own_Npc_QuestList = new List<QuestData>();

    public List<GameObject> QuestIcons = new List<GameObject>();

    // BaseController를 따르되, AddHpBar만 추가로 해준다.
    protected override void Init()
    {
        base.Init();
        AddName();

        // Npc가 가지고 있는 퀘스트 정보 모두 가지고 오기

        foreach (int t in Managers.Quest.Get_Quests_Npc(_stat.TemplateId))
        {
            QuestData questData = null;
            Managers.Data.QuestDict.TryGetValue(t, out questData);

            if(questData != null)
                Own_Npc_QuestList.Add(questData);
        }



        // 퀘스트 표시
        QuestIconRefresh();


    }

    public void QuestIconRefresh()
    {

        if (Own_Npc_QuestList == null)
            return;

        // 퀘스트 아이콘들 초기화

        foreach(GameObject t in QuestIcons)
        {
            Managers.Resource.Destroy(t);
        }

        QuestIcons.Clear();

        foreach(QuestData t in Own_Npc_QuestList)
        {
            if(t.status == 1)
            {
                bool IsAlready = false;

                foreach (Quest q in Managers.Quest.Quests.Values)
                {
                    //Debug.Log("#" + q.QuestTemplateId + "/" + q.Info + "/" + q.NpcId + "/" + q.Status);

                    if (q.QuestTemplateId/100 == t.questId/100)
                        IsAlready = true;

                }

                if(IsAlready == false)
                {
                    // 첫 물음표 생성

                    Debug.Log("물음표");
                    GameObject QuestIcon = Managers.Resource.Instantiate("Effect/QuestIcon/QuestIcon", transform);
                    QuestIcon.transform.position = transform.position + new Vector3(0, 48f, 0);

                    QuestIcons.Add(QuestIcon);

                }
            }
            else
            {
                bool IsProcess = false;

                foreach (Quest q in Managers.Quest.Quests.Values)
                {
                    Debug.Log("#" + q.QuestTemplateId / 100 + "@@@" + q.QuestTemplateId + "/" + q.Info + "/" + q.NpcId + "/" + q.Status);


                    if (q.QuestTemplateId / 100 == t.questId / 100 &&        q.Status == t.status)
                        IsProcess = true;
                

                }

                if(IsProcess == true)
                {
                    Debug.Log("진행중 말풍선");
                    GameObject QuestIcon = Managers.Resource.Instantiate("Effect/QuestIcon/QuestIcon", transform);
                    QuestIcon.transform.position = transform.position + new Vector3(0, 48f, 0);

                    QuestIcons.Add(QuestIcon);
                }


            }

  

        }




    }

    // HpBar 생성
    protected void AddName()
    {
        Text Name = transform.Find("Canvas/NameBox/NameText").GetComponent<Text>();
        Name.text = transform.name;
    }

    public override void OnDamaged(int damage, int skillId, List<int> DamageList, int attackerId)
    {
        base.OnDamaged(damage,skillId, DamageList, attackerId);
        Debug.Log("Player Hit !");
    }

    float clickTIme = 0;
    private void OnMouseUp()
    {
        if((Time.time - clickTIme) < 0.3f)
        {
            OnMouseDoubleClick();
            clickTIme = -1;
        }
        else
        {
            clickTIme = Time.time;
        }
    }

    void OnMouseDoubleClick()
    {

        // 이미 NPC 대화창이 켜져 있는지 확인한다.

        GameObject NpcDialogue = GameObject.Find("UI_DialoguePopup");

        if (NpcDialogue != null)
            return;


        C_Npc npcPacket = new C_Npc();

        npcPacket.ObjectId = Id;
        Managers.Network.Send(npcPacket);

        Debug.Log("NPC Double Clicked");

    }



}
