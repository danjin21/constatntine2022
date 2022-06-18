using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer   // 하나의 스레드에서 일감을 실행한다.
    {


        public void HandleEquipItem(Player player, C_EquipItem equipPacket)
        {
            if (player == null)
                return;

            // Player -> HandleEquipItem 으로 옮김
            player.HandleEquipItem(equipPacket);
        }

        public void HandleUseItem(Player player, C_UseItem usePacket)
        {
            // player가 없으면 리턴
            if (player == null)
                return;


            if (player.Room == null)
                return;

            if (player.ConsumeCool == true)
                return;

            GameRoom room = player.Room;

            // 쿨타임 주기.
            player.ConsumeCool = true;
            room.PushAfter(500, player.ConsumeCooltime);



            // Player -> HandleEquipItem 으로 옮김
            player.HandleUseItem(usePacket);
        }


        public void HandleDropItem(Player player, C_DropItem dropItemPacket)
        {
            // player가 없으면 리턴
            if (player == null )
                return;

            player.HandleDropItem(dropItemPacket);
        }

        public void HandleSlotChange(Player player, C_SlotChange slotChangePacket)
        {
            // player가 없으면 리턴
            if (player == null)
                return;

            // 서버에서 슬롯을 변경한다.
            
            // 변경하려는 아이템
            Item A = player.Inven.Get(slotChangePacket.ItemDbId);

            //-> 인벤에 바꿀 아이템 있는지 확인
            if (A == null)
                return;

            // 해당 슬롯의 아이템
            Item B = player.Inven.GetFromSlot(slotChangePacket.Slot);

            //-> 해당 슬롯에 아이템 있는지 확인
            if (B == null)
                return;

            //-> 각자 아이템 슬롯 변경
            int templateSlot = A.Slot;

            A.Slot = B.Slot;
            B.Slot = templateSlot;


            // DB 상관없이 바로 클라에 리스트 패킷을 보내준다.

            // 클라한테 패킷 보내기
            S_ItemList itemListPacket = new S_ItemList();

            foreach (Item t in player.Inven.Items.Values)
            {
                // 아이템 디비정보를 이용해서 아이템을 만들어준다.
                Item item = t;

                // 아무 문제가 없으면
                if (item != null)
                {

                    // 패킷에 해당 아이템 정보를 넣어준다.
                    ItemInfo info = new ItemInfo();

                    info.MergeFrom(item.Info);

                    itemListPacket.Items.Add(info);
                }
            }

            player.Session.Send(itemListPacket);


            // DB에서 슬롯을 변경한다.
            // -> 해당 ID 2개의 아이템 2개를 불러와서 슬롯을 변경해준다.

            DbTransaction.SlotChange(player, this, A, B);




        }


    }




}
