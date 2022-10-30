using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
    [SerializeField]
    Image _icon = null;


    [SerializeField]
    Image _frame = null;

    [SerializeField]
    Text _count = null;

    [SerializeField]
    Image _backgroundImage = null;
    public int EmptySlot = -1;

    public int ItemDbId { get; private set; }
    public int TemplateId { get; private set; }
    public int Count { get; private set; }
    public bool Equipped { get; private set; }
    public int Slot { get; private set; }

    public float m_DoubleClickSecond = 0.5f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;

    public GameObject ItemInfoPopup;

    public int SlotOrder { get; private set; }

    enum Buttons
    {
        DropButton,
        SellButton,
    }

    public void OnDisable()
    {
        GameObject.Destroy(ItemInfoPopup);
        ItemInfoPopup = null;
    }



    //  한박자 느리게 해야 아이템 먹어지는게 초기화된다.
    public void LateUpdate()
    {


        //  한번만 클릭했을 때 KeySetting
        //  0.25 초 이상되면 원상태로
        if (m_IsOneClick && ((Time.time - m_Timer) > m_DoubleClickSecond))
        {

            m_IsOneClick = false;

            if (Managers.KeySetting.ChangeItemDbId != -1)
            {
                Debug.Log("이전 슬롯의 Db : " + Managers.KeySetting.ChangeItemDbId + "/ 현재 슬롯의 Db :" + ItemDbId);

                {
                    //// 해당 DBId 의 슬롯 갖고오기
                    //Item A = Managers.Inven.Get(Managers.KeySetting.ChangeItemDbId);

                    C_SlotChange slotchangePacket = new C_SlotChange();
                    slotchangePacket.ItemDbId = Managers.KeySetting.ChangeItemDbId;
                    slotchangePacket.Slot = Slot;

                    Managers.Network.Send(slotchangePacket);
                }

                Managers.KeySetting.ChangeKey = -1;
                Managers.KeySetting.ChangeItemDbId = -1;
                return;
            }



            Managers.KeySetting.ChangeKey = TemplateId;
            Managers.KeySetting.ChangeItemDbId = ItemDbId;


            // 커서 이미지
            CursorManager obj = FindObjectOfType((typeof(CursorManager))) as CursorManager;



            if (obj.transform.GetChild(0).gameObject.activeInHierarchy == false)
            {
                obj.transform.GetChild(0).gameObject.SetActive(true);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = _icon.sprite;
            }








            //C_SlotChange slotchangePacket = new C_SlotChange();
            //slotchangePacket.ItemDbId = ItemDbId;
            //slotchangePacket.Slot = 0;

            //Managers.Network.Send(slotchangePacket);



        }


        if (Input.GetMouseButtonUp(0))
        {
            // 단축기 아이콘에 커서가 없다면 다 그냥 지워준다.
            if (Managers.KeySetting.CurrentCursor != 2)
            {
                CursorManager obj2 = FindObjectOfType((typeof(CursorManager))) as CursorManager;
                obj2.transform.GetChild(0).gameObject.SetActive(false);

                // 빈 공간에 버리는 경우 & 아이템인 경우 버릴건지 물어본다.
                if (Managers.KeySetting.CurrentCursor == -1)
                {
                    // 종류가 같고,
                    if (Managers.KeySetting.ChangeKey == TemplateId)
                    {
                        // DbID 까지 같을때
                        if (Managers.KeySetting.ChangeItemDbId == ItemDbId)
                        {
                            // UI 일때만 버린다
                            if (EventSystem.current.IsPointerOverGameObject() == false)
                            {
                                ItemDrop();
                          
                            }

                            Managers.KeySetting.ChangeKey = -1;
                            Managers.KeySetting.ChangeItemDbId = -1;


                        }
                    }
                }
                //// 아이템 인벤창
                //else if(Managers.KeySetting.CurrentCursor == 0)
                //{
                //    Debug.Log("옹");
                //}



            }
            else
            {
            }
        }




    }


    public override void Init()
    {
        m_DoubleClickSecond = 0.3f;

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.DropButton).gameObject.BindEvent(OnClickDropButton);
        GetImage((int)Buttons.DropButton).gameObject.SetActive(false);

        GetImage((int)Buttons.SellButton).gameObject.BindEvent(OnClickSellButton);
        GetImage((int)Buttons.SellButton).gameObject.SetActive(false);



        // 배경이미지

        _backgroundImage = transform.GetChild(0).GetComponent<Image>();

        _backgroundImage.gameObject.BindEvent((e) =>
        {
            // 왼쪽 클릭이 아니면 리턴
            if (e.button == PointerEventData.InputButton.Left)
            {
                Debug.Log($"클릭된 슬롯은 {Slot}입니다.");


                // 서버에서, 빈칸일때 슬롯 바꾸는거 만들고 처리해줘야함

                C_SlotChange slotchangePacket = new C_SlotChange();
                slotchangePacket.ItemDbId = Managers.KeySetting.ChangeItemDbId;
                slotchangePacket.Slot = SlotOrder;

                Managers.Network.Send(slotchangePacket);

                Managers.KeySetting.ChangeKey = -1;
                Managers.KeySetting.ChangeItemDbId = -1;

            }

        }, Define.UIEvent.Click);


        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템에 들어왔따.");


            if (ItemInfoPopup == null)
            {
                GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_ItemInfoPopup");
                ItemInfoPopup = go;

                // 아이템 정보는 Inventory 에서 갖고오라고 하고, 여기는 그냥 Slot만 넘겨준다.
                ItemInfoPopup.GetComponent<UI_ItemInfoPopup>().SetItemInfo(Slot);

                ItemInfoPopup.transform.GetChild(0).transform.position = this.transform.position;

                //Debug.Log(" 1) 설명창의 위치 :" + ItemInfoPopup.transform.GetChild(0).transform.position);

                //Debug.Log(" 2) 아이템창의 위치 :" + this.transform.position);
            }

            // 현재 커서 여기있다고 말해주기.
            Managers.KeySetting.CurrentCursor = 0;

        }, Define.UIEvent.Enter);

        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템으로부터 나갔다.");

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
                GameObject UI_ShopPopup = GameObject.Find("UI_ShopPopup");

                if (UI_ShopPopup != null)
                {



                    foreach (Transform child in transform.parent)
                    {
                        if (child != this.transform)
                            child.GetChild(6).transform.gameObject.SetActive(false);
                    }

                    transform.GetChild(6).transform.gameObject.SetActive(!transform.GetChild(6).transform.gameObject.activeSelf); // Drop

                    if(transform.GetChild(6).transform.gameObject.activeSelf)
                    {
                        ItemInfo sellItem = new ItemInfo();
                        sellItem.ItemDbId = ItemDbId;
                        sellItem.TemplateId = TemplateId;

                        UI_ShopPopup Shop = UI_ShopPopup.GetComponent<UI_ShopPopup>();

                        Shop.nowItem = sellItem;
                        Shop.OKAlarm(true);
                    }
                    else
                    {

                        UI_ShopPopup Shop = UI_ShopPopup.GetComponent<UI_ShopPopup>();

                        Shop.nowItem = null;
                        Shop.OKAlarm(false);
                    }
                    return;
                }


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

                    Data.ItemData itemData = null;
                    Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);

                    if (itemData == null)
                        return;

                    // TODO : C_USE_ITEM 아이템 사용 패킷
                    if (itemData.itemType == ItemType.Consumable)
                    {
                        Debug.Log($"Click Potion + {ItemDbId}");
                        C_UseItem usePacket = new C_UseItem();
                        usePacket.ItemDbId = ItemDbId;

                        Managers.Network.Send(usePacket);
                        return;
                    }


                    C_EquipItem equipPacket = new C_EquipItem();
                    equipPacket.ItemDbId = ItemDbId;
                    equipPacket.Equipped = !Equipped;

                    Managers.Network.Send(equipPacket);
                }


            }
            else if (e.button == PointerEventData.InputButton.Right)
            {

                GameObject UI_ShopPopup = GameObject.Find("UI_ShopPopup");

                if (UI_ShopPopup != null)
                {

                    return;
                }


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


    public void SetItem(Item item)
    {

        if (item == null)
        {
            ItemDbId = 0;
            TemplateId = 0;
            Count = 0;
            Equipped = false;
            Slot = -1;

            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }
        else
        {

            ItemDbId = item.ItemDbId;
            TemplateId = item.TemplateId;
            Count = item.Count;
            Equipped = item.Equipped;
            Slot = item.Slot;


            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);

            // 없으면 크래쉬

            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            _icon.sprite = icon;

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(Equipped);

            // 90000 이상이 포션임.
            if (TemplateId >= 90000)
            {
                _count.gameObject.SetActive(true);
                _count.text = Count.ToString();
            }
        }

        CloseButtons();



        // Inventory 창의 순서를 찾는다. =======================================

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        int order = 0;

        foreach (UI_Inventory_Item t in invenUI.Items)
        {
            if (t == this)
            {
                SlotOrder = order;
                break;
            }

            order++;
        }

        // Inventory 창의 순서를 찾는다. 끝  =======================================


    }

    void OnClickDropButton(PointerEventData evt)
    {
        ItemDrop();

    }


    void ItemDrop()
    {
        // 멈춰있을때만 버릴 수 있게함.
        if (Managers.Object.MyPlayer.State != CreatureState.Idle)
            return;

        //C_DropItem dropItemPacket = new C_DropItem();

        GameObject A = GetImage((int)Buttons.DropButton).gameObject;
        A.SetActive(false);

        UI_DropCountPopup popup = Managers.UI.ShowPopupUI<UI_DropCountPopup>();

        ItemInfo item = new ItemInfo();

        item.ItemDbId = ItemDbId;
        item.Count = Count;
        item.TemplateId = TemplateId;
        item.Equipped = Equipped;

        popup.nowItem = item;

        Debug.Log("버리기 ! ");

    }




    void OnClickSellButton(PointerEventData evt)
    {
        //GetImage((int)Buttons.SellButton).gameObject.SetActive(false);
    }










}
