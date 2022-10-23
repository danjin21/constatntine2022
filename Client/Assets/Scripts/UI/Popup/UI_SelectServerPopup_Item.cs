using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerPopup_Item : UI_Base
{
    public ServerInfo Info { get; set; }

    enum Buttons
    {
        SelectServerButton
    }

    enum Texts
    {
        NameText
    }




    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SelectServerButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null)
            return;

        GetText((int)Texts.NameText).text = Info.Name;
    }

    void OnClickButton(PointerEventData evt)
    {


        StartCoroutine(Delay());
    }


    IEnumerator Delay()
    {
        // Sound
        //Managers.Sound.Play("UI/Button/Slick Button", Define.Sound.Effect);

        yield return new WaitForSeconds(0.5f);

        Managers.Network.ConnectToGame(Info);
        Managers.Scene.LoadScene(Define.Scene.Game);
        Managers.UI.ClosePopupUI();


    }


}
