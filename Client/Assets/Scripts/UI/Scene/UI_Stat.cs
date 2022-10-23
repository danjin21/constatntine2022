using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Stat : UI_Base
{

    [SerializeField]
    Transform _statHpBar = null;
    [SerializeField]
    Transform _statMpBar = null;
    [SerializeField]
    Transform _statExpBar = null;

    public void SetHpBar(int hp, int maxHp)
    {

        if (_statHpBar == null)
            return;

        float ratio = 0.0f;

        if (maxHp > 0)
        {
            // 3 / 2 = 1
            ratio = ((float)hp / maxHp);
        }

        // 0과 1사이에 두는거임
        ratio = Mathf.Clamp(ratio, 0, 1);
        _statHpBar.localScale = new Vector3(ratio, 1, 1);

    }

    public void SetMpBar(int mp, int maxMp)
    {

        if (_statMpBar == null)
            return;

        float ratio = 0.0f;

        if (maxMp > 0)
        {
            // 3 / 2 = 1
            ratio = ((float)mp / maxMp);
        }


        // 0과 1사이에 두는거임
        ratio = Mathf.Clamp(ratio, 0, 1);
        _statMpBar.localScale = new Vector3(ratio, 1, 1);

    }

    public void SetExpBar(int exp, int maxExp)
    {

        if (_statExpBar == null)
            return;

        float ratio = 0.0f;

        if (maxExp > 0)
        {
            // 3 / 2 = 1
            ratio = ((float)exp / maxExp);
        }


        // 0과 1사이에 두는거임
        ratio = Mathf.Clamp(ratio, 0, 1);
        _statExpBar.localScale = new Vector3(ratio, 1, 1);

    }


    enum Texts
    {
        NameText,
        AttackValueText,
        DefenceValueText,
        LevelValueText,
        MpText,
        HpText,
        ExpText,
        Stats01ValueText,
        Stats02ValueText,
        Stats03ValueText,
        Stats04ValueText,
        StatPointValueText,
        MapNameText,
        GoldText,
        PosText,
    }

    enum Buttons
    {
        Slot_Helmet,
        Slot_Armor,
        Slot_Boots,
        Slot_Weapon,
        Slot_Shield,
        Stats01Button,
        Stats02Button,
        Stats03Button,
        Stats04Button
    }


    bool _init = false;
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Buttons));

        GetImage((int)Buttons.Stats01Button).gameObject.BindEvent(Str_Up);
        GetImage((int)Buttons.Stats02Button).gameObject.BindEvent(Dex_Up);
        GetImage((int)Buttons.Stats03Button).gameObject.BindEvent(Int_Up);
        GetImage((int)Buttons.Stats04Button).gameObject.BindEvent(Luk_Up);


        _init = true;
        RefreshUI();
    }

    public void RefreshUI()
    {

        MyPlayerController player = Managers.Object.MyPlayer;


        if (_init == false || player==null)
            return;

        // 우선은 다 가린다. (이미지만)
        Get<Image>((int)Buttons.Slot_Helmet).enabled = false;
        //Get<Image>((int)Images.Slot_Helmet).gameObject.SetActive(false);
        Get<Image>((int)Buttons.Slot_Armor).enabled = false;
        Get<Image>((int)Buttons.Slot_Boots).enabled = false;
        Get<Image>((int)Buttons.Slot_Weapon).enabled = false;
        Get<Image>((int)Buttons.Slot_Shield).enabled = false;


        // 채워준다.
        foreach(Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(item.TemplateId, out itemData);
            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);

            if(item.ItemType == ItemType.Weapon)
            {
                Get<Image>((int)Buttons.Slot_Weapon).enabled = true;
                Get<Image>((int)Buttons.Slot_Weapon).sprite = icon;
                Get<Image>((int)Buttons.Slot_Weapon).GetComponent<UI_Stat_Item>().SetItem(item);
            }
            else if(item.ItemType == ItemType.Armor)
            {
                Armor armor = (Armor)item;
                switch (armor.ArmorType)
                {
                    case ArmorType.Helmet:
                        Get<Image>((int)Buttons.Slot_Helmet).enabled = true;
                        Get<Image>((int)Buttons.Slot_Helmet).sprite = icon;
                        Get<Image>((int)Buttons.Slot_Helmet).GetComponent<UI_Stat_Item>().SetItem(item);
                        break;
                    case ArmorType.Armor:
                        Get<Image>((int)Buttons.Slot_Armor).enabled = true;
                        Get<Image>((int)Buttons.Slot_Armor).sprite = icon;
                        Get<Image>((int)Buttons.Slot_Armor).GetComponent<UI_Stat_Item>().SetItem(item);
                        break;
                    case ArmorType.Boots:
                        Get<Image>((int)Buttons.Slot_Boots).enabled = true;
                        Get<Image>((int)Buttons.Slot_Boots).sprite = icon;
                        Get<Image>((int)Buttons.Slot_Boots).GetComponent<UI_Stat_Item>().SetItem(item);
                        break;

                }
            }
        }


        // Text

        player.RefreshAdditionalStat();

        int totalDamage = player.Stat.Attack + player.WeaponDamage;

        int minAttack = (int)((((player.TotalStr * 4 * 0.9 * 0.4) + player.TotalDex) / 100) * player.itemWAtk); 
        int maxAttack = (int)((((player.TotalStr * 4.0) + player.TotalDex) / 100) * player.itemWAtk) -1;



        //Get<Text>((int)Texts.NameText).text = /*player.name;*/$"ID : {Managers.Network.AccountName}";
        Get<Text>((int)Texts.NameText).text = "" + player.name;
        //Get<Text>((int)Texts.AttackValueText).text = $"{totalDamage} ({player.Stat.Attack}+{player.WeaponDamage})";

        Get<Text>((int)Texts.AttackValueText).text = $"{minAttack} - {maxAttack}";
        Get<Text>((int)Texts.DefenceValueText).text = $"{player.ArmorDefence+ player.itemWDef}";

        Get<Text>((int)Texts.MpText).text = $"{player.Stat.Mp}/{player.Stat.MaxMp}";
        Get<Text>((int)Texts.HpText).text = $"{player.Stat.Hp}/{player.Stat.MaxHp}";
        float a = (float)player.Stat.Exp / (float)player.Stat.TotalExp;
        Debug.Log("+++" + a + "/" + player.Stat.Exp +"/"+ player.Stat.TotalExp);
        Get<Text>((int)Texts.ExpText).text = $"{player.Stat.Exp} <color=#13ad65>[</color>{string.Format("{0:N2}", a * 100)}%<color=#13ad65>]</color>";
        Get<Text>((int)Texts.Stats01ValueText).text = $"{player.TotalStr}({player.Stat.Str}+{player.itemStr})";
        Get<Text>((int)Texts.Stats02ValueText).text = $"{player.TotalDex}({player.Stat.Dex}+{player.itemDex})";
        Get<Text>((int)Texts.Stats03ValueText).text = $"{player.TotalInt}({player.Stat.Int}+{player.itemInt})";
        Get<Text>((int)Texts.Stats04ValueText).text = $"{player.TotalLuk}({player.Stat.Luk}+{player.itemLuk})";
        Get<Text>((int)Texts.StatPointValueText).text = $"{player.Stat.StatPoint}";
        Get<Text>((int)Texts.LevelValueText).text = $"{player.Stat.Level}";
        Get<Text>((int)Texts.GoldText).text = $"{player.Stat.Gold}";

        MapInfoData mapData = null;
        Managers.Data.MapDict.TryGetValue(player.Stat.Map, out mapData);
        Get<Text>((int)Texts.MapNameText).text = $"{mapData.name}";

        if (player.Stat.StatPoint > 0)
            StatButtonOnOff(true);
        else
            StatButtonOnOff(false);

        // 게이지 업데이트
        SetHpBar(player.Stat.Hp, player.Stat.MaxHp);
        SetMpBar(player.Stat.Mp, player.Stat.MaxMp);
        SetExpBar(player.Stat.Exp, player.Stat.TotalExp);


    }


    public void RefreshUI_HpMp()
    {

        MyPlayerController player = Managers.Object.MyPlayer;
        Get<Text>((int)Texts.MpText).text = $"{player.Stat.Mp}/{player.Stat.MaxMp}";
        Get<Text>((int)Texts.HpText).text = $"{player.Stat.Hp}/{player.Stat.MaxHp}";


        // 게이지 업데이트
        SetHpBar(player.Stat.Hp, player.Stat.MaxHp);
        SetMpBar(player.Stat.Mp, player.Stat.MaxMp);
    }

    public void RefreshUI_Position(int x, int y)
    {
        Get<Text>((int)Texts.PosText).text = $"X:{x},Y:{y}";
    }

    public void Str_Up(PointerEventData evt)
    {
        Stat_Up(1);
    }

    public void Dex_Up(PointerEventData evt)
    {
        Stat_Up(2);
    }

    public void Int_Up(PointerEventData evt)
    {
        Stat_Up(3);
    }

    public void Luk_Up(PointerEventData evt)
    {
        Stat_Up(4);
    }

    public void Stat_Up(int stat)
    {
        // Sound
        //Managers.Sound.Play("UI/Button/Slick Button", Define.Sound.Effect);

        C_StatUp statupPacket = new C_StatUp();
        statupPacket.Stat = stat;

        Managers.Network.Send(statupPacket);
    }

    public void StatButtonOnOff(bool boolean)
    {
        GetImage((int)Buttons.Stats01Button).gameObject.SetActive(boolean);
        GetImage((int)Buttons.Stats02Button).gameObject.SetActive(boolean);
        GetImage((int)Buttons.Stats03Button).gameObject.SetActive(boolean);
        GetImage((int)Buttons.Stats04Button).gameObject.SetActive(boolean);
    }




}
