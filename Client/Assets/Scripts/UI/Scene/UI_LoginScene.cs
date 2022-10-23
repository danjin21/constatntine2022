using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{

    public InputField A;
    public InputField B;

    enum GameObjects
    {
        AccountName,
        Password,
        Info
    }

    enum Buttons
    {
        CreateBtn,
        LoginBtn
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Buttons));

        GetImage((int)Buttons.CreateBtn).gameObject.BindEvent(OnClickCreateButton);
        GetImage((int)Buttons.LoginBtn).gameObject.BindEvent(OnClickLoginButton);

        A = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>();
        B = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>();

    }


    private void Update()
    {
        if(A.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
              B.ActivateInputField();
            }
        }
        else if (!A.isFocused && !B.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                A.ActivateInputField();
            }
        }

        if (B.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                A.ActivateInputField();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (A.text != "" && B.text != "")
            {
                PointerEventData a = null;
                OnClickLoginButton(a);
            }
            else
            {
                A.DeactivateInputField();
                B.DeactivateInputField();
            }
        }



        //if (Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().isFocused)
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().ActivateInputField();
        //    }
        //}


        //if (Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().isFocused)
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().ActivateInputField();
        //    }
        //}




    }


    public void OnClickCreateButton(PointerEventData evt)
    {
        // Sound
        //Managers.Sound.Play("UI/Button/Slick Button", Define.Sound.Effect);

        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            // 응답이오면 처리하는 부분
            Debug.Log("result = " + res.CreateOk);
            Debug.Log("Info = " + res.Info);

            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

            Get<GameObject>((int)GameObjects.Info).GetComponent<Text>().text = res.Info;
        });

    }

    public void OnClickLoginButton(PointerEventData evt)
    {

        // Sound
        //Managers.Sound.Play("UI/Button/Slick Button", Define.Sound.Effect);

        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;


        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
        {

            Debug.Log("result = " + res.LoginOk);
            Debug.Log("Info = " + res.Info);

            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Info).GetComponent<Text>().text = res.Info;

            // res = LoginAccountPacketRes
            if (res.LoginOk)
            {
                // 계정 아이디와 토큰을 저장해준다.
                Managers.Network.AccountId = res.AccountId;
                Managers.Network.Token = res.Token;
                Managers.Network.AccountName = res.AccountName;
                Managers.Network.Password = password;

                // 팝업을 띄어준다.
                UI_SelectServerPopup popup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
                popup.SetServers(res.ServerList);

                // 응답이오면 처리하는 부분
                //Managers.Network.ConnectToGame();
                //Managers.Scene.LoadScene(Define.Scene.Game);
            }

            
        });
    }


}
