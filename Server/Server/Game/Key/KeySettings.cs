using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Server.Game
{
    public class KeySettings
    {
        // 외부에서 꺼내 쓸수 있도록

        public Dictionary<int, Key> Keys { get; } = new Dictionary<int, Key>();


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

            foreach (Key t in Keys.Values)
            {
                Console.WriteLine($"Pastkey : {t.Action}");
            }

            Keys.Remove(key.KeyDbId);

            foreach (Key t in Keys.Values)
            {
                Console.WriteLine($"Nowkey : {t.Action}");
            }


        }

        
    }

}
