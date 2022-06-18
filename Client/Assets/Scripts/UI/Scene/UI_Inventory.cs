using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : UI_Base
{
    public List<UI_Inventory_Item> Items { get; } = new List<UI_Inventory_Item>();


    public override void Init()
    {
        Items.Clear();

        GameObject grid = transform.Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        for(int i=0; i< 20; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Item", grid.transform);
            UI_Inventory_Item item = go.GetOrAddComponent<UI_Inventory_Item>();
            Items.Add(item);


        }

        RefreshUI();
    }

    public void RefreshUI()
    {

        if (Items.Count == 0)
            return;


        // 처음에 초기화
        foreach(UI_Inventory_Item t in Items)
        {
            t.SetItem(null);
        }
      

        List<Item> items = Managers.Inven.Items.Values.ToList();

        // 슬롯에 따라 정렬
        items.Sort((left, right) => { return left.Slot - right.Slot; });

        foreach(Item item in items)
        {
            if (item.Slot < 0 || item.Slot >= 20)
                continue; // 예외처리
            Items[item.Slot].SetItem(item);

        }

    }
}
