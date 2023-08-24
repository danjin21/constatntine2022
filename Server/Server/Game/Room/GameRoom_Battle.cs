using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer   // 하나의 스레드에서 일감을 실행한다.
    {


        public void HandleMove(Player player, C_Move movePacket)
        {

            if (player == null)
                return;

            //// 스킬쓴 바로 직후에는 걷지못하게함
            if (player.SkillWalkCool == true)
                return;

            Console.WriteLine("걷는 중의 플레이어 상테 : " + player.State + "->" + movePacket.PosInfo.State);

            //lock (_lock)
            //{
            // TODO : 검증


            // 일단 서버에서 좌표 이동

            // 희망 이동 정보
            PositionInfo movePosInfo = movePacket.PosInfo;

            // 현재 플레이어 정보
            ObjectInfo info = player.Info;


  


            //클라단에서 왔다리 갔다리 하는거 해결
            //if (player.State == CreatureState.Skill && movePacket.PosInfo.State == CreatureState.Idle)
            //{
            //    info.PosInfo.State = movePosInfo.State;
            //    Console.WriteLine("테스트1");
            //    return;
            //}



            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
            {
                // 갈 수 없는지 체크
                if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                {
                    // 20221119 이거땜에 그.. 텔레포트가 안써져서.. 일단.. 그대로 둠
                    //player.State = CreatureState.Idle; // 갈수 없으니까 플레이어의 상태를 Idle로 바꿔준다.

                    // 남들에게 알려준다.
                    S_Move IdleMovePacket = new S_Move();
                    IdleMovePacket.ObjectId = player.Info.ObjectId;
                    IdleMovePacket.PosInfo = player.PosInfo;
                    Broadcast(player.CellPos, IdleMovePacket);

                    return;
                }

                // 2칸이상 이동하려면 return;
                if (Math.Abs(movePosInfo.PosX - info.PosInfo.PosX) > 1 || Math.Abs(movePosInfo.PosY - info.PosInfo.PosY) > 1)
                    return;
                // 대각선 이동 하면 return;
                if (Math.Abs(movePosInfo.PosX - info.PosInfo.PosX) >= 1 && Math.Abs(movePosInfo.PosY - info.PosInfo.PosY) >= 1)
                    return;
            }

            // 캐릭터의 state, move dir 을 갱신시켜준다. ( 서버내에서만? )
            info.PosInfo.State = movePosInfo.State;
            info.PosInfo.MoveDir = movePosInfo.MoveDir;

            // ★☆ 충돌체의 정보를 주는 곳  ★☆
            Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));


            // 다른 플레이어한테도 알려준다.

            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;

            Broadcast(player.CellPos,resMovePacket);

            //Console.WriteLine($"S_Move : {resMovePacket.PosInfo.PosX},{resMovePacket.PosInfo.PosY}");


            if ( Map.IsPortal(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) != null)
            {
                
                PortalData A = Map.IsPortal(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                
                Console.WriteLine("이동합니다.");
                Console.WriteLine($"다음맵 : {A.destMap}");
                Console.WriteLine($"다음좌표 X: {A.destPosX}");
                Console.WriteLine($"다음좌표 Y: {A.destPosY}");

                player.MoveMap( A.destMap,  A.destPosX,  A.destPosY);
            }


            //}
        }


        public void HandleSkill(Player player, C_Skill skillPacket)
        {

            Console.WriteLine("(1)스킬분기 : " + player.Id);
            if (player == null)
                return;

            GameRoom room = player.Room;

            if (room == null)
                return;


            // 더미스킬 패킷보내주기
            S_Skill skill_dummy = new S_Skill() { Info = new SkillInfo() }; // Info도 클래스이기 때문에 새로 만들어주어야한다.
            skill_dummy.ObjectId = player.Info.ObjectId;
            skill_dummy.Info.SkillId = -1;
            player.Session.Send(skill_dummy);

            Console.WriteLine($"★ / {player.State}");
            // 스킬을 가지고 있는지 확인

            Skills PlayerSkill = player.SkillInven.Find(i =>  i.SkillId == skillPacket.Info.SkillId);
            if (PlayerSkill == null)
                return;

            // 쿨확인 - 텔레포트는 쿨 상관 안한다. 우선 스킬 쓰자마자 바로 텔포 쓰는건 막아두자
            if (player.SkillCool == true && skillPacket.Info.SkillId != 3101000 && skillPacket.Info.SkillId != 4001000)
            {

                // 전과의 시간을 비교

                // 1
                // 충분히 길게 눌렀다면 예약을 해준다.
                if (player.SkillKeyContinue == true)
                    player.C_Skill_Book = skillPacket; // 예약을 걸어둔다.


                return;
            }

            Console.WriteLine($"★★  / {player.State}");

            // 텔레포트의 쿨 확인

            if (player.TeleportCool == true && skillPacket.Info.SkillId == 3101000)
                return;

            Console.WriteLine($"★★★  / {player.State}");

            // 순보 쿨 타임

            if (player.SoonboCool == true && skillPacket.Info.SkillId == 4001000)
            {
                // 충분히 길게 눌렀다면 예약을 해준다.
                // 공격할때 같이 먹혀야 해서 그래
                if (player.SkillKeyContinue_Soonbo == true)
                    player.C_Skill_Soonbo_Book = skillPacket; 
                return;
            }

            // 순보 스킬 직후 스킬 못쓰게
            if (player.SoonboCool == true )
            {

                if (player.SoonboComboCool == false)
                    return;
            }


            Console.WriteLine($"★★★★  / {player.State}");

            // 마나 없으면 리턴하기
            if (player.Stat.Mp < PlayerSkill.Mp)
                return;


            ObjectInfo info = player.Info;

            Console.WriteLine($"★★★★★  / {player.State}");

            if (player.Info.PosInfo.State == CreatureState.Idle && skillPacket.Info.SkillId == 3101000)
            {

                //// 스킬 패킷보내주기
                //S_Skill skill_2 = new S_Skill() { Info = new SkillInfo() }; // Info도 클래스이기 때문에 새로 만들어주어야한다.
                //skill_2.ObjectId = info.ObjectId;
                //skill_2.Info.SkillId = skillPacket.Info.SkillId;

                //Broadcast(player.CellPos, skill_2);

                //player.TeleportCool = true;
                //room.PushAfter(1000, player.TeleportCooltime);

                return;
            }

            Console.WriteLine($"★★★★★★  / {player.State}");

            if ((info.PosInfo.State != CreatureState.Idle) && skillPacket.Info.SkillId != 3101000 )
            {

                //if (info.PosInfo.State == CreatureState.Skill && player.SoonboCool == true)
                //{
                //    Console.WriteLine($"★★★★★★★  / {player.State}");
                //    if (player.SoonboComboCool == false)
                //        return;
                //}
                //else

                info.PosInfo.State = CreatureState.Idle;

                    return;
                //else
                //{
                //    Console.WriteLine($"★★★★★★★★  / {player.State}");
                //        return;
                //}


            }



            Console.WriteLine("텔레포트 쓸때 유저의 상태 :" + player.State + " -> Skill");
            // TODO : 스킬 사용 가능 여부 체크





            Data.Skill skillData = null;
             
            // 스킬 데이터가 없으면 return 
            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                return;

            // 타겟
            GameObject target = null;
            int realDamage = 0;

            Google.Protobuf.Protocol.ProjectileInfo tempProjectileInfo = new Google.Protobuf.Protocol.ProjectileInfo();
            tempProjectileInfo.PosInfo = new PositionInfo();


            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:
                    {
                        // TODO : 데미지 판정 
                        // 위치를 받는다.
                        Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                        target = Map.Find(skillPos);


                        // 승보일 경우
                       if (skillPacket.Info.SkillId == 4001000)
                        {
                            target = Map.Find(skillPos);

                            if (target != null)
                            {


                                // 그 적의 뒤로 갈 수 있는지 확인
                                // 방향은 플레이어여야 한다.

                                Vector2Int nextPos = target.GetFrontCellPos(player.PosInfo.MoveDir);

                                bool FinishSoonbo = true;

                                if (Map.ApplyMove(player, nextPos) == false)
                                {
                                    nextPos = target.GetLeftCellPos(player.PosInfo.MoveDir);

                                    if (Map.ApplyMove(player, nextPos) == false)
                                    {
                                        nextPos = target.GetRightCellPos(player.PosInfo.MoveDir);

                                        if (Map.ApplyMove(player, nextPos) == false)
                                        {
                                            // 순보를 못 마쳤다.
                                            FinishSoonbo = false;
                                        }
                                    }
                                }

                                // 순보를 마쳤을 때만
                                if (FinishSoonbo == true)
                                {

                                    // 몬스터 바라보게

                                    Vector2Int dir = target.CellPos - nextPos;

                                    if (dir.x > 0)
                                        player.PosInfo.MoveDir = MoveDir.Right;
                                    else if (dir.x < 0)
                                        player.PosInfo.MoveDir = MoveDir.Left;
                                    else if (dir.y > 0)
                                        player.PosInfo.MoveDir = MoveDir.Up;
                                    else /*if (dir.y < 0)*/
                                        player.PosInfo.MoveDir = MoveDir.Down;
                                }


                            }
                            else
                            {
                                return;
                            }

                        }



                        // 공격일 경우
                        if (skillPacket.Info.SkillId == 9001000)
                        {


                            // 화살을 들고 있다면 화살 하나를 소환한다.

                            // 플레이어가 착용한 무기가 활이 아니면 리턴

                            Console.WriteLine("@1");
                            Item bowItem = null;
                            bowItem = player.Inven.Find(i => i.Equipped && i.ItemType == ItemType.Weapon);

                            Console.WriteLine("@2");
                            if (bowItem != null && ((Weapon)bowItem).WeaponType == WeaponType.Bow)
                            {


                                Arrow serverArrow = ObjectManager.Instance.Add<Arrow>();
                                if (serverArrow == null)
                                    break; // 아래까지 내려가야 하므로

                                DataManager.SkillDict.TryGetValue(2001001, out skillData);

                                serverArrow.Owner = player;

                                // 투사체 에게 스킬 데이터를 넣어준다.
                                serverArrow.Data = skillData;

                                serverArrow.PosInfo.State = CreatureState.Moving;
                                serverArrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                                serverArrow.PosInfo.PosX = player.PosInfo.PosX;
                                serverArrow.PosInfo.PosY = player.PosInfo.PosY;
                                serverArrow.IsFinal = true;
                                //Push(EnterGame, arrow, false); // => JobQueue 화  

                                serverArrow.Speed = skillData.projectile.speed;
                                serverArrow.shot = 1;
                                serverArrow.Stat.Hp = 1; // 이건 4개 소환하기 위해 쓴거임.



                                int distance;
                                distance = 15;

                                // 패킷에 넣을 정보 생성
                                tempProjectileInfo.OwnerId = player.Id;
                                tempProjectileInfo.PosInfo.PosX = serverArrow.PosInfo.PosX;
                                tempProjectileInfo.PosInfo.PosY = serverArrow.PosInfo.PosY;
                                tempProjectileInfo.PosInfo.MoveDir = serverArrow.PosInfo.MoveDir;
                                tempProjectileInfo.PosInfo.State = serverArrow.PosInfo.State;
                                tempProjectileInfo.TargetId = -1;
                                tempProjectileInfo.Distance = distance;
                                tempProjectileInfo.Shots = serverArrow.shot;

                                bool shot = false;
                                int i = 0;

                                // 자기 앞 칸에 오브젝트가 있으면 공격을 준다.
                                // 10칸
                                while (shot ==false && i < distance)
                                {
                                    // 자기 방향을 구하고,
                                    Vector2Int destPos = serverArrow.GetFrontCellPos(); // 내 앞 방향



                                    if (room.Map.Find(destPos) == null)
                                    {
                                        //room.Map.ApplyMove(serverArrow, destPos, collision: false /*충돌영향안준다.*/);

                                        // 충돌체

                                        if(room.Map.CanGo(destPos,false) == false)
                                        {
                                            tempProjectileInfo.Distance = i+1;
                                            break;
                                        }


                                        serverArrow.PosInfo.PosX = destPos.x;
                                        serverArrow.PosInfo.PosY = destPos.y;

                                        i += 1;
                                    }
                                    else
                                    {

                                        target = room.Map.Find(destPos);

                                        int OwnerAttack = new Random().Next(serverArrow.Owner.MinAttack, serverArrow.Owner.MaxAttack);  // 포함 , 포함되지 않는 숫자

                                        GameObject serverTarget = room.Map.Find(destPos);
                                        serverTarget.OnDamaged(serverArrow, serverArrow.Data.damage + OwnerAttack, serverArrow.Data.id, serverArrow.shot, player);

                                        shot = true;

                                        tempProjectileInfo.TargetId = target.Id;


                                    }
                                }

                 

                                //Push(EnterGame, serverArrow, false); // => JobQueue 화  



                                if (false)
                                {
                                    // 그 위치에 있는 몬스터에게 데미지를 준다.

                                    DataManager.SkillDict.TryGetValue(2001001, out skillData);


                                    Console.WriteLine("@3");
                                    // TODO : Arrow ( 일단 에로우만 넣음 )
                                    Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                                    if (arrow == null)
                                        break; // 아래까지 내려가야 하므로

                                    arrow.Owner = player;


                                    // 스킬 데이터가 없으면 return 
                                    // 임시로 더블샷 스킬데이터로 넣어준다


                                    // 투사체 에게 스킬 데이터를 넣어준다.
                                    arrow.Data = skillData;

                                    arrow.PosInfo.State = CreatureState.Moving;
                                    arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                                    arrow.PosInfo.PosX = player.PosInfo.PosX;
                                    arrow.PosInfo.PosY = player.PosInfo.PosY;

                                    Console.WriteLine("@4");

                                    // 투사체의 스피드를 데이터에서 불러온다.
                                    arrow.Speed = skillData.projectile.speed;
                                    arrow.IsFinal = true;
                                    Push(EnterGame, arrow, false); // => JobQueue 화  


                                    Console.WriteLine("@5" + skillData.skillType);
                                }
                              
                            }
                            else // 활 안들고 있을때
                            {
                                if (target != null)
                                {
                                    // TODO : 피격판정

                                    //if (target.ObjectType == GameObjectType.Monster)
                                    //{
                                    //    // 반이상 왔을때만
                                    //    if (Math.Abs(target.moveProcess) < (int)((32 * 1000 / target.Speed) / 2))
                                    //    {
                                    //        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                    //        realDamage = target.OnDamaged(player, TryAttack);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                    //    }

                                    //}
                                    //else if(target.ObjectType == GameObjectType.Player)
                                    //{

                                    //    // 반이상 왔을때만
                                    //    if (Math.Abs(target.moveProcess) < (int)((32 * 1000 / target.Speed) / 2))
                                    //    {
                                    //        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                    //        realDamage = target.OnDamaged(player, TryAttack);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                    //    }

                                    //    //// 플레이어도 반이상 왔을때 타격하는것 연구해 봐야함.

                                    //    //int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                    //    //realDamage = target.OnDamaged(player, TryAttack);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                    //}




                                    // 멈춰있는 상태 ( movetick은 가만히 있고, 환경시간만 증가하므로 계속 음수가 될수밖에 없다.)
                                    if ((target._nextMoveTick - Environment.TickCount64) < (int)((32 * 1000 / target.Speed) / 2.0f))
                                    {
                                        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                        realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId, 1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                    }
                                    else
                                    {
                                        // 반을 못넘어왔지만, 걷는게 아니고 멈춰있었던 거라면 데미지를 준다.
                                        if (target.State != CreatureState.Moving)
                                        {
                                            int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                            realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId, 1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                        }

                                        //Console.WriteLine("###다 안왔죵? 둘의 차이 : " + (target._nextMoveTick - Environment.TickCount64));
                                    }

                                    //Console.WriteLine("멈춰있고 떄릴때 몬스터의 실시간 이동 진행과정 = " + target._nextMoveTick + "/환경시간 : "+ Environment.TickCount64 + "/ 둘의 차이 : " + (target._nextMoveTick- Environment.TickCount64) + "/ 반의 거리 : " + (int)((32 * 1000 / target.Speed) / 2.0f));






                                }

                            }


                        }



                        // 이격일 경우
                        if (skillPacket.Info.SkillId == 1001000)
                        {

                            for (int i = 0; i < 4; i++)
                            {
                                if (i == 0)
                                    skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                                else if (i == 1)
                                    skillPos = player.GetBackCellPos(info.PosInfo.MoveDir);
                                else if (i == 2)
                                    skillPos = player.GetRightCellPos(info.PosInfo.MoveDir);
                                else if (i == 3)
                                    skillPos = player.GetLeftCellPos(info.PosInfo.MoveDir);


                                target = Map.Find(skillPos);


                                if (target != null)
                                {


                                    // 멈춰있는 상태 ( movetick은 가만히 있고, 환경시간만 증가하므로 계속 음수가 될수밖에 없다.)
                                    if ((target._nextMoveTick - Environment.TickCount64) < (int)((32 * 1000 / target.Speed) / 2.0f))
                                    {
                                        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                        realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId ,1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                    }
                                    else
                                    {
                                        // 반을 못넘어왔지만, 걷는게 아니고 멈춰있었던 거라면 데미지를 준다.
                                        if (target.State != CreatureState.Moving)
                                        {
                                            int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                            realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId, 1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                        }

                                    }

                                }
                            }
                        }

                        // 삼격일 경우
                        if (skillPacket.Info.SkillId == 1001001)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                if(i == 0)
                                    skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                                else if (i == 1)
                                    skillPos = player.GetRightCellPos(info.PosInfo.MoveDir);
                                else if (i == 2)
                                    skillPos = player.GetLeftCellPos(info.PosInfo.MoveDir);

                                target = Map.Find(skillPos);


                                if (target != null)
                                {


                                    // 멈춰있는 상태 ( movetick은 가만히 있고, 환경시간만 증가하므로 계속 음수가 될수밖에 없다.)
                                    if ((target._nextMoveTick - Environment.TickCount64) < (int)((32 * 1000 / target.Speed) / 2.0f))
                                    {
                                        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                        realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId, 1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                    }
                                    else
                                    {
                                        // 반을 못넘어왔지만, 걷는게 아니고 멈춰있었던 거라면 데미지를 준다.
                                        if (target.State != CreatureState.Moving)
                                        {
                                            int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                            realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId, 1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                        }

                                    }

                                }
                            }
                        }


                        // 힐일 경우
                        if (skillPacket.Info.SkillId == 3101002)
                        {
                            // Int만큼 체력을 올려준다.
                            target = player;
                            target.OnHealed(player, player.TotalInt);
                            Console.WriteLine($"Heal!! + {player.TotalInt} ");

                            realDamage = player.Stat.Int;
                        }

                        // 텔레포트인 경우

                        if(skillPacket.Info.SkillId == 3101000)
                        {

                            // 방향키를 누르고 있는 상태가 아니면 리턴한다.
                            //if (player.State != CreatureState.Moving)
                            //    return;

                            Vector2Int destPos = new Vector2Int();
                            int Dist = 4;
                            switch (player.Dir)
                            {
                                case MoveDir.Down:
                                    destPos = player.CellPos + new Vector2Int(0, -Dist);
                                    break;
                                case MoveDir.Up:
                                    destPos = player.CellPos + new Vector2Int(0, Dist);
                                    break;
                                case MoveDir.Left:
                                    destPos = player.CellPos + new Vector2Int(-Dist, 0);
                                    break;
                                case MoveDir.Right:
                                    destPos = player.CellPos + new Vector2Int(Dist, 0);
                                    break;
                            }




                            // 진형쓰룰
                            int countX = 0;
                            int countY = 0;

                            // ★☆ 충돌체의 정보를 주는 곳  ★☆
                            // 포탈일 경우에도 앞에 멈추게 한다. <-- 이걸 먼저해야, 먼저 이동하지 않는다.
                            while (Map.IsPortal(new Vector2Int(destPos.x + countX, destPos.y + countY)) != null || Map.ApplyMove(player, new Vector2Int(destPos.x + countX, destPos.y + countY)) == false )
                            {
                                Dist -= 1;

                                Dist = Math.Max(0, Dist);


                                switch (player.Dir)
                                {
                                    case MoveDir.Down:
                                        destPos = player.CellPos + new Vector2Int(0, -Dist);
                                        break;
                                    case MoveDir.Up:
                                        destPos = player.CellPos + new Vector2Int(0, Dist);
                                        break;
                                    case MoveDir.Left:
                                        destPos = player.CellPos + new Vector2Int(-Dist, 0);
                                        break;
                                    case MoveDir.Right:
                                        destPos = player.CellPos + new Vector2Int(Dist, 0);
                                        break;
                                }

                                if (Dist == 0)
                                    break;

                                //countX++;

                                //// 진형쓰룰
                                //if (player.CellPos.x + countX < Map.MinX || player.CellPos.x + countX >= Map.MaxX)
                                //{
                                //    countX = 0;
                                //    countY++;
                                //}


                                //if (player.CellPos.y + countY < Map.MinY || player.CellPos.y + countY >= Map.MaxY)
                                //    break;
                            }

                            //Map.ApplyMove(player, new Vector2Int(destPos.x + countX, destPos.y + countY));


                            // 다른 플레이어한테도 알려준다.

                            S_Teleport resMovePacket = new S_Teleport();
                            resMovePacket.ObjectId = player.Info.ObjectId;
                            resMovePacket.PosInfo = player.PosInfo;
                            Broadcast(player.CellPos, resMovePacket);

                            //// 포탈일 경우 순간이동
                            //if (Map.IsPortal(new Vector2Int(destPos.x + countX, destPos.y + countY)) != null)
                            //{

                            //    PortalData A = Map.IsPortal(new Vector2Int(destPos.x + countX, destPos.y + countY));

                            //    player.MoveMap(A.destMap, A.destPosX, A.destPosY);
                            //}

                        }




                        // 썬더볼트인 경우

                        if (skillPacket.Info.SkillId == 3111002)
                        {

                            // 갯수를 정한다.
                            int hitCount = 49;

                            // 리스트에 넣는다.

                            for(int i = 0; i< 9; i++)
                            {

                                for(int t = 0; t < 9; t ++)
                                {
                                    Vector2Int TargetPos = player.CellPos + new Vector2Int(-4 + t , 4-i);
                                    target = Map.Find(TargetPos);

                                    if (target != null)
                                    {
                                        // 자기자신은 제외한다.
                                        if (target == player)
                                            continue;


                                        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                        realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId, 1, player);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.

                                        //// 멈춰있는 상태 ( movetick은 가만히 있고, 환경시간만 증가하므로 계속 음수가 될수밖에 없다.)
                                        //if ((target._nextMoveTick - Environment.TickCount64) < (int)((32 * 1000 / target.Speed) / 2.0f))
                                        //{
                                        //    int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                        //    realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                        //}
                                        //else
                                        //{
                                        //    // 반을 못넘어왔지만, 걷는게 아니고 멈춰있었던 거라면 데미지를 준다.
                                        //    if (target.State != CreatureState.Moving)
                                        //    {
                                        //        int TryAttack = new Random().Next(player.MinAttack, player.MaxAttack);
                                        //        realDamage = target.OnDamaged(player, TryAttack, skillPacket.Info.SkillId);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                                        //    }

                                        //}

                                    }
                                }
                            }


                        }




                    }
                    break;
                case SkillType.SkillProjectile:
                    {
                        if (skillPacket.Info.SkillId == 3111002)
                        {
                        }


                        Arrow serverArrow = ObjectManager.Instance.Add<Arrow>();
                        if (serverArrow == null)
                            break; // 아래까지 내려가야 하므로

                        DataManager.SkillDict.TryGetValue(2001001, out skillData);

                        serverArrow.Owner = player;

                        // 투사체 에게 스킬 데이터를 넣어준다.
                        serverArrow.Data = skillData;

                        serverArrow.PosInfo.State = CreatureState.Moving;
                        serverArrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        serverArrow.PosInfo.PosX = player.PosInfo.PosX;
                        serverArrow.PosInfo.PosY = player.PosInfo.PosY;
                        serverArrow.IsFinal = true;
                        //Push(EnterGame, arrow, false); // => JobQueue 화  

                        serverArrow.Speed = skillData.projectile.speed;
                        serverArrow.shot = 2;
                        serverArrow.Stat.Hp = 2; // 이건 4개 소환하기 위해 쓴거임.



                        int distance;
                        distance = 15;

                        // 패킷에 넣을 정보 생성
                        tempProjectileInfo.OwnerId = player.Id;
                        tempProjectileInfo.PosInfo.PosX = serverArrow.PosInfo.PosX;
                        tempProjectileInfo.PosInfo.PosY = serverArrow.PosInfo.PosY;
                        tempProjectileInfo.PosInfo.MoveDir = serverArrow.PosInfo.MoveDir;
                        tempProjectileInfo.PosInfo.State = serverArrow.PosInfo.State;
                        tempProjectileInfo.TargetId = -1;
                        tempProjectileInfo.Distance = distance;
                        tempProjectileInfo.Shots = serverArrow.shot;

                        bool shot = false;
                        int i = 0;

                        // 자기 앞 칸에 오브젝트가 있으면 공격을 준다.
                        // 10칸
                        while (shot == false && i < distance)
                        {
                            // 자기 방향을 구하고,
                            Vector2Int destPos = serverArrow.GetFrontCellPos(); // 내 앞 방향



                            if (room.Map.Find(destPos) == null)
                            {
                                //room.Map.ApplyMove(serverArrow, destPos, collision: false /*충돌영향안준다.*/);


                                // 충돌체

                                if (room.Map.CanGo(destPos, false) == false)
                                {
                                    tempProjectileInfo.Distance = i + 1;
                                    break;
                                }

                                serverArrow.PosInfo.PosX = destPos.x;
                                serverArrow.PosInfo.PosY = destPos.y;

                                i += 1;
                            }
                            else
                            {

                                target = room.Map.Find(destPos);

                                int OwnerAttack = new Random().Next(serverArrow.Owner.MinAttack, serverArrow.Owner.MaxAttack);  // 포함 , 포함되지 않는 숫자

                                GameObject serverTarget = room.Map.Find(destPos);
                                serverTarget.OnDamaged(serverArrow, serverArrow.Data.damage + OwnerAttack, serverArrow.Data.id, serverArrow.shot, player);

                                shot = true;

                                tempProjectileInfo.TargetId = target.Id;


                            }
                        }



                        //// 플레이어가 착용한 무기가 활이 아니면 리턴

                        //Item bowItem = null;
                        //bowItem = player.Inven.Find(i => i.Equipped && i.ItemType == ItemType.Weapon);

                        //if (((Weapon)bowItem).WeaponType != WeaponType.Bow)
                        //{
                        //    // 중단해야 하므로
                        //    return;
                        //    // TODO : 스킬 사용 가능 여부 체크
                        //    // break; // 아래까지 내려가야 하므로
                        //}

                        //// TODO : Arrow ( 일단 에로우만 넣음 )
                        //Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                        //if (arrow == null)
                        //    break; // 아래까지 내려가야 하므로

                        //arrow.Owner = player;

                        //// 투사체 에게 스킬 데이터를 넣어준다.
                        //arrow.Data = skillData;

                        //arrow.PosInfo.State = CreatureState.Moving;
                        //arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        //arrow.PosInfo.PosX = player.PosInfo.PosX;
                        //arrow.PosInfo.PosY = player.PosInfo.PosY;

                        //// 투사체의 스피드를 데이터에서 불러온다.
                        //arrow.Speed = skillData.projectile.speed;
                        //Console.WriteLine($"arrow speed : { arrow.Speed}");
                        ////EnterGame(arrow);
                        //arrow.IsFinal = true;
                        //arrow.shot = 4;
                        //arrow.Stat.Hp = 4; // 이건 4개 소환하기 위해 쓴거임.

                        //// shot 과 hp 4 로 하면 4발이나간다.

                        //Push(EnterGame, arrow, false); // => JobQueue 화  


                        ////Arrow arrow_2 = ObjectManager.Instance.Add<Arrow>();
                        ////if (arrow_2 == null)
                        ////    return;

                        ////arrow_2.Owner = player;

                        ////// 투사체 에게 스킬 데이터를 넣어준다.
                        ////arrow_2.Data = skillData;

                        ////arrow_2.PosInfo.State = CreatureState.Moving;
                        ////arrow_2.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        ////arrow_2.PosInfo.PosX = player.PosInfo.PosX;
                        ////arrow_2.PosInfo.PosY = player.PosInfo.PosY;


                        ////// 투사체의 스피드를 데이터에서 불러온다.
                        ////arrow_2.Speed = skillData.projectile.speed;

                        ////arrow_2.IsFinal = false;

                        ////PushAfter(100, EnterGame, arrow_2, false);




                    }
                    break;

                case SkillType.ItemDrop:
                    {
                        // TODO : 데미지 판정 
                        // 위치를 받는다.
                        Vector2Int skillPos = player.CellPos;

                        Zone NowZone = GetZone(skillPos);
                        target = NowZone.FindOneItem(i => i.CellPos.x == skillPos.x && i.CellPos.y == skillPos.y);

                        // 줍기일 경우
                        if (skillPacket.Info.SkillId == 9001001)
                        {

                            if (target != null)
                            {
                                // TODO : 피격판정
                                target.OnDead(player, 0);   // 공격자와 데미지+화살의 데미지를 넣는다. this 는 나중에 Owner로 바꿀수있다.
                               
                            }
                        }

          

                    }
                    break;
            }

            // 텔레포트 아닌 경우에만 스킬 State
            if (skillPacket.Info.SkillId != 3101000 && skillPacket.Info.SkillId != 4001000)
                info.PosInfo.State = CreatureState.Skill;


            // 스킬 패킷보내주기
            // 스킬 패킷 생성
            S_Skill skill = new S_Skill() { Info = new SkillInfo() }; // Info도 클래스이기 때문에 새로 만들어주어야한다.
            skill.ObjectId = info.ObjectId;
            skill.Info.SkillId = skillPacket.Info.SkillId;
            //skill.Damage = player.TotalAttack;
            skill.Damage = realDamage ;
            skill.Info.MoveDir = player.Info.PosInfo.MoveDir; // 스킬쓴 방향 저장 
            skill.ProjectileInfo = tempProjectileInfo;
            skill.PosInfo = player.Info.PosInfo; // 스킬쓴 위치 저장
            Console.WriteLine($"후보 : {player.PosInfo} / {player.Info.PosInfo}");

            if (target != null)
                 skill.TargetId = target.Id;
            else
                skill.TargetId = -1;
              Broadcast(player.CellPos,skill);
           



            if(skillPacket.Info.SkillId == 3101000)
            {
                // ＃＃텔레포트 쿨타임 주기. Idle 로 인해 return 되는데 먼저 실행되면 안되지!

   
                player.TeleportCool = true;
                room.PushAfter(1000, player.TeleportCooltime);
            }
            else if(skillPacket.Info.SkillId == 3111002)
            {
                // ＃＃스킬 쿨타임 주기. Idle 로 인해 return 되는데 먼저 실행되면 안되지!
                player.SkillCool = true;
                room.PushAfter(800, player.SkillCooltime);


            }
            else if (skillPacket.Info.SkillId == 4001000)
            {
                player.SoonboCool = true;
                room.PushAfter(700, player.SoonboCooltime);


                // 순보쓰자마자 스킬 쓸 수 잇는 시간 0.1초
                player.SoonboComboCool = true;
                room.PushAfter(250, player.Soonbo_Combo_Cooltime);

                // 스킬쓰자마자 걷지 못하게 ( 텔레포트는 제외 )
                player.SkillWalkCool = true;
                room.PushAfter(200, player.SkillWalkCooltime);

            }
            else if(skillPacket.Info.SkillId == 1001001)
            {

                player.SkillCool = true;
                room.PushAfter(700, player.SkillCooltime);


                // 스킬쓰자마자 걷지 못하게 ( 텔레포트는 제외 )
                player.SkillWalkCool = true;
                room.PushAfter(200, player.SkillWalkCooltime);
            }
            else // 그외 스킬 쿨타임 주기
            {

                // ＃＃스킬 쿨타임 주기. Idle 로 인해 return 되는데 먼저 실행되면 안되지!


                player.SkillCool = true;
                room.PushAfter(800, player.SkillCooltime);


                // 스킬쓰자마자 걷지 못하게 ( 텔레포트는 제외 )
                player.SkillWalkCool = true;
                room.PushAfter(200, player.SkillWalkCooltime);

            }

            if (skillPacket.Info.SkillId != 4001000)
            {
                // continue 키 ( 지속적으로 키를 눌렀는지를 확인하는 )
                // 처음에는 지속적으로 눌르지 않았을테니 false
                player.SkillKeyContinue = false;
                room.PushAfter(200, player.SkillKeyContinueCooltime);
            }
            else
            {

                // continue 키 ( 지속적으로 키를 눌렀는지를 확인하는 )
                // 처음에는 지속적으로 눌르지 않았을테니 false
                player.SkillKeyContinue_Soonbo = false;
                room.PushAfter(200, player.SkillKeyContinue_Soonbo_Cooltime);
            }



            // 마나를 소비하자.



            int MpConsume = PlayerSkill.Mp;

            // 최대 마나보다 높아지면 최대마나로.
            player.Stat.Mp = Math.Max(player.Stat.Mp - MpConsume, 0);

            // 브로드 캐스팅
            S_ChangeMp changePacket = new S_ChangeMp();
            changePacket.ObjectId = player.Id;
            changePacket.Mp = player.Stat.Mp;
            changePacket.Damage = MpConsume;
            Broadcast(player.CellPos, changePacket);



            // 스킬쓰자마자 남들에게 알려준다.
            S_Move IdleMovePacket = new S_Move();
            IdleMovePacket.ObjectId = player.Info.ObjectId;
            IdleMovePacket.PosInfo = player.PosInfo;
            Broadcast(player.CellPos, IdleMovePacket);



            //Console.WriteLine("스킬 사용");

            //// # # 단축키 쿨타임 주기. 단축키 쿨타임은 스킬 쿨타임보다 항상 적게 해줘야 한다.
            //player.ShortKeyCool = true;
            //player.Room.PushAfter(700, player.ShortKeyCooltime);

            //}
        }


        public void HandleChat(Player player, C_Chat chatPacket)
        {

            if (player == null)
                return;

            // TODO : 검증


            // Chat에서 앞 부분을 확인한다.
            // /w 닉네임

            //string chat = chatPacket.Message;

            //int startIndex = 0;
            //int length = 3;

            //string substring = chat.Substring(startIndex, length);


            //// 앞 두글자가 /w공백 인지 확인
            //// /w공백이면 그 다음 문자부터 그 다음 공백까지 찾아서 닉네임으로 하기.

            //if (substring == "/w " || substring == "/ㅈ ")
            //{
            //    Console.WriteLine($"귓속말 요청");

            //    string[] words = chat.Split(' ');

            //    Console.WriteLine($"귓속말 받는 유저 : {words[1]}");

            //    string A = "";
            //    int i = 0;
            //    foreach( var word in words)
            //    {
            //        if (i == 0 || i == 1)
            //        {
            //            i++;
            //            continue;
            //        }

            //        A += (word+" ");
            //        i++;
            //    }

            //    string Temp = A.Substring(0, A.Length - 2);
            //    Console.WriteLine($"귓속말 내용 : {Temp}");
                
            //    // 개인 플레이어와, 다른 플레이어에게 말해주되,
            //    // 귓속말이라는것을 말해준다.


            //}
            //else
            //{
            //    // 현재 플레이어 정보
            //    ObjectInfo info = player.Info;

            //    // 다른 플레이어한테도 알려준다.

            //    S_Chat resChatPacket = new S_Chat();
            //    resChatPacket.ObjectId = player.Info.ObjectId;
            //    resChatPacket.Name = player.Info.Name;
            //    resChatPacket.Message = chatPacket.Message;

            //    Broadcast(player.CellPos, resChatPacket);
            //}



            // 현재 플레이어 정보
            ObjectInfo info = player.Info;

            // 다른 플레이어한테도 알려준다.

            S_Chat resChatPacket = new S_Chat();
            resChatPacket.ObjectId = player.Info.ObjectId;
            resChatPacket.Name = player.Info.Name;
            resChatPacket.Message = chatPacket.Message;

            Broadcast(player.CellPos, resChatPacket);



            // 전체 _players 중에서 닉네임으로 같은 유저를 찾는다.
            // 그 유저와 이 유저에게 Send를 한다.

        }


    }

}
