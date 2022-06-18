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
