using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UI_KeySetting : UI_Base
{
    public List<UI_KeySetting_Item> Keys { get; } = new List<UI_KeySetting_Item>();

    public override void Init()
    {

        Keys.Clear();

        GameObject grid = transform.Find("KeyGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < 11; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_KeySetting_Item", grid.transform);

            UI_KeySetting_Item item = go.GetOrAddComponent<UI_KeySetting_Item>();


            switch (i+1)
            {
                case 1:
                    go.GetComponent<UI_KeySetting_Item>().Name = "Shift";
                    break;
                case 2:
                    go.GetComponent<UI_KeySetting_Item>().Name = "Ctrl";
                    break;
                case 3:
                    go.GetComponent<UI_KeySetting_Item>().Name = "Alt";
                    break;
                case 4:
                    go.GetComponent<UI_KeySetting_Item>().Name = "Q";
                    break;
                case 5:
                    go.GetComponent<UI_KeySetting_Item>().Name = "W";
                    break;
                case 6:
                    go.GetComponent<UI_KeySetting_Item>().Name = "E";
                    break;
                case 7:
                    go.GetComponent<UI_KeySetting_Item>().Name = "A";
                    break;
                case 8:
                    go.GetComponent<UI_KeySetting_Item>().Name = "S";
                    break;
                case 9:
                    go.GetComponent<UI_KeySetting_Item>().Name = "D";
                    break;
                case 10:
                    go.GetComponent<UI_KeySetting_Item>().Name = "Space";
                    break;
                case 11:
                    go.GetComponent<UI_KeySetting_Item>().Name = "Z";
                    break;
            }
            go.GetComponent<UI_KeySetting_Item>()._name.text = go.GetComponent<UI_KeySetting_Item>().Name;
            go.GetComponent<UI_KeySetting_Item>().KeyCode = i + 1;

            Keys.Add(item);


        }

        RefreshUI();

    }


    public void RefreshUI()
    {

        if (Keys.Count == 0)
            return;


        // 처음에 초기화
        foreach (UI_KeySetting_Item t in Keys)
        {
            t.SetItem(null);
        }


        List<Key> keys = Managers.KeySetting.Keys.Values.ToList();


        foreach (Key key in keys)
        {
            Keys[key.KeyValue-1].SetItem(key);
        }

    }

}
