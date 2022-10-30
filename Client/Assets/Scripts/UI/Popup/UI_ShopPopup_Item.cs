using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopPopup_Item : UI_Base, IPointerClickHandler
{

    public UI_ShopPopup UI_ShopPopup;

    [SerializeField]
    Image _icon = null;

    [SerializeField]
    Image _frame = null;


    public ItemInfo Info { get; set; }

    enum Buttons
    {
    }

    //enum Texts
    //{
    //    NameText
    //}

    public GameObject ItemInfoPopup;

    public void OnDisable()
    {
        GameObject.Destroy(ItemInfoPopup);
        ItemInfoPopup = null;
    }



    public override void Init()
    {
        Bind<Image>(typeof(Buttons));
        //Bind<Text>(typeof(Texts));

        //GetButton((int)Buttons.SelectServerButton).gameObject.BindEvent(OnClickButton);

        //GetImage((int)Buttons.DropButton).gameObject.BindEvent(OnClickDropButton);
        //GetImage((int)Buttons.DropButton).gameObject.SetActive(false);


        // 상점창 열면 아이템창도 갱신
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();


        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템에 들어왔따.");


            if (ItemInfoPopup == null)
            {
                GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_ItemInfoPopup");
                ItemInfoPopup = go;

                // 아이템 정보는 Inventory 에서 갖고오라고 하고, 여기는 그냥 Slot만 넘겨준다.
                ItemInfoPopup.GetComponent<UI_ItemInfoPopup>().SetItemInfo_TemplateId(Info.TemplateId, Info);

                ItemInfoPopup.transform.GetChild(0).transform.position = this.transform.position;

                //Debug.Log(" 1) 설명창의 위치 :" + ItemInfoPopup.transform.GetChild(0).transform.position);

                //Debug.Log(" 2) 아이템창의 위치 :" + this.transform.position);
            }


        }, Define.UIEvent.Enter);

        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템으로부터 나갔다.");

            GameObject.Destroy(ItemInfoPopup);
            ItemInfoPopup = null;



        }, Define.UIEvent.Exit);


        _icon.gameObject.BindEvent((e) =>
        {
            // 왼쪽 클릭이 아니면 리턴
            if (e.button == PointerEventData.InputButton.Left)
            {

                OnMouseDoubleClick();

            }
            //else if (e.button == PointerEventData.InputButton.Right)
            //{

         
            //}
        }, Define.UIEvent.Click);


    }

    public void RefreshUI()
    {
        //if (Info == null)
        //    return;

        //GetText((int)Texts.NameText).text = Info.Name;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(Info.TemplateId, out itemData);

        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        _icon.sprite = icon;

        _icon.gameObject.SetActive(true);

    }

    void OnClickButton(PointerEventData evt)
    {

        //Managers.Network.ConnectToGame(Info);
        //Managers.Scene.LoadScene(Define.Scene.Game);
        //Managers.UI.ClosePopupUI();
    }


    float clickTIme = 0;
    //private void OnPointerClick(PointerEventData eventData)
    //{
    //    if ((Time.time - clickTIme) < 0.3f)
    //    {
    //        OnMouseDoubleClick();
    //        clickTIme = -1;
    //    }
    //    else
    //    {
    //        clickTIme = Time.time;
    //    }
    //}

    void OnMouseDoubleClick()
    {

        UI_ShopPopup.nowItem = Info;
        UI_ShopPopup.OKAlarm(true);

    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnMouseDoubleClick();
        //if ((Time.time - clickTIme) < 0.3f)
        //{
        //    OnMouseDoubleClick();
        //    clickTIme = -1;
        //}
        //else
        //{
        //    clickTIme = Time.time;
        //}
    }
}
