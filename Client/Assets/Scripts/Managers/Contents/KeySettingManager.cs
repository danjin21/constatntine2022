using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class KeySettingManager : MonoBehaviour
{
    // 외부에서 꺼내 쓸수 있도록

    public Dictionary<int, Key> Keys { get; } = new Dictionary<int, Key>();


    // 단축키 할거다! 하는 부분

    public int ChangeKey = -1;

    // 아이템인 경우 버리는 것 때문에 DbId도 넣는다.

    public int ChangeItemDbId = -1;


    // 0 : 아이템 인벤 1: 스킬 인벤 2: 단축키 인벤
    public int CurrentCursor = -1;


    // Lock은 안건다 : 이유 = Player에 키셋팅을 넣을거고, 모두다 GameRoom 을 통해 접근할 것이기 때문.

    // 아이템을 넣는다.
    public void Add(Key key)
    {
        int t = 0;

        // 원래 키값중에 원래것이 있으면 더해진 키의 카운트로 교체한다.
        foreach (Key a in Keys.Values)
        {
            if (a.KeyValue == key.KeyValue)
            {
                a.Action = key.Action;
                a.Type = key.Type;

                t = 1;
            }
        }

        if (t == 0)
            Keys.Add(key.KeyDbId, key);
    }


    // 아이템 DB ID 를 통해 아이템의 정보를 불러온다.
    public Key Get(int keyDbId)
    {
        Key key = null;
        Keys.TryGetValue(keyDbId, out key);
        return key;

    }


    // 아이템 DB ID 를 통해 아이템의 정보를 불러온다.
    public Key Get_KeyValue(int keyValue)
    {
        Key key = null;
        //Keys.TryGetValue(keyValue, out key);


        // 밸류값으로 키값 찾기.
        int KeyCode = Keys.FirstOrDefault(x => x.Value.KeyValue == keyValue).Key;

        Keys.TryGetValue(KeyCode, out key);

        return key;


    }

    // 아이템을 찾는다.
    public Key Find(Func<Key, bool> condition)
    {
        foreach (Key key in Keys.Values)
        {
            if (condition.Invoke(key))
                return key;
        }

        return null;
    }

    // 아이템을 넣는다.
    public void Delete(Key key)
    {


        Keys.Remove(key.KeyDbId);


    }


    public void Clear()
    {
        Keys.Clear();
    }





    public void SetKeySetting(GameObject Inventory,int Action)
    {
        // KeySetting 이 활성화되어있을떄 단축키 눌러서 설정하는 부분
        if (Inventory.transform.GetChild(4).transform.gameObject.activeSelf)
        {
            int Key = -1;

            if ((Input.GetKeyDown(KeyCode.LeftShift)) || (Input.GetKeyDown(KeyCode.RightShift)))
                Key = 1;
            else if ((Input.GetKeyDown(KeyCode.LeftControl)) || (Input.GetKeyDown(KeyCode.RightControl)))
                Key = 2;
            else if ((Input.GetKeyDown(KeyCode.LeftAlt)) || (Input.GetKeyDown(KeyCode.RightAlt)))
                Key = 3;
            else if ((Input.GetKeyDown(KeyCode.Q)))
                Key = 4;
            else if ((Input.GetKeyDown(KeyCode.W)))
                Key = 5;
            else if ((Input.GetKeyDown(KeyCode.E)))
                Key = 6;
            else if ((Input.GetKeyDown(KeyCode.A)))
                Key = 7;
            else if ((Input.GetKeyDown(KeyCode.S)))
                Key = 8;
            else if ((Input.GetKeyDown(KeyCode.D)))
                Key = 9;
            else if ((Input.GetKeyDown(KeyCode.Space)))
                Key = 10;
            else if ((Input.GetKeyDown(KeyCode.Z)))
                Key = 11;

            if (Key != -1)
            {
                // 서버 전송
                C_KeySetting keysettingPacket = new C_KeySetting();
                keysettingPacket.Key = Key;
                keysettingPacket.Action = Action;

                Managers.Network.Send(keysettingPacket);
                Inventory.transform.GetChild(4).transform.gameObject.SetActive(false);
            }



        }
    }





}