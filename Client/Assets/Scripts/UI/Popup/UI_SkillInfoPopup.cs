using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillInfoPopup : UI_Popup
{

    //public ItemInfo nowItem;
    public Item Item;

    public List<Text> InfoTexts = new List<Text>();

    public GameObject ReqInfoList;
    public GameObject JobList;

    enum Texts
    {
        ItemNameText,
        ReqLevText,
        ReqStrText,
        ReqDexText,
        ReqIntText,
        ReqLukText,
        ReqPopText,
        Info1Text,
        Info2Text,
        Info3Text,
        Info4Text,
        Info5Text,
        Info6Text,
        Info7Text,
        Info8Text,
        Info9Text,
        Info10Text,
        Info11Text,
        Info12Text,
        Info13Text,
        Info14Text,
        Info15Text,
        Info16Text,
        Info17Text,

    }

    enum Images
    {
        ItemIcon
    }

    public InputField CountInput;


    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();


        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));


        InfoTexts.Add(Get<Text>((int)Texts.Info1Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info2Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info3Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info4Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info5Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info6Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info7Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info8Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info9Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info10Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info11Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info12Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info13Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info14Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info15Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info16Text));
        InfoTexts.Add(Get<Text>((int)Texts.Info17Text));

        foreach (Text t in InfoTexts)
            t.gameObject.SetActive(false);

        ReqInfoList = GameObject.Find("ReqInfoList").gameObject;
        JobList = GameObject.Find("JobList").gameObject;

    }


    public void SetItemInfo(int Slot)
    {

        Item = Managers.Inven.Find(
                        i => i.Slot  == Slot);



        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(Item.TemplateId, out itemData);

        Get<Text>((int)Texts.ItemNameText).text = $"[{itemData.name}]";

        Get<Text>((int)Texts.ReqLevText).text = $"REQ LEV : {Item.ReqLev}";
        Get<Text>((int)Texts.ReqStrText).text = $"REQ STR : {Item.ReqStr}";
        Get<Text>((int)Texts.ReqDexText).text = $"REQ DEX : {Item.ReqDex}";
        Get<Text>((int)Texts.ReqIntText).text = $"REQ INT : {Item.ReqInt}";
        Get<Text>((int)Texts.ReqLukText).text = $"REQ LUK : {Item.ReqLuk}";
        Get<Text>((int)Texts.ReqPopText).text = $"REQ POP : {Item.ReqPop}";

        if(Item.ReqLev <= 0)
            Get<Text>((int)Texts.ReqLevText).text = $"REQ LEV : -";
        if(Item.ReqStr <= 0)
            Get<Text>((int)Texts.ReqStrText).text = $"REQ STR : -";
        if (Item.ReqDex<= 0)
            Get<Text>((int)Texts.ReqDexText).text = $"REQ DEX : -";
        if (Item.ReqInt <= 0)
            Get<Text>((int)Texts.ReqIntText).text = $"REQ INT : -";
        if (Item.ReqLuk <= 0)
            Get<Text>((int)Texts.ReqLukText).text = $"REQ LUK : -";
        if (Item.ReqPop <= 0)
            Get<Text>((int)Texts.ReqPopText).text = $"REQ POP : -";


        if (Item.ItemType == ItemType.Weapon || Item.ItemType == ItemType.Armor)
        {

            ReqInfoList.SetActive(true);
            JobList.SetActive(true);

            NextText().text = $"업그레이드 : {Item.UpgradeSlot}";

            //if (Item.ItemType == ItemType.Weapon)
            //    NextText().text = $"공격력 : {((Weapon)Item).Damage}";
            //else
            //    NextText().text = $"방어력 : {((Armor)Item).Defence}";

            if(Item.Str >0)
                NextText().text = $"STR + {Item.Str}";

            if (Item.Dex > 0)
                NextText().text = $"DEX + {Item.Dex}";

            if (Item.Int > 0)
                NextText().text = $"INT + {Item.Int}";

            if (Item.Luk > 0)
                NextText().text = $"LUK + {Item.Luk}";

            if (Item.Hp > 0)
                NextText().text = $"HP + {Item.Hp}";

            if (Item.Mp > 0)
                NextText().text = $"MP + {Item.Mp}";

            if (Item.WAtk > 0)
                NextText().text = $"물리공격력 + {Item.WAtk}";

            if (Item.MAtk > 0)
                NextText().text = $"마법공격력 + {Item.MAtk}";

            if (Item.WDef > 0)
                NextText().text = $"물리방어력 + {Item.WDef}";

            if (Item.MDef > 0)
                NextText().text = $"마법저항력 + {Item.MDef}";

            if (Item.Speed > 0)
                NextText().text = $"이동속도 + {Item.Speed}";

            if (Item.AtkSpeed > 0)
                NextText().text = $"공격속도 + {Item.AtkSpeed}";


            if (Item.WPnt > 0)
                NextText().text = $"물리관통력 + {Item.WPnt}";

            if (Item.MPnt > 0)
                NextText().text = $"마법관통력 + {Item.MPnt}";


        }
        
        else
        {


            ReqInfoList.SetActive(false);
            JobList.SetActive(false);

            // 장비템 아닌 경우 설명창을 넣어준다.

            Text A = NextText();
            A.rectTransform.sizeDelta = new Vector2(A.rectTransform.sizeDelta.x, 30);
            A.text = itemData.desc;
        }



        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        Get<Image>((int)Texts.ItemNameText).sprite = icon;

    }

    public void AddStat()
    {

    }


    public Text NextText()
    {
        foreach(Text t in InfoTexts)
        {
            if (t.isActiveAndEnabled == false)
            {
                t.gameObject.SetActive(true);
                return t;
            }
        }

        return null;
    }
}
