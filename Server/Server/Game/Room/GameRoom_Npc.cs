using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer   // 하나의 스레드에서 일감을 실행한다.
    {



        public void HandleNpc(Player player, C_Npc npcPacket)
        {
            if (player == null)
                return;

            // 타겟
            GameObject targetNpc = null;

            //// 존에 있는지 확인
            //Zone NowZone = GetZone(player.CellPos);

            //// 같은 오브젝트가 같은 존에 있는지 확인
            //targetNpc = NowZone.FindOneNpc(i => i.Id == npcPacket.ObjectId);


            // 마우스로 클릭하다보니, 상하좌우 zone 까지 다 봐야한다.

            targetNpc = adjacentZoneObject(player, npcPacket.ObjectId);


            // 현재 존에 npc가 없다면 무효
            if (targetNpc == null)
                return;

            // Type이 npc가 아니라면 무효
            if (targetNpc.ObjectType != GameObjectType.Npc)
                return;

 


            // npc가 있다면 그 npc의 정보를 불러온다.

            Npc npc = (Npc)targetNpc;

            // TemplateId에서 해당 Npc의 데이터를 가지고 온다.
            NpcData npcData = null;
            DataManager.NpcDict.TryGetValue(npc.TemplateId, out npcData);

            // npc 이름을 넣어준다.
            npc.Info.Name = npcData.name;

            // 상인일 경우
            if (npcData.merchant == 1)
            {
                // 상인 정보를 준다.
                // 아이템 templateId List를 준다.
                // 아이템 slot을 준다.

                // 상점아이템 리스트 패킷을 생성한다.

                S_Shop shopPacket = new S_Shop();
                shopPacket.NpcInfo = npc.Info;

                ItemData itemData = null;
       

                foreach (ProductData productData in npcData.products)
                {
                    ItemInfo info = new ItemInfo();
                    info.TemplateId = productData.templateId;
                    //info.Price = productData.cost;

                    DataManager.ItemDict.TryGetValue(productData.templateId, out itemData);
                    info.Price = itemData.price;

                    shopPacket.Items.Add(info);
                }

                player.Session.Send(shopPacket);

            }
            // 상인이 아닐 경우
            else if (npcData.merchant == 0)
            {
                // npc 정보를 준다.

                S_Npc dialoguePacket = new S_Npc();
                dialoguePacket.NpcInfo = npc.Info;

                // 기본대사 (하드코딩)
                dialoguePacket.Dialogue = /*npcData.npcId*1000 +*/011;

   

                // 그 NPC가 가지고 있는 퀘스트 정보들을 모두 넘겨준다.
                foreach(QuestData p in npc.NpcQuestList)
                {
                    QuestInfo questInfo = new QuestInfo
                    {                        
                        NpcId = npc.TemplateId,
                        QuestTemplateId = p.questId,
                        Status = p.status,
                    };

                    // 여기서 요구사항으로 거르거나, DB에 갖고 있는지 등을 확인한다..
                    dialoguePacket.Quests.Add(questInfo);
                }

                player.Session.Send(dialoguePacket);
            }


        }






        public void HandlePurchase(Player player, C_Purchase purchasePacket)
        {
            if (player == null)
                return;

            Console.WriteLine("구매요청 : " + purchasePacket.TemplateId + "/" + purchasePacket.Count + "개");

            // 진짜 상인이랑 같은 존에 있는지 확인

            GameObject targetNpc = null;
            //Zone NowZone = GetZone(player.CellPos);
            //targetNpc = NowZone.FindOneNpc(i => i.Id == purchasePacket.NpcId);

            targetNpc = adjacentZoneObject(player, purchasePacket.NpcId);

            //List<Zone> adjacentZones = GetAdjacentZones(player.CellPos);

            //foreach (Zone t in adjacentZones)
            //{

            //    // 이미 존재한다면 넘긴다.
            //    if (targetNpc != null)
            //        continue;

            //    // 같은 오브젝트가 같은 존에 있는지 확인
            //    targetNpc = t.FindOneNpc(i => i.Id == purchasePacket.NpcId);
            //}


            if (targetNpc == null)
                return;

            if (targetNpc.ObjectType != GameObjectType.Npc)
                return;

            Npc npc = (Npc)targetNpc;

            NpcData npcData = null;
            DataManager.NpcDict.TryGetValue(npc.TemplateId, out npcData);

            // 상인이 맞는지 확인
            if (npcData.merchant != 1)
                return;

            // 갯수가 음수이거나 숫자가 아닌지 확인 Count 자체가 Int라서 모르겠네..  

            if (purchasePacket.Count < 1)
                return;


            // 아이템 정보
            ItemData itemData = null;

            // 상인이 파는 물품이 맞는지 확인

            bool ItemExist = false;

            ItemInfo info = new ItemInfo();

            foreach (ProductData productData in npcData.products)
            {
                if (purchasePacket.TemplateId != productData.templateId)
                    continue;

                info.TemplateId = productData.templateId;
                //info.Price = productData.cost;
                DataManager.ItemDict.TryGetValue(productData.templateId, out itemData);
                info.Price = itemData.price;


                // 아이템 갯수 정해주기
                if (itemData.itemType == ItemType.Consumable || itemData.itemType == ItemType.Etc)
                    info.Count = purchasePacket.Count;
                else
                    info.Count = 1;

                // 아이템 존재한다.
                ItemExist = true;
            }

            if (ItemExist == false)
                return;


            // 돈 없으면 return;
            if (player.Stat.Gold < ( info.Price * info.Count ))
                return;


            // 아이템 풀창인지 확인 or 대체되는 && 아이템 넣어주기.

            RewardData rewardData = new RewardData();
            rewardData.itemId = info.TemplateId;
            rewardData.count = info.Count;

            {
                rewardData.itemInfo = new ItemInfo();
                rewardData.itemInfo.UpgradeSlot = 7; // 처음에는 어쩔 수 없음
                rewardData.itemInfo.Str = itemData.Str;
                rewardData.itemInfo.Dex = itemData.Dex;
                rewardData.itemInfo.Int = itemData.Int;
                rewardData.itemInfo.Luk = itemData.Luk;
                rewardData.itemInfo.Hp = itemData.Hp;
                rewardData.itemInfo.Mp = itemData.Mp;
                rewardData.itemInfo.WAtk = itemData.WAtk;
                rewardData.itemInfo.MAtk = itemData.MAtk;
                rewardData.itemInfo.WDef = itemData.WDef;
                rewardData.itemInfo.MDef = itemData.MDef;
                rewardData.itemInfo.Speed = itemData.Speed;
                rewardData.itemInfo.AtkSpeed = itemData.AtkSpeed;
                rewardData.itemInfo.Durability = itemData.Durability;
                rewardData.itemInfo.Enhance = itemData.Enhance;
                rewardData.itemInfo.WPnt = itemData.WPnt;
                rewardData.itemInfo.MPnt = itemData.MPnt;
            }



            if (DbTransaction.RewardPlayer(player, rewardData, this))
            {
                RewardData costData = new RewardData();
                costData.itemId = 99999;
                costData.count = - (info.Price * info.Count);

                DbTransaction.RewardPlayer(player, costData, this);

            }
            else // 풀창이거나 슬롯이없거나 그러면 return;
                return;

        }


        public void HandleSell(Player player, C_Sell sellPacket)
        {
            if (player == null)
                return;

            Console.WriteLine("판매요청 : " + sellPacket.ItemDbId + "/" + sellPacket.Count + "개");


            // 진짜 상인이랑 같은 존에 있는지 확인

            GameObject targetNpc = null;
            //Zone NowZone = GetZone(player.CellPos);
            //targetNpc = NowZone.FindOneNpc(i => i.Id == sellPacket.NpcId);

            targetNpc = adjacentZoneObject(player, sellPacket.NpcId);


            if (targetNpc == null)
                return;

            if (targetNpc.ObjectType != GameObjectType.Npc)
                return;

            // 갯수가 음수이거나 숫자가 아닌지 확인 Count 자체가 Int라서 모르겠네..  

            if (sellPacket.Count < 1)
                return;

            Npc npc = (Npc)targetNpc;

            NpcData npcData = null;
            DataManager.NpcDict.TryGetValue(npc.TemplateId, out npcData);

            // 상인이 맞는지 확인
            if (npcData.merchant != 1)
                return;

            // 내가 갖고 물품이 맞는지 확인 (장비하고 있는 상태라면 리턴)
            // 이제 item은 진짜 검증된 아이템 정보값이다.

            Item item = player.Inven.Get(sellPacket.ItemDbId);

            if (item == null)
                return;

            if (item.Equipped == true)
                return;

            // 서버 메모리에서 아이템 지우기 ( count = 0 이면 지우기 )

            int originalItemCount = item.Count;
            Console.WriteLine("(전)포션개수 : " + item.Count + "개");

            // 포션인 경우 갯수를 줄여주고, 0일때 서버 메모리에서 지워준다.
            if (item.ItemType == ItemType.Consumable || item.ItemType == ItemType.Etc)
            {
                item.Count -= sellPacket.Count;

                if (item.Count <= 0)
                {
                    item.Count = 0;
                    player.Inven.Delete(item);
                }
            }
            // 포션류가 아닌 경우 무조건 서버 메모리에서 지워주고 Drop을 해준다.
            else
            {
                item.Count = 0;
                player.Inven.Delete(item);
            }

            int newItemCount = item.Count;

            // DB에서 카운트 깎아주기.
            DbTransaction.SellItem(player, item, this);


            Console.WriteLine("(후)포션개수 : " + item.Count + "개");


            // 아이템 가격 책정

            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(item.TemplateId, out itemData);
            int itemPrice = itemData.sellPrice;

            // 완료되면 돈 지급 

            RewardData rewardData = new RewardData();
            rewardData.itemId = 99999;
            rewardData.count = (originalItemCount-newItemCount) * itemPrice; // 원래 진짜 갯수에서 계산 후 갯수를 뺸것
            rewardData.probability = -1;




            DbTransaction.RewardPlayer(player, rewardData, this);

        }
    }



}
