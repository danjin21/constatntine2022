using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
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

        public static Key MakeKey(KeySettingDb keySettingDb)
        {
            Key key = null;

            key = new Key();

            key.Action = keySettingDb.action;
            key.Type = keySettingDb.type;
            key.KeyValue = keySettingDb.key;
            key.KeyDbId = keySettingDb.KeySettingDbId;

            return key;
        }

    }



}
