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
    public partial class GameRoom : JobSerializer // 하나의 스레드에서 일감을 실행한다.
    {

        public void HandleQuest(Player player_1, C_Npc npcPacket)
        {


            // 멀티스레드에서 null로 될수 있기에 항상 로컬저장
            Player player = player_1;
            if (player == null)
                return;

            // 멀티스레드에서 null로 될수 있기에 항상 로컬저장
            GameRoom room = player.Room;
            if (room == null)
                return;


            // 유효성 검사

            #region Check

            if (player == null)
                return;

            // 타겟
            GameObject targetNpc = null;

            //// 존에 있는지 확인
            //Zone NowZone = GetZone(player.CellPos);

            //// 같은 오브젝트가 같은 존에 있는지 확인
            //targetNpc = NowZone.FindOneNpc(i => i.Id == npcPacket.ObjectId);

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

            #endregion


            // 퀘스트 갖고 있는건지 확인
            int QuestId = npcPacket.Quest;

            // 퀘스트 데이터 인덱스 가져오기
            int Order = npcPacket.Order;


            // Get Lose, Check, Req Item 확인

            QuestData questData = null;
            DataManager.QuestDict.TryGetValue(QuestId, out questData);

            if (questData == null)
                return;


            bool IsNormal = true;
            bool IsAlreadyPacket = false;

            // 아이템을 잃는 경우
            if (questData.dialogue[Order].loseItem != null && questData.dialogue[Order].loseItem.Count != 0)
            {
                IsNormal = false;

                S_Npc snpcPacket = new S_Npc();
                snpcPacket.NpcInfo = npc.Info;
                snpcPacket.QuestId = questData.questId;
                

                // 아이템을 갖고있는지 없는지 확인한다.
                bool checkItem = true;

                List<Item> loseItems = new List<Item>();

                foreach(QuestItemData questItemData in questData.dialogue[Order].loseItem)
                {
                    // 해당 아이템의 정보가 서버 메모리 Inven에 존재하는지 확인하기.

                    Item item = null;

                    item = player.Inven.Get_template(questItemData.itemId);

                    // 혹시라도 없으면 foreach 에서 나와서 checkItem 을 false로 만든다. break;

                    if (item == null)
                    {
                        checkItem = false;
                        Console.WriteLine("테스트1");
                        break;
                    }
                    else
                    {
                        // 혹시라도 장비템을 끼고 있었다면, break;
                        if (item.Equipped == true)
                            break;

                        // 만약 갖고 있는 아이템의 갯수가 적다면, break;
                        if(item.Count < questItemData.quantity)
                        {
                            checkItem = false;
                            break;
                        }


                        // item의 버릴 개수를 item의 Count 에 넣어둔다.
                        // 주의점 : 이때 new item으로 만들어서 기본 변수와 견결을 끊는다.
                        // 아래에서 사용되는게 DbId / TemplateId / quantity 3가지므로 값만 넣어준다.
                        // 깊은복사 ( 복사는 하되, 연결을 끊는 것 )

                        Item loseItem_New = new Item(item.ItemType);
                        loseItem_New.ItemDbId = item.ItemDbId;
                        loseItem_New.TemplateId = item.TemplateId;
                        loseItem_New.Count = questItemData.quantity;

                        loseItems.Add(loseItem_New); // 잃어버릴 아이템들의 리스트를 넣는다.

                        //Item loseItem_New = item;

                        //loseItem_New.Count = questItemData.quantity;
                        //loseItems.Add(loseItem_New); // 잃어버릴 아이템들의 리스트를 넣는다.

                       
                    }
                }

                // 아이템이 없을때
                if (checkItem == false)
                {
                    snpcPacket.Dialogue = questData.dialogue[Order + 1].index;

                    player.Session.Send(snpcPacket);
                    return;
                }
                else if(checkItem == true)
                {
                    // 아이템이 있을때
                    snpcPacket.Dialogue = questData.dialogue[Order].index;


                    // 아이템을 잃기 전에, 아이템 창에 슬롯이 있는지 본다.
                    if (questData.dialogue[Order].getItem != null && questData.dialogue[Order].getItem.Count != 0)
                    {

                        int GetItemCount = questData.dialogue[Order].getItem.Count;

                        int? emptySlot = player.Inven.GetEmptySlot();

                        // 몇칸이상 슬롯이 비어있는지 확인하는 기능을 넣어야 한다.
                        List<int> emptySlots = new List<int>();
                        emptySlots = player.Inven.GetEmptySlots(GetItemCount);


                        // 아이템 슬롯이 없을때 || 아이템 갯수보다 슬롯이 적을때
                        if (emptySlot == null || emptySlots.Count < GetItemCount)
                        {


                            snpcPacket.Dialogue = questData.dialogue[Order + 2].index;

                            player.Session.Send(snpcPacket);
                            return;
                        }
                    }

                    // 아이템을 없앤다
                    // 검증된 아이템이다.
                    foreach (Item t in loseItems)
                    {
                        Console.WriteLine("삭제요청 : " + t.ItemDbId + "/" + t.Count + "개");

                        // 원래 아이템
                        Item originItem = null;
                        originItem = player.Inven.Get_template(t.TemplateId);

                        int originalItemCount = originItem.Count;

                        //포션이거나, 기타 아이템일 경우 갯수를 줄여주고, 0일때 서버 메모리에서 지워준다.
                        if (t.ItemType == ItemType.Consumable || t.ItemType == ItemType.Etc)
                        {
                            originItem.Count -= t.Count;

                            if (originItem.Count <= 0)
                            {
                                originItem.Count = 0;
                                player.Inven.Delete(originItem);
                            }
                        }
                        // 포션류가 아닌 경우 무조건 서버 메모리에서 지워주고 Drop을 해준다.
                        else
                        {
                            originItem.Count = 0;
                            player.Inven.Delete(originItem);
                        }

                        DbTransaction.SellItem(player, originItem, this);

                    }

                    // 동시에 많이


                }

                player.Session.Send(snpcPacket);
                
                // 아래 아이템 얻는 부분에서 패킷 또 보내는것 방지하기 위해 표시
                IsAlreadyPacket = true;

                Console.WriteLine("퀘스트 loseItem 변경 요청");
            }

            //if (questData.dialogue[Order].checkItem != null && questData.dialogue[Order].checkItem.Count != 0)
            //    Console.WriteLine("퀘스트 checkItem 변경 요청");

            //if (questData.dialogue[Order].reqItem != null && questData.dialogue[Order].reqItem.Count != 0)
            //    Console.WriteLine("퀘스트 reqItem 변경 요청");

            //// slot Chage가 있는 부분을 찾는다.
            //foreach( DialogueData p in questData.dialogue )
            //{
            //    if(p.statusChange != -999)

            //}


            // 아이템을 주는 경우
            if (questData.dialogue[Order].getItem != null && questData.dialogue[Order].getItem.Count != 0)
            {
                IsNormal = false;

                S_Npc snpcPacket = new S_Npc();
                snpcPacket.NpcInfo = npc.Info;
                snpcPacket.QuestId = questData.questId;


                int GetItemCount = questData.dialogue[Order].getItem.Count;

                int? emptySlot = player.Inven.GetEmptySlot();

                // 몇칸이상 슬롯이 비어있는지 확인하는 기능을 넣어야 한다.
                List<int> emptySlots = new List<int>();
                emptySlots = player.Inven.GetEmptySlots(GetItemCount);


                // 아이템 슬롯이 없을때 || 아이템 갯수보다 슬롯이 적을때
                if (emptySlot == null || emptySlots.Count < GetItemCount)
                {
                    snpcPacket.Dialogue = questData.dialogue[Order + 1].index;

                    player.Session.Send(snpcPacket);
                    return;
                }
                else
                {


                    // 아이템 슬롯이 있을때
                    snpcPacket.Dialogue = questData.dialogue[Order].index;


                    // 아이템 준다.
                    int i = 0;

                    foreach (QuestItemData questItemData in questData.dialogue[Order].getItem)
                    {




                        RewardData newData = new RewardData();
                        newData.itemId = questItemData.itemId;
                        newData.count = questItemData.quantity;
                        // 원래 장비템 같은 경우에는 여기에 능력치 넣어줘야함
                        newData.itemInfo = new ItemInfo();



                        // 장비인경우 가치를 넣어준다.
                        ItemData itemData = null;
                        DataManager.ItemDict.TryGetValue(newData.itemId, out itemData);

                        if (itemData == null)
                            continue;

                        if (itemData.itemType == ItemType.Weapon || itemData.itemType == ItemType.Armor)
                        {

                            newData.itemInfo.UpgradeSlot = 7; // 처음에는 어쩔 수 없음
                            newData.itemInfo.Str = itemData.Str;
                            newData.itemInfo.Dex = itemData.Dex;
                            newData.itemInfo.Int = itemData.Int;
                            newData.itemInfo.Luk = itemData.Luk;
                            newData.itemInfo.Hp = itemData.Hp;
                            newData.itemInfo.Mp = itemData.Mp;
                            newData.itemInfo.WAtk = itemData.WAtk;
                            newData.itemInfo.MAtk = itemData.MAtk;
                            newData.itemInfo.WDef = itemData.WDef;
                            newData.itemInfo.MDef = itemData.MDef;
                            newData.itemInfo.Speed = itemData.Speed;
                            newData.itemInfo.AtkSpeed = itemData.AtkSpeed;
                            newData.itemInfo.Durability = itemData.Durability;
                            newData.itemInfo.Enhance = itemData.Enhance;
                            newData.itemInfo.WPnt = itemData.WPnt;
                            newData.itemInfo.MPnt = itemData.MPnt;

                          
                        }
                       


                        // 이미 갖고 있는 아이템일때는 카운트만 증가

                        int slot = emptySlots[i];


                        // 장비일때는 emptSlots 대로 준다.
                        if (itemData.itemType == ItemType.Weapon || itemData.itemType == ItemType.Armor)
                        {
                            // 여러개 동시에 줄때에는 이것 이용해야함
                            DbTransaction.RewardPlayer(player, newData, room, slot);
                            i += 1;

                        }
                        else // 중복 가능할 때는, 중복 여부에 따라 분배하고 ,슬로수를 증가시킨다.
                        {
                            Item AlreadyItem = null;
                            AlreadyItem = player.Inven.Get_template(questItemData.itemId);

                            // 중복이면 그냥 리워드만해준다.
                            if (AlreadyItem == null)
                            {
                                // 여러개 동시에 줄때에는 이것 이용해야함
                                DbTransaction.RewardPlayer(player, newData, room, slot);
                                i += 1;
                            }
                            else
                            {
                                DbTransaction.RewardPlayer(player, newData, room);
                            }

                        }






                  

                    }
                }

                if(IsAlreadyPacket == false)
                    player.Session.Send(snpcPacket);

                Console.WriteLine("퀘스트 getItem 변경 요청");
            }


            // get 이나 lose 없이 status만 변경하려고 할 경우
            if (IsNormal == true)
            {
                S_Npc snpcPacket = new S_Npc();
                snpcPacket.NpcInfo = npc.Info;
                snpcPacket.QuestId = questData.questId;
                snpcPacket.Dialogue = questData.dialogue[Order].index;
                player.Session.Send(snpcPacket);
            }



            // ★☆★☆★☆★☆★☆★☆★☆ 퀘스트 주기전에 먼저 아이템 주고 확인하고, 퀘스트를 주자.★☆★☆★☆★☆★☆★☆★☆

            Quest PlayerQuest = player.QuestInven.Find(i => i.QuestTemplateId == QuestId);

            if (PlayerQuest == null)
            {
                // 안 갖고 있다면 퀘스트를 갖게 만든다.
                Quest quest = Quest.MakeQuestFromId(player, QuestId);

                // 퀘스트 획득 ( 서버 메모리 저장 및 DB 저장  )
                if (DbTransaction.GetQuest(player, room, quest) == true)
                {

                }
            }
            // 퀘스트를 갖고 있는 상태라면, Status 변경해준다.
            else
            {

                int newTemplateId;
                int newStatus = questData.dialogue[Order].statusChange;

                // 만약 퀘스트 성공아니라면, Id도 새로운 Status에 맞게 변경해준다.
                if (newStatus != -1)
                    newTemplateId = (PlayerQuest.QuestTemplateId / 100) * 100 + newStatus;

                // 만약 퀘스트 성공이면, 최후의 QuestId를 처음(1000001) 으로 만들어주고, Status만 -1 로 해준다.
                else
                    newTemplateId = (PlayerQuest.QuestTemplateId / 100) * 100 + 1; // 처음 시작은 1 이다


                DbTransaction.ChangeQuest(player, room, PlayerQuest, questData.dialogue[Order].statusChange, newTemplateId);
            }



        }


    }
}
