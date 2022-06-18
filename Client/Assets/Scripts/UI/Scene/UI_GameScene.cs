using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_KeySetting KeySettingUI { get; private set; }
    public UI_Skill SkillUI { get; private set; }

    public Text Hp;
    public Text Mp;
    public Text Exp;


    enum Buttons
    {
        ChangeButton,
    }


    public override void Init()
    {
        base.Init();

        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        KeySettingUI = GetComponentInChildren<UI_KeySetting>();
        SkillUI = GetComponentInChildren<UI_Skill>();

        Hp = GameObject.Find("Hp/HpText").GetComponent<Text>();
        Mp = GameObject.Find("Mp/MpText").GetComponent<Text>();
        Exp = GameObject.Find("Exp/ExpText").GetComponent<Text>();


        SkillUI.gameObject.SetActive(false);

        Bind<Image>(typeof(Buttons));

        GetImage((int)Buttons.ChangeButton).gameObject.BindEvent(Change);

        //처음에 UI 안보이게 하는거
        //StatUI.gameObject.SetActive(false);
        //InvenUI.gameObject.SetActive(false);


        //UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        //UI_Inventory invenUI = gameSceneUI.InvenUI;

        //invenUI.gameObject.SetActive(true);
        //invenUI.RefreshUI();


    }

    public void OnClickedExit()
    {

        //Managers.Network.DisconnectFromGame();
        //Managers.Scene.LoadScene(Define.Scene.Login);

        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;

        //        Managers.Network.DisconnectFromGame();

        //#else
        //                Application.Quit(); // 어플리케이션 종료
        //#endif


        // 서버한테 로그아웃 하겠다고 하는 곳
        C_Logout logout = new C_Logout();
        Managers.Network.Send(logout);


    }

    public void OnClickUsers()
    {

        // User 창이 이미 켜져있는지 확인한다.

        GameObject UserList = GameObject.Find("UI_UserListPopup");

        if (UserList != null)
            return;


        // 서버한테 접속 유저를 물어본다.

        C_Users users = new C_Users();
        Managers.Network.Send(users);
    }


    public void Change(PointerEventData evt)
    {
        // 스위칭

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        if (invenUI.gameObject.activeSelf)
        {
            invenUI.gameObject.SetActive(false);
        }
        else
        {
            invenUI.gameObject.SetActive(true);
            invenUI.RefreshUI();
        }

        //// 스위칭

        //UI_Stat statUI = gameSceneUI.StatUI;

        //if (statUI.gameObject.activeSelf)
        //{
        //    statUI.gameObject.SetActive(false);
        //}
        //else
        //{
        //    statUI.gameObject.SetActive(true);
        //    statUI.RefreshUI();
        //}

        // 스위칭

        UI_Skill skillUI = gameSceneUI.SkillUI;

        if (skillUI.gameObject.activeSelf)
        {
            skillUI.gameObject.SetActive(false);
            GetImage((int)Buttons.ChangeButton).gameObject.GetComponentInChildren<Text>().text = "↔ Skill";
        }
        else
        {
            skillUI.gameObject.SetActive(true);
            GetImage((int)Buttons.ChangeButton).gameObject.GetComponentInChildren<Text>().text = "↔ Inven";
            skillUI.RefreshUI();
        }

    }

}
