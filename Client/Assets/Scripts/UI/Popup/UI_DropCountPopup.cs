using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DropCountPopup : UI_Popup
{

    public ItemInfo nowItem;

    enum Buttons
    {
        OkBtn,
        NoBtn,
    }

    enum Texts
    {
        NoticeText,
    }

    enum GameObjects
    {
        DropCount
    }

    public InputField CountInput;


    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.OkBtn).gameObject.BindEvent(OnClickOkButton);
        GetImage((int)Buttons.NoBtn).gameObject.BindEvent(OnClickNoButton);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        CountInput = Get<GameObject>((int)GameObjects.DropCount).GetComponent<InputField>();

    }
    



    void OnClickOkButton(PointerEventData evt)
    {
        // 멈춰있을때만 버릴 수 있게함.
        if (Managers.Object.MyPlayer.State != CreatureState.Idle)
            return;


        C_DropItem dropItemPacket = new C_DropItem();
        dropItemPacket.ItemDbId = nowItem.ItemDbId;

        if (CountInput.text == string.Empty)
            dropItemPacket.Count = 1;
        else
            dropItemPacket.Count = int.Parse(CountInput.text);

        Managers.Network.Send(dropItemPacket);

        Managers.UI.ClosePopupUI(this);
    }

    void OnClickNoButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
    }


}
