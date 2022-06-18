using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerPopup : UI_Popup
{
    public List<UI_SelectServerPopup_Item> Items { get; } = new List<UI_SelectServerPopup_Item>();



    enum Buttons
    {
        LogoutBtn
    }



    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.LogoutBtn).gameObject.BindEvent(OnClickLogoutButton);



    }

    public void SetServers(List<ServerInfo> servers)
    {
        Items.Clear();

        // 다지워주고
        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        // 다 생성해주고.
        for (int i = 0; i < servers.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_SelectServerPopup_Item", grid.transform);
            UI_SelectServerPopup_Item item = go.GetOrAddComponent<UI_SelectServerPopup_Item>();
            Items.Add(item);

            item.Info = servers[i];
        }

        RefreshUI();
    }

    public void RefreshUI()
    {

        if (Items.Count == 0)
            return;


        foreach(var item in Items)
        {
            item.RefreshUI();
        }

    }

    void OnClickLogoutButton(PointerEventData evt)
    {


        // 위험할수도 있다.
        LogoutAccountPacketReq packet = new LogoutAccountPacketReq()
        {
            AccountName = Managers.Network.AccountName,
            Password = Managers.Network.Password
        };


        Managers.Web.SendPostRequest<LogoutAccountPacketRes>("account/logout", packet, (res) =>
        {

            Debug.Log("result = " + res.LogoutOk);

            // res = LoginAccountPacketRes
            if (res.LogoutOk)
            {
                Managers.UI.ClosePopupUI(this);
            }

        });



    }


}
