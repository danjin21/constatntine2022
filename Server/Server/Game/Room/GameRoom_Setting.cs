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


        public void HandleKeySetting(Player player, C_KeySetting keysettingPacket)
        {
            if (player == null)
                return;

            // Player -> HandleEquipItem 으로 옮김
            player.HandleKeySetting(keysettingPacket);
        }

        public void HandleShortKey(Player player, C_ShortKey shortKeyPacket)
        {
            if (player == null)
                return;

            if (player.Room == null)
                return;

            GameRoom room = player.Room;
            //if (player.ShortKeyCool == true)
            //    return;
            //// 쿨타임 주기.
            //player.ShortKeyCool = true;
            //player.Room.PushAfter(500, player.ShortKeyCooltime);



            // 해당 Action이 아이템인지 스킬인지 확인한다.

            if (shortKeyPacket.Action >= 1000000 )
            {
                //스킬사용
                C_Skill skillPacket = new C_Skill() { Info = new SkillInfo() };
                skillPacket.Info.SkillId = shortKeyPacket.Action; // 힐
                HandleSkill(player, skillPacket);
          

            }
            else if(shortKeyPacket.Action >=90000 && shortKeyPacket.Action < 1000000)
            {
                // 아이템이 있는지 확인 및 없으면 리턴한다.

                Item A = player.Inven.Get_template(shortKeyPacket.Action);

                if (A == null)
                  return;

                // 서버에서 자체 실행

                C_UseItem usePacket = new C_UseItem();
                usePacket.ItemDbId = A.ItemDbId;

                HandleUseItem(player, usePacket);

            }

            // # # 단축키 쿨타임 주기. 단축키 쿨타임은 스킬 쿨타임보다 항상 적게 해줘야 한다.
            player.ShortKeyCool = true;


            room.PushAfter(100, player.ShortKeyCooltime);

        }
        }
     



}
