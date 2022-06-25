using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UserListPopup : UI_Popup
{

    public List<ObjectInfo> UserList = new List<ObjectInfo>();

    enum Buttons
    {
        CloseBtn,

    }

    enum Texts
    {
    }

    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.CloseBtn).gameObject.BindEvent(OnClickCloseButton);


        GetImage((int)Buttons.CloseBtn).gameObject.SetActive(true);


        Bind<Text>(typeof(Texts));

    }

    public void closeAllButtons()
    {
        GetImage((int)Buttons.CloseBtn).gameObject.SetActive(false);

    }

    void OnClickCloseButton(PointerEventData evt)
    {
        // Sound
        Managers.Sound.Play("UI/Button/Slick Button", Define.Sound.Effect);

        Managers.UI.ClosePopupUI(this);

    }

    public void RefreshUsers()
    {
        GameObject verticalGrid = GetComponentInChildren<VerticalLayoutGroup>().gameObject;

        foreach (Transform child in verticalGrid.transform)
            Destroy(child.gameObject);

        int i = 0;

        foreach (ObjectInfo User in UserList)
        {
            MapInfoData mapData = null;
            Managers.Data.MapDict.TryGetValue(User.StatInfo.Map, out mapData);

            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_UserListPopup_Item", verticalGrid.transform);

            go.GetComponent<Text>().text = $" {i+1} - 접속 중인 유저 : {User.Name} / 레벨 : {User.StatInfo.Level} / 위치 : {mapData.name}";
            i++;
        }
    }



}
