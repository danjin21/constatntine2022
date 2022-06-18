using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopPopup : UI_Popup
{
    public List<UI_ShopPopup_Item> Items { get; } = new List<UI_ShopPopup_Item>();

    public ItemInfo nowItem;
    public ObjectInfo npcInfo;

    enum Buttons
    {
        CloseBtn,
        OkBtn,
    }

    enum Texts
    {
        OKQuestionText
    }


    enum GameObjects
    {
        Count
    }

    public InputField CountInput;

    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.CloseBtn).gameObject.BindEvent(OnClickCloseButton);
        GetImage((int)Buttons.OkBtn).gameObject.BindEvent(OnClickOkButton);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        CountInput = Get<GameObject>((int)GameObjects.Count).GetComponent<InputField>();


        OKAlarm(false);
    }

    public void OKAlarm(bool boolean)
    {
        // 초기에 확답 팝업 삭제함
        CountInput.gameObject.SetActive(boolean);
        GetImage((int)Buttons.OkBtn).gameObject.SetActive(boolean);
        GetText((int)Texts.OKQuestionText).gameObject.SetActive(boolean);

        if (nowItem != null)
        {
            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(nowItem.TemplateId, out itemData);

            // ItemDBId 가 있다는 것은 소유한 아이템을 클릭했따는 의미이다. ( 상점에서 파는 템은 DbId가 0이다)
            if (nowItem.ItemDbId > 0)
            {
                Color color;
                ColorUtility.TryParseHtmlString("#54C095", out color);
                GetImage((int)Buttons.OkBtn).gameObject.GetComponent<Image>().color = color;
            
                GetText((int)Texts.OKQuestionText).text = $"<color=#FFFFFF>{itemData.name}</color>" + $" <color=#F9E79F><size=11> (​{(int)(itemData.sellPrice)}G)</size></color>" + "\n정말 판매하시겠어요? (갯수)";
            }
            else
            {
                Color color;
                ColorUtility.TryParseHtmlString("#FF7272", out color);
                GetImage((int)Buttons.OkBtn).gameObject.GetComponent<Image>().color = color;

                GetText((int)Texts.OKQuestionText).text = $"<color=#FFFFFF>{itemData.name}</color>" + $"<color=#F9E79F><size=11> (​{itemData.price}G)</size></color>" + "\n정말 구매하시겠어요? (갯수)";

                // 상점창 닫으면 아이템창도 갱신
                UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
                UI_Inventory invenUI = gameSceneUI.InvenUI;
                invenUI.RefreshUI();
            }
        }


    }

    public void SetProducts(List<ItemInfo> products)
    {
        Items.Clear();

        // 다지워주고
        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        // 다 생성해주고.
        for (int i = 0; i < products.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_ShopPopup_Item", grid.transform);
            UI_ShopPopup_Item item = go.GetOrAddComponent<UI_ShopPopup_Item>();
            Items.Add(item);

            item.Info = products[i];
            item.UI_ShopPopup = this;
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

    void OnClickCloseButton(PointerEventData evt)
    {



        Managers.UI.ClosePopupUI(this);

        
        // 상점창 닫으면 아이템창도 갱신
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();

    }

    void OnClickOkButton(PointerEventData evt)
    {

        // 상점창 닫으면 아이템창도 갱신
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        invenUI.RefreshUI();

        OKAlarm(false);

        // ItemDBId 가 있다는 것은 소유한 아이템을 클릭했따는 의미이다. ( 상점에서 파는 템은 DbId가 0이다)
        if (nowItem.ItemDbId > 0)
        {
            C_Sell sellPacket = new C_Sell();

            sellPacket.ItemDbId = nowItem.ItemDbId;
            sellPacket.NpcId = npcInfo.ObjectId;

            if (CountInput.text == string.Empty)
                sellPacket.Count = 1;
            else
                sellPacket.Count = int.Parse(CountInput.text);

            Managers.Network.Send(sellPacket);

        }
        else
        {

            C_Purchase purchasePacket = new C_Purchase();

            purchasePacket.TemplateId = nowItem.TemplateId;
            purchasePacket.NpcId = npcInfo.ObjectId;

            if (CountInput.text == string.Empty)
                purchasePacket.Count = 1;
            else
                purchasePacket.Count = int.Parse(CountInput.text);


            Managers.Network.Send(purchasePacket);
        }




    }
    
}
