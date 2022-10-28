using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class Inventory
    {
        //Dictionary<int, Item> _items = new Dictionary<int, Item>();
        // 외부에서 꺼내 쓸수 있도록

        public Dictionary<int, Item> Items { get; } = new Dictionary<int, Item>();

        public int inventoryCapacity = 20;

        // Lock은 안건다 : 이유 = Player에 인벤토리를 넣을거고, 모두다 GameRoom 을 통해 접근할 것이기 때문.

        // 아이템을 넣는다.
        public void Add(Item item)
        {
            int t = 0;

            // 원래 아이템중에 원래것이 있으면 더해진 아이템의 카운트로 교체한다.
            foreach(Item a in Items.Values)
            {
                if (a.ItemDbId == item.ItemDbId)
                {
                    a.Count = item.Count;
                    t = 1;
                }
            }

            if(t==0)
                Items.Add(item.ItemDbId, item);
        }


        // 아이템 DB ID 를 통해 아이템의 정보를 불러온다.
        public Item Get(int itemDbId)
        {
            Item item = null;
            Items.TryGetValue(itemDbId, out item);
            return item;

        }

        // 아이템을 찾는다.
        public Item Find(Func<Item, bool> condition)
        {
            foreach(Item item in Items.Values)
            {
                if (condition.Invoke(item))
                    return item;
            }

            return null;
        }

        // 아이템을 넣는다.
        public void Delete(Item item)
        {

            foreach (Item t in Items.Values)
            {
                Console.WriteLine($"Pastitem : {t.Slot}");
            }

            Items.Remove(item.ItemDbId);

            foreach (Item t in Items.Values)
            {
                if (t.Slot > item.Slot)
                    t.Slot -= 1;
            }


            foreach (Item t in Items.Values)
            {
                Console.WriteLine($"Nowitem : {t.Slot}");
            }

        }

        // 비어있으면 슬롯 지정해주고, 꽉 차있으면 null을 리턴
        public int? GetEmptySlot()
        {   
            for(int slot = 0; slot < inventoryCapacity; slot ++)
            {
                Item item = Items.Values.FirstOrDefault(i => i.Slot == slot);
                if (item == null)
                    return slot;
            }

            return null;
        }

        public List<int> GetEmptySlots(int count)
        {
            List<int> slotCount = new List<int>();

            // 아이템 창에 슬롯 비어있는게 얼마나 되는지 알려준다.

            for (int slot = 0; slot < inventoryCapacity; slot++)
            {
                Item item = Items.Values.FirstOrDefault(i => i.Slot == slot);
                if (item == null)
                    slotCount.Add(slot);
            }


            return slotCount;
        }

        // 비어있으면 슬롯 지정해주고, 꽉 차있으면 null을 리턴
        public Item GetFromSlot(int CheckSlot)
        {

            Item item = Items.Values.FirstOrDefault(i => i.Slot == CheckSlot);
            if (item != null)
                return item;


            return null;
        }



        // 이거 오류 뜸 쓰면 안됨.
        // 무조건 중복이 있을 경우에만 되더라고.... return null 나오는 순간 오류뜸
        // 포션의 이름을 보고 슬로에 있는지 확인해줌
        public int? GetSlotFromTemplateId(int TemplateId)
        {
            for (int slot = 0; slot < inventoryCapacity; slot++)
            {
                Item item = Items.Values.FirstOrDefault(i => i.Slot == slot);
                if (item.TemplateId == TemplateId)
                    return slot;
            }

            return null;
        }

        
        // 아이템 template ID 를 통해 아이템의 정보를 불러온다.
        public Item Get_template(int templateId)
        {
            Item item = null;

            // 밸류값으로 키값 찾기.
            int Key = Items.FirstOrDefault(x => x.Value.TemplateId == templateId).Key;
            Items.TryGetValue(Key, out item);

            return item;

        }

    }
}
