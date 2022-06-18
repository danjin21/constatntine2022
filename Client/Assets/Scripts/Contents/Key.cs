using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Key
    {
        // 실질적으로 이것으로 관리를 하지만,
        public KeySettingInfo Info { get; } = new KeySettingInfo();

        // 여기서도 각각 통해서 갖고온다.

        public int KeyDbId
        {
            get { return Info.KeyDbId; }
            set { Info.KeyDbId = value; }
        }


        public int KeyValue
        {
            get { return Info.Key; }
            set { Info.Key = value; }
        }

        public int Type
        {
            get { return Info.Type; }
            set { Info.Type = value; }
        }

        public int Action
        {
            get { return Info.Action; }
            set { Info.Action = value; }
        }







        // 아이템 생성하는 부분

        public static Key MakeKey(KeySettingInfo keysettingInfo)
        {
            Key key = null;

            key = new Key();


            key.Action = keysettingInfo.Action;
            key.Type = keysettingInfo.Type;
            key.KeyValue = keysettingInfo.Key;
            key.KeyDbId = keysettingInfo.KeyDbId;

            return key;
        }

    }


