using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    // 서버 복붙

    public Dictionary<int, Item> Items { get; } = new Dictionary<int, Item>();


    // Lock은 안건다 : 이유 = Player에 인벤토리를 넣을거고, 모두다 GameRoom 을 통해 접근할 것이기 때문.

    // 아이템을 넣는다.
    public void Add(Item item)
    {
        //Items.Add(item.ItemDbId, item);


        int t = 0;

        // 원래 아이템중에 원래것이 있으면 더해진 아이템의 카운트로 교체한다.
        foreach (Item a in Items.Values)
        {
            if (a.ItemDbId == item.ItemDbId)
            {
                a.Count = item.Count;
                t = 1;
            }
        }

        if (t == 0)
            Items.Add(item.ItemDbId, item);



    }

    // 아이템을 삭제한다.

    public void Delete(int ItemDbId)
    {

        Item A = Managers.Inven.Get(ItemDbId);

        Items.Remove(ItemDbId);


        // 클라 자체 내에서도 슬롯을 한칸씩 댕겨준다.
        foreach (Item t in Items.Values)
        {
            if (t.Slot > A.Slot)
                t.Slot -= 1;
        }

    }

    // 아이템 DB ID 를 통해 아이템의 정보를 불러온다.
    public Item Get(int itemDbId)
    {
        Item item = null;
        Items.TryGetValue(itemDbId, out item);
        return item;

    }

    // 아이템 DB ID 를 통해 아이템의 정보를 불러온다.
    public Item Get_template(int templateId)
    {
        Item item = null;

        // 밸류값으로 키값 찾기.
        int Key = Items.FirstOrDefault(x => x.Value.TemplateId == templateId).Key;
        Items.TryGetValue(Key, out item);

        return item;

    }


    // 아이템을 찾는다.
    public Item Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
                return item;
        }

        return null;
    }


    public void Clear()
    {
        Items.Clear();
    }

}

