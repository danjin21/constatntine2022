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

    public List<QuestData> NpcQuestList = new List<QuestData>();




    // BaseController를 따르되, AddHpBar만 추가로 해준다.
    protected override void Init()
    {
        base.Init();
        AddName();
    }
 
    // HpBar 생성
    protected void AddName()
    {
        Text Name = transform.Find("Canvas/NameBox/NameText").GetComponent<Text>();
        Name.text = transform.name;
    }

    public override void OnDamaged(int damage, int skillId, List<int> DamageList)
    {
        base.OnDamaged(damage,skillId, DamageList);
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
