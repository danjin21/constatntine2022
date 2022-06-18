using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class DropItem : GameObject
    {
        public int TemplateId { get; private set; }

        public int Order { get; private set; }

        public RewardData RewardData { get; private set; }

        public Player Owner { get; private set; }



        // 생성자
        public DropItem()
        {
            ObjectType = GameObjectType.DropItem;

         
        }


        public void Init(Player player, RewardData rewardData, GameRoom room, int order)
        {

            RewardData = rewardData;
            Order = order;
            Owner = player;


            // TemplateId에서 해당 몬스터의 데이터를 가지고 온다.


            //// 데이터 메모장에 있는 정보로 넣어준다.
            //Stat.MergeFrom(monsterData.stat);

            //// 다만 체력은 최대 체력과 같게 따로 설정해준다.
            //Stat.Hp = monsterData.stat.MaxHp;
            //Stat.Exp = monsterData.stat.Exp;


        }



        public override void OnDead(GameObject attacker, int damage)
        {
            if (Room == null)
                return;


            // Room 임시저장
            GameRoom room = Room;




            // 아이템 생성
            GameObject owner = attacker.GetOwner();

            if (owner.ObjectType == GameObjectType.Player)  // pet이든 projectile 이든 포함이 될 수도 있다. 그래서 애들한테 owner를 불러오는 식
            {
                RewardData rewardData = RewardData;

                if (rewardData != null)
                {
                    Player player = (Player)owner;

                    // 아이템 넣고 디비에 저장해주세요~
                    // 그리고 성공을 할 경우에만 아이템을 없애준다.

                    if(DbTransaction.RewardPlayer(player, rewardData, room))
                    {

                        // 우선 죽음 패킷 보내고
                        S_Die diePacket = new S_Die();
                        diePacket.ObjectId = Id;
                        diePacket.AttackerId = attacker.Id;
                        Room.Broadcast(CellPos, diePacket);

                        // 방에서 나가게한다.
                        //room.LeaveGame(Id);
                        //room.Push(room.LeaveGame,Id); // => JobQueue 화 : 바로 실행되지 않을 수 있어 문제가 있음. -> 처리를해줘야함. 처리를해줌
                        room.LeaveGame(Id); // 따라서 바로 실행시켜준다.

                    }


                }
            }

        }






    }
}
