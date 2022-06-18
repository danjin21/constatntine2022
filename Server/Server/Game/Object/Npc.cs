using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
    public class Npc : GameObject
    {
        public int TemplateId { get; private set; }
        public Inventory Inven { get; private set; } = new Inventory();

        //public QuestInventory QuestInven { get; private set; } = new QuestInventory();

        public List<QuestData> NpcQuestList = new List<QuestData>();


        public Npc()
        {
            ObjectType = GameObjectType.Npc;
            //Vision = new VisionCube(this);

        }





        public void Init(int templateId)
        {

            TemplateId = templateId;

            // TemplateId에서 해당 Npc의 데이터를 가지고 온다.
            NpcData npcData = null;
            DataManager.NpcDict.TryGetValue(TemplateId, out npcData);

            // 데이터 메모장에 있는 정보로 넣어준다.

            Stat.PosX = npcData.posX;
            Stat.PosY = npcData.posY;
            Stat.Map = npcData.map;
            Stat.Hp = 99999;
            Stat.MaxHp = 99999;
            State = CreatureState.Idle;

            // 퀘스트 DB 에서 npc가 자기랑 같은 애들만 가지고 온다.
            QuestData questData = null;
            DataManager.QuestDict.TryGetValue(TemplateId, out questData);


            List<QuestData> QuestList = new List<QuestData>();

            foreach(QuestData Quest in DataManager.QuestDict.Values)
            {
                Console.WriteLine($"퀴스트 정보 : {Quest.questId} / {Quest.npc}" );

                if(Quest.npc == templateId)
                {
                    QuestList.Add(Quest);
                }  
            }

            NpcQuestList = QuestList;


            foreach(QuestData p in NpcQuestList)
            {
                Console.WriteLine($"NPC({templateId})가 가지고 있는 퀘스트 : {p.questId} / {p.questName} / {p.status}");
            }
      

            // 퀘스트들을 넣는다



        }



        public override void OnDead(GameObject attacker, int damage)
        {
            // npc는 안죽는다.
        }

        public override int OnDamaged(GameObject attacker, int damage, int skillId, int shot = 1, GameObject Owner = null)
        {

            return 0;
        }
    }
}
