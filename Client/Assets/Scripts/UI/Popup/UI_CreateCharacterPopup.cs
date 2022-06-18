using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CreateCharacterPopup : UI_Popup
{
    public List<UI_SelectServerPopup_Item> Items { get; } = new List<UI_SelectServerPopup_Item>();

    public int Hair;
    public int HairColor;
    public int Face;
    public int Skin;
    public int Gender;

    public int Hair_End;
    public int Hair_Color_End;
    public int Face_End;
    public int Skin_End;


    public List<Sprite> HeadList = new List<Sprite>();
    public List<Sprite> FaceList = new List<Sprite>();
    public List<Sprite> HairList = new List<Sprite>();
    public List<Sprite> BodyList = new List<Sprite>();


    enum GameObjects
    {
        PlayerName
    }

    enum Buttons
    {
        //LogoutBtn,
        CreateBtn,

    }




    public override void Init()
    {
        // 팝업 Sorting 하는 부분임
        base.Init();

        //Bind<Image>(typeof(Buttons));
        //GetImage((int)Buttons.LogoutBtn).gameObject.BindEvent(OnClickLogoutButton);
        Bind<GameObject>(typeof(GameObjects));

        Bind<Image>(typeof(Buttons));
        GetImage((int)Buttons.CreateBtn).gameObject.BindEvent(OnClickCreateButton);

        Hair = 0;
        HairColor = 0;
        Face = 0;
        Skin = 0;
        Gender = 0;

        Hair_End = 3;
        Hair_Color_End =1 ;
        Face_End = 5;
        Skin_End =1;

        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Body/1") as Sprite);

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/1") as Sprite); //0
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/2") as Sprite); //3
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/3") as Sprite); //6
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/4") as Sprite); //6
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/5") as Sprite); //6
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/6") as Sprite); //6


        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/1") as Sprite); // 0
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/2") as Sprite); // 1
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/3") as Sprite); // 2
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/4") as Sprite); // 3

        HeadList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Head/1") as Sprite);

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

    void OnClickCreateButton(PointerEventData evt)
    {
        string account = Get<GameObject>((int)GameObjects.PlayerName).GetComponent<InputField>().text;


        C_CreatePlayer createPacket = new C_CreatePlayer();
        //createPacket.Name = $"PLAYER_{Random.Range(0, 10000).Tostring("0000")}";
        createPacket.Name = account;
        createPacket.Face = Face;
        createPacket.Hair = Hair;
        createPacket.Skin = Skin;
        createPacket.Gender = Gender;

        Managers.Network.Send(createPacket); // 서버한테 캐릭터 만들어달라고 요청


        Managers.UI.ClosePopupUI(this);
    }


    //void OnClickLogoutButton(PointerEventData evt)
    //{
    //        LogoutAccountPacketReq packet = new LogoutAccountPacketReq()
    //        {
    //            AccountName = Managers.Network.AccountName,
    //            Password = Managers.Network.Password
    //        };

    //    Managers.Web.SendPostRequest<LogoutAccountPacketRes>("account/logout", packet, (res) =>
    //    {

    //        Debug.Log("result = " + res.LogoutOk);

    //        // res = LoginAccountPacketRes
    //        if (res.LogoutOk)
    //        {
    //            Managers.UI.ClosePopupUI();
    //        }

    //    });

    //}




    public void HairButton (int direction)
    {
        if(direction == -1)
        {
            Hair -= 1;
            if (Hair == -1)
                Hair = Hair_End;
        }
        else if(direction == 1)
        {
            Hair += 1;
            if (Hair == Hair_End+1)
                Hair = 0;
        }

        transform.Find("Character/Hair/Number").GetComponent<Text>().text = Hair.ToString();

        transform.Find("MyPlayer/Hair").GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/" + (Hair+1)) as Sprite;

        // 헤어의 사이즈만큼 이미지의 사이즈도 늘려준다.,
        transform.Find("MyPlayer/Hair").GetComponent<RectTransform>().sizeDelta = new Vector2( HairList[Hair].rect.size.x , HairList[Hair].rect.size.y);

        

    }


    public void HairColorButton(int direction)
    {
        if (direction == -1)
        {
            HairColor -= 1;
            if (HairColor == -1)
                HairColor = Hair_Color_End;
        }
        else if (direction == 1)
        {
            HairColor += 1;
            if (HairColor == Hair_Color_End+1)
                HairColor = 0;
        }

        transform.Find("Character/HairColor/Number").GetComponent<Text>().text = HairColor.ToString();

     

    }


    public void FaceButton(int direction)
    {
        if (direction == -1)
        {
            Face -= 1;
            if (Face == -1)
                Face = Face_End;
        }
        else if (direction == 1)
        {
            Face += 1;
            if (Face == Face_End+1)
                Face = 0;
        }
        transform.Find("Character/Face/Number").GetComponent<Text>().text = Face.ToString();


        transform.Find("MyPlayer/Face").GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/" + (Face+1)) as Sprite;


    }


    public void SkinButton(int direction)
    {
        if (direction == -1)
        {
            Skin -= 1;
            if (Skin == -1)
                Skin = Skin_End;
        }
        else if (direction == 1)
        {
            Skin += 1;
            if (Skin == Skin_End+1)
                Skin = 0;
        }
        transform.Find("Character/Skin/Number").GetComponent<Text>().text = Skin.ToString();
    }


    public void GenderButton(int direction)
    {
        Gender = direction;
        transform.Find("Character/Gender/Number").GetComponent<Text>().text = Gender.ToString();
    }



}
