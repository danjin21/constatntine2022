using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Skill_Item : UI_Base
{
    [SerializeField]
    Image _icon = null;


    [SerializeField]
    Image _frame = null;

    [SerializeField]
    Text _count = null;


    public int SkillDbId { get; private set; }
    public int SkillId { get; private set; }
    public int SkillLevel { get; private set; }
    public int Slot { get; private set; }

    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;

    public GameObject ItemInfoPopup;


    enum Buttons
    {
        DropButton,
        SellButton,
    }


    //  한박자 느리게 해야 아이템 먹어지는게 초기화된다.
    public void LateUpdate()
    {


        //  한번만 클릭했을 때 KeySetting
        //  0.25 초 이상되면 원상태로
        if (m_IsOneClick && ((Time.time - m_Timer) > m_DoubleClickSecond))
        {
            m_IsOneClick = false;


            Managers.KeySetting.ChangeKey = SkillId;

            // 커서 이미지
            CursorManager obj = FindObjectOfType((typeof(CursorManager))) as CursorManager;
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.transform.GetChild(0).GetComponent<Image>().sprite = _icon.sprite;


        }

        //Managers.KeySetting.SetKeySetting(this.gameObject, SkillId);

    }


    public override void Init()
    {

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.DropButton).gameObject.BindEvent(OnClickDropButton);
        GetImage((int)Buttons.DropButton).gameObject.SetActive(false);

        GetImage((int)Buttons.SellButton).gameObject.BindEvent(OnClickSellButton);
        GetImage((int)Buttons.SellButton).gameObject.SetActive(false);

        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템에 들어왔따.");


            if (ItemInfoPopup == null && SkillId != 0)
            {
                GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_ItemInfoPopup");
                ItemInfoPopup = go;

                // 아이템 정보는 Inventory 에서 갖고오라고 하고, 여기는 그냥 Slot만 넘겨준다.
                //ItemInfoPopup.GetComponent<UI_ItemInfoPopup>().SetItemInfo(Slot);

                ItemInfoPopup.GetComponent<UI_ItemInfoPopup>().SetItemInfo_Skill_Consume(SkillId);

                //ItemInfoPopup.transform.GetChild(0).transform.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                ItemInfoPopup.transform.GetChild(0).transform.position = this.transform.position;

                //Debug.Log(" 1) 설명창의 위치 :" + ItemInfoPopup.transform.GetChild(0).transform.position);

                //Debug.Log(" 2) 아이템창의 위치 :" + this.transform.position);
            }

            // 현재 커서 여기있다고 말해주기.
            Managers.KeySetting.CurrentCursor = 1;


        }, Define.UIEvent.Enter);

        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템으로부터 나갔다.");

            //GameObject.Destroy(ItemInfoPopup);
            //ItemInfoPopup = null;

            GameObject.Destroy(ItemInfoPopup);
            ItemInfoPopup = null;

            // 현재 커서 나갔다고 말해주기.
            Managers.KeySetting.CurrentCursor = -1;

        }, Define.UIEvent.Exit);



        _icon.gameObject.BindEvent((e) =>
        {
            // 왼쪽 클릭이 아니면 리턴
            if (e.button == PointerEventData.InputButton.Left)
            {


                if (!m_IsOneClick)
                {
                    // 더블클릭 전 한번
                    m_Timer = Time.time;
                    m_IsOneClick = true;
                }
                else if (m_IsOneClick && ((Time.time-m_Timer)<m_DoubleClickSecond))
                {
                    // 더블클릭 되는 순간
                    m_IsOneClick = false;

                    Data.Skill skillData = null;
                    Managers.Data.SkillDict.TryGetValue(SkillId, out skillData);

                    //if (itemData == null)
                    //    return;

                    //// TODO : C_USE_ITEM 아이템 사용 패킷
                    //if (itemData.itemType == ItemType.Consumable)
                    //{
                    //    Debug.Log($"Click Potion + {ItemDbId}");
                    //    C_UseItem usePacket = new C_UseItem();
                    //    usePacket.ItemDbId = ItemDbId;

                    //    Managers.Network.Send(usePacket);
                    //    return;
                    //}


                    //C_EquipItem equipPacket = new C_EquipItem();
                    //equipPacket.ItemDbId = ItemDbId;
                    //equipPacket.Equipped = !Equipped;

                    //Managers.Network.Send(equipPacket);
                }


            }
            else if (e.button == PointerEventData.InputButton.Right)
            {

                //if (transform.GetChild(5).transform.gameObject.activeSelf) // Drop
                //{
                //    transform.GetChild(5).transform.gameObject.SetActive(false); // Drop
                //}
                //else
                //{

                //    foreach (Transform child in transform.parent)
                //        child.GetChild(4).transform.gameObject.SetActive(false);

                //    foreach (Transform child in transform.parent)
                //        child.GetChild(5).transform.gameObject.SetActive(false); // Drop

                //    transform.GetChild(5).transform.gameObject.SetActive(true); // Drop
                //}


            }
        }, Define.UIEvent.Click);
    }


    public void CloseButtons()
    {
        for(int i = 4; i<7; i++)
        transform.GetChild(i).transform.gameObject.SetActive(false); 

    }


    public void SetSkill(Skills skill)
    {

        if (skill == null)
        {
            SkillDbId = 0;
            SkillId = 0;
            SkillLevel = 0;
            Slot = -1;

            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }
        else
        {

            SkillDbId = skill.SkillDbId;
            SkillId = skill.SkillId;
            SkillLevel = skill.SkillLevel;
            Slot = skill.Slot;


            Data.Skill skillData = null;
            Managers.Data.SkillDict.TryGetValue(SkillId, out skillData);

            // 없으면 크래쉬

            Sprite icon = Managers.Resource.Load<Sprite>(skillData.iconPath);
            _icon.sprite = icon;

            _icon.gameObject.SetActive(true);



            _count.gameObject.SetActive(true);
            _count.text = "Lv " +SkillLevel.ToString();

        }

        CloseButtons();
    }

    void OnClickDropButton(PointerEventData evt)
    {
        //// 멈춰있을때만 버릴 수 있게함.
        //if (Managers.Object.MyPlayer.State != CreatureState.Idle)
        //    return;

        ////C_DropItem dropItemPacket = new C_DropItem();

        //GameObject A = GetImage((int)Buttons.DropButton).gameObject;
        //A.SetActive(false);

        //UI_DropCountPopup popup = Managers.UI.ShowPopupUI<UI_DropCountPopup>();

        //ItemInfo item = new ItemInfo();

        //item.ItemDbId = ItemDbId;
        //item.Count = Count;
        //item.TemplateId = TemplateId;
        //item.Equipped = Equipped;

        //popup.nowItem = item;

        //Debug.Log("버리기 ! ");

    }

    void OnClickSellButton(PointerEventData evt)
    {
        //GetImage((int)Buttons.SellButton).gameObject.SetActive(false);
    }


}
