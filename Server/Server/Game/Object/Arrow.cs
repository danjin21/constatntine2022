using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        //long _nextMoveTick = 0;

        // 처음에는 느리게 움직이게 해야함
        public int FirstPing = 200;

        public override void Update()
        {
            // TODO
            if (Data == null || Data.projectile == null ||Owner == null || Room == null)
                return;

            //if (_nextMoveTick >= Environment.TickCount64)
            //    return;

            // 1000은 1초 , speed = 10 이라면 1초에 10칸을 간다는 뜻임  tick이라는 정보는 밀리세컨드
            // 타일이 32 칸이므로 1ㅊ초 *32를 해줘야한다.
            int tick = (int)((1000 * 32) / Data.projectile.speed);
            //_nextMoveTick = Environment.TickCount64 + tick;//원하는 시간 만큼

            // 처음에는 tick으 르닐게


            // 1초뒤에 실행
            Room.PushAfter(tick+FirstPing, Update);
            FirstPing = 0;


            // 먼저 내 화살 위치에 뭔가가 들어왔는지 확인하고 나서 앞에 뭐가 있는지 체크한다.

            Vector2Int destPos = GetFrontCellPos(); // 내 앞에 있는 방향 

            //Console.WriteLine("FIND(CELLPOS) = " + Room.Map.Find(CellPos));

 
            if (( Room.Map.Find(CellPos) == Owner || Room.Map.Find(CellPos) == null ) && Room.Map.ApplyMove(this, destPos, collision: false /*충돌영향안준다.*/))
            {

                //// 이동
                //CellPos = destPos;                
                // 이동은 ApplyMove로만 



                // 현재 위치에도 캐릭터가 없는 거라면 이동을 시킨다.

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(CellPos, movePacket);

                //Console.WriteLine("Move Arrow");



            }
            else
            {

                int OwnerAttack = new Random().Next(Owner.MinAttack, Owner.MaxAttack);  // 포함 , 포함되지 않는 숫자



                GameObject target = Room.Map.Find(destPos);

                if (target != null)
                {
                    // 이동한 거리에 따라 데미지를 준다.
                    // Data.id 는 skillData의 스킬 아이디 이다.

                    if (Damage_IsCurrent(target, (Player)Owner, OwnerAttack, Data.id) == false)
                    {
                        // #####몬스터 있떠라도 다안온거면 그냥 지나가게 해준다. #####checkObjects false로 해서.
                        if(Room.Map.ApplyMove(this, destPos, checkObjects : false ,collision: false /*충돌영향안준다.*/))
                        {
                            S_Move movePacket = new S_Move();
                            movePacket.ObjectId = Id;
                            movePacket.PosInfo = PosInfo;
                            Room.Broadcast(CellPos, movePacket);
                        }

                        return;
                    }

                }
                else
                {
                    // 현재 위치에 오브젝트가 있을 수도 있음으로 한번더 검사

                    target = Room.Map.Find(CellPos);

                    if(target != null)
                    {
                        // 이동한 거리에 따라 데미지를 준다.
                        // Data.id 는 skillData의 스킬 아이디 이다.
                        if (Damage_IsCurrent(target, (Player)Owner, OwnerAttack, Data.id ) == false)
                        {
                            // ###### 몬스터 있떠라도 다안온거면 그냥 지나가게 해준다. ######checkObjects false로 해서.
                            if (Room.Map.ApplyMove(this, destPos, checkObjects: false, collision: false /*충돌영향안준다.*/))
                            {
                                S_Move movePacket = new S_Move();
                                movePacket.ObjectId = Id;
                                movePacket.PosInfo = PosInfo;
                                Room.Broadcast(CellPos, movePacket);
                            }

                            return;
                        }                            

                    }

                }


                // 소멸
                //Room.LeaveGame(Id);

                Room.Push(Room.LeaveGame, Id); // => JobQueue 화 // 소멸
            }

        }


        public override GameObject GetOwner()
        {
            return Owner;
        }

        public bool Damage_IsCurrent(GameObject target, Player player, int OwnerAttack, int skillId)
        {
            if (target == Owner)
            {
                Room.Push(Room.LeaveGame, Id); // => JobQueue 화 // 소멸
                return false;
            }

            //// 허상으로 되어있는 화살들이면 그냥 날린다
            //if (IsFinal == false)
            //{
            //    Room.Push(Room.LeaveGame, Id); // => JobQueue 화 // 소멸
            //    return false;
            //}


            // 멈춰있는 상태 ( movetick은 가만히 있고, 환경시간만 증가하므로 계속 음수가 될수밖에 없다.)
            if ((target._nextMoveTick - Environment.TickCount64) < (int)((32 * 1000 / target.Speed) / 100.0f))
            {
                target.OnDamaged(this, Data.damage + OwnerAttack, skillId, shot, player);
                return true;
            }
            else
            {
                // 반을 못넘어왔지만, 걷는게 아니고 멈춰있었던 거라면 데미지를 준다.
                if (target.State != CreatureState.Moving)
                {
                    target.OnDamaged(this, Data.damage + OwnerAttack, skillId, shot, player);
                    return true;
                }


                // 같은 선상에 있을때는 약간 늦어도 화살을 맞는다.
                if(target.Dir == MoveDir.Left || target.Dir == MoveDir.Right)
                {
                    if (Dir == MoveDir.Left || Dir == MoveDir.Right)
                    {
                        target.OnDamaged(this, Data.damage + OwnerAttack, skillId, shot, player);
                        return true;
                    }
                }

                if(target.Dir == MoveDir.Up || target.Dir == MoveDir.Down)
                {
                    if (Dir == MoveDir.Up || Dir == MoveDir.Down)
                    {
                        target.OnDamaged(this, Data.damage + OwnerAttack, skillId , shot, player);
                        return true;
                    }
                }



            }

            return false;
        }

    }
}
