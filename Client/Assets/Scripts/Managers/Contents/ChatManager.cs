using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerCore;
using System;
using System.Net;
using Google.Protobuf;
using TMPro;
using UnityEngine.EventSystems;

using System.Text.RegularExpressions;



public class ChatManager
{


    public Text[] ChatText;
    public TMP_InputField ChatInput;

    public void Init()
    {
        ChatInput = GameObject.Find("UI_GameScene/Bottom/InputField").GetComponent<TMP_InputField>();

        GameObject Content = GameObject.Find("UI_GameScene/Bottom/Scroll View/Viewport/Content").gameObject;

        ChatText = Content.GetComponentsInChildren<Text>();

        for (int i = 0; i < ChatText.Length / 2; i++)
        {
            Text A = ChatText[i];
            ChatText[i] = ChatText[ChatText.Length - 1 - i];
            ChatText[ChatText.Length - 1 - i] = A;
        }

    }



    public void Update()
    {

        if (ChatText == null || ChatInput == null)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (ChatInput.text == "" && ChatInput.isFocused)
            {
                //ChatInput.ActivateInputField();
                ChatInput.DeactivateInputField();
                ChatInput.interactable = false;

                Input.imeCompositionMode = IMECompositionMode.On;

                Debug.Log("@3");
                return;
            }
        }




        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChatInput.text = "";
            ChatInput.DeactivateInputField();
            ChatInput.interactable = false;

            Input.imeCompositionMode = IMECompositionMode.On;
        }


        if (ChatInput.interactable == true)
            CaptureText();





    }


    // 인풋 값 실시간 복사하는 부분 ( TMP -> 일반  TEXT로 표기하기 위함 )

    public void CaptureText()
    {
        string A = Regex.Replace(ChatInput.transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text, @"<[u|U|/](.|\n)*?>", string.Empty);
              
        ChatInput.transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<Text>().text = A;
    }
    



       



    #region 채팅
    public void Send()
    {

        // Sound
        //Managers.Sound.Play("UI/Button/Slick Button", Define.Sound.Effect);

        //if (ChatInput.text == "")
        //{
        //    //ChatInput.Select();
        //    //ChatInput.Select();

        //    //if (ChatInput.isFocused == false)
        //    //    ChatInput.ActivateInputField();
        //    //else
        //    //    ChatInput.DeactivateInputField();
        //    if (ChatInput.interactable == true)
        //        ChatInput.interactable = false;
        //    else if (ChatInput.interactable == false)
        //    {
        //        ChatInput.interactable = true;
        //        ChatInput.ActivateInputField();
        //    }

        //    Debug.Log(ChatInput.isFocused);



        //    return;
        //}

        if (ChatInput.interactable == false)
        {
            Debug.Log("Input.imeCompositionMode 1 : " + Input.imeCompositionMode + "." + Input.imeIsSelected);

            ChatInput.interactable = true;
            ChatInput.ActivateInputField();

            return;
        }
        else
        {



            if (ChatInput.text == "")
            {
                Input.imeCompositionMode = IMECompositionMode.On;
                ChatInput.interactable = false;
                return;
            }
        }

        // 서버에서 응답을 받았을 때 실행이 되는 부분이거든요.
        //ChatRPC(Managers.Object.MyPlayer.name + " : " + ChatInput.transform.GetChild(2).gameObject.GetComponent<Text>().text);

        // 서버 전송
        C_Chat chatPacket = new C_Chat();
        chatPacket.Message = ChatInput.transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text;
        Managers.Network.Send(chatPacket);


        ChatInput.text = "";
        ChatInput.transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "";

        Input.imeCompositionMode = IMECompositionMode.On;
        ChatInput.interactable = false;
        //ChatInput.ActivateInputField();


        //ChatInput.ActivateInputField();
        //ChatInput.Select();
        Debug.Log("@@2");
        Debug.Log("Input.imeCompositionMode 4 : " + Input.imeCompositionMode + "." + Input.imeIsSelected);

        CaptureText();

    }


    public void ChatRPC(string msg)
    {

        if (ChatText[0].text == "")
        {
            ChatText[0].text = msg;
        }
        else if (ChatText[0].text != "")
        {
            for (int i = ChatText.Length - 1; i > 0; i--)
            {
                ChatText[i].text = ChatText[i - 1].text;
            }
            ChatText[0].text = msg;
        }
    }
    #endregion





}
