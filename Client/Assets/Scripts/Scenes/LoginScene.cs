using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    UI_LoginScene _sceneUI;

    protected override void Init()
    {

        base.Init();

        // 나중에 Lobby 만들때는 Scene 을 Lobby로하면됨. ( Define.cs 에 있음 )
        SceneType = Define.Scene.Login;


        //Managers.Web.BaseUrl = "https://49.142.31.135:5003/api";
        //Managers.Web.BaseUrl = "https://49.142.31.135:5001/api";
        //Managers.Web.BaseUrl = "https://127.0.0.1:5001/api";

        //Managers.Web.BaseUrl = "https://1.222.183.124:5001/api";
        Managers.Web.BaseUrl = "https://203.170.123.127:5001/api";



        //WebPacket.SendCreateAccount("Rookiss", "1234");

        //Screen.SetResolution(1920, 1080, false);
        //Screen.SetResolution(1280, 960, false);

        Screen.SetResolution(1280, 960, false);




        //Screen.SetResolution(1920, 1440, false);
        //Screen.SetResolution(1920, 1080, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LoginScene>();

        //Application.targetFrameRate = 150;
        Application.targetFrameRate = 500;

    }

    public override void Clear()
    {
        
    }

  
}
