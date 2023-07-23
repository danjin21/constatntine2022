using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        
        // 게임 안에서의 Id지, DB의 Id는 아니다.
        public int Id
        {
            get { return Info.ObjectId;  }
            set { Info.ObjectId = value;  }
        }


        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        // 플레이어는 다른 설정임
        public virtual int TotalAttack { get { return Stat.Attack; } }
        public virtual int TotalDefence { get { return 0; } }


        public virtual int MaxAttack { get { return TotalAttack; } }
        public virtual int MinAttack { get { return TotalAttack; } }

        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }

        public int Hp
        {
            get { return Stat.Hp; }
            set { Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp);  }
        }

        public int Mp
        {
            get { return Stat.Mp; }
            set { Stat.Mp = Math.Clamp(value, 0, Stat.MaxMp); }
        }



        public MoveDir Dir
        {
            get { return PosInfo.MoveDir; }
            set { PosInfo.MoveDir = value; }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        public long moveProcess;
        public long _nextMoveTick = 0;


        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;

        }


        public virtual void Update()
        {

        }



        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        // 전방 찾기

        public Vector2Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }


        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;

            }

            return cellPos;
        }

        // 후방 찾기

        public Vector2Int GetBackCellPos()
        {
            return GetBackCellPos(PosInfo.MoveDir);
        }


        public Vector2Int GetBackCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.right;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.left;
                    break;

            }

            return cellPos;
        }



        // 좌방 찾기

        public Vector2Int GetLeftCellPos()
        {
            return GetLeftCellPos(PosInfo.MoveDir);
        }


        public Vector2Int GetLeftCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.right;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.up;
                    break;

            }

            return cellPos;
        }



        // 우방 찾기

        public Vector2Int GetRightCellPos()
        {
            return GetRightCellPos(PosInfo.MoveDir);
        }


        public Vector2Int GetRightCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.right;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.down;
                    break;

            }

            return cellPos;
        }




        // 외부에서 필요하면  static
        public static MoveDir GetDirFromVec(Vector2Int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else /*if (dir.y < 0)*/
                return MoveDir.Down;
        }



        public virtual int OnDamaged(GameObject attacker, int damage, int skillId , int shot = 1, GameObject Owner  = null)
        {
            if (Room == null)
                return 0;
            
            // 마을에서는 안싸우게.
            if (Room.RoomId == 4 || Room.RoomId == 10100031)
                return 0;

            //if (Room == null)
            //    return;

            List<DamageInfo> DamageList = new List<DamageInfo>();

            int OwnerAttack;

            int TotalDamage = 0;

            for (int i = 0; i< shot; i++)
            {
                OwnerAttack = new Random().Next(Owner.MinAttack, Owner.MaxAttack);  // 포함 , 포함되지 않는 숫자



                // 데미지 계산 (방어력 계산)
                OwnerAttack = Math.Max(OwnerAttack - TotalDefence, 0);

                // 0 보다 작아지면 0으로
                Stat.Hp = Math.Max(Stat.Hp - OwnerAttack, 0);

       



                DamageInfo damageInfo = new DamageInfo();

                damageInfo.Damage = OwnerAttack;
                damageInfo.Kind = 1;

                if (Owner.ObjectType == GameObjectType.Player)
                {
                    damageInfo.Damage = OwnerAttack*2;
                    damageInfo.Kind = 2;
                }


                DamageList.Add(damageInfo);

                TotalDamage += OwnerAttack;
            }

            Projectile A = attacker as Projectile;

            if (attacker.ObjectType == GameObjectType.Projectile && A.IsFinal == true)
            {


                // TODO :

                // 브로드 캐스팅
                S_ChangeHp changePacket = new S_ChangeHp();
                changePacket.ObjectId = Id;
                changePacket.Hp = Stat.Hp;
                changePacket.Damage = damage;
                changePacket.SkillId = skillId;
                changePacket.AttackerId = Owner.Id; // 화살이니까 Owner의 아이디를 준다.

                for (int i = 0; i < shot; i++)
                {
                    changePacket.MultiDamage.Add(DamageList[i]);
                }

                Room.Broadcast(CellPos, changePacket);
            }
            else if(attacker.ObjectType != GameObjectType.Projectile)
            {
                // TODO :

                // 브로드 캐스팅
                S_ChangeHp changePacket = new S_ChangeHp();
                changePacket.ObjectId = Id;
                changePacket.Hp = Stat.Hp;
                changePacket.Damage = damage;
                changePacket.SkillId = skillId;
                changePacket.AttackerId = attacker.Id; 

                for (int i = 0; i < shot; i++)
                {
                    changePacket.MultiDamage.Add(DamageList[i]);
                }

                Room.Broadcast(CellPos, changePacket);
            }



            // 죽으면 처리
            if(Stat.Hp <= 0)
            {


                if (attacker.ObjectType == GameObjectType.Projectile)
                {


                    // 화살 하나로 여러발 떄릴떄
                    OnDead(attacker, damage);

                    // 화살 여러발 소환할때는 밑에를 활성화

                    //if (A.IsFinal == false)
                    //    OnDead(attacker, damage);
                    //else
                    //{
                    //    // 화살이 하나라면 그냥 바로 죽게 만든다.
                    //    if(shot== 1)
                    //    {
                    //        OnDead(attacker, damage);
                    //    }

                    //}


                }
                else
                    OnDead(attacker, TotalDamage);

                //if(attacker.ObjectType == GameObjectType.Projectile)
                //{
                //    Projectile A = attacker as Projectile;

                //    if (A.IsFinal == false)
                //        Room.PushAfter(1000, OnDead, attacker, damage);
                //    else
                //        OnDead(attacker, damage);

                //}
                //else
                //    OnDead(attacker, damage);                
            }

            return TotalDamage;
        }


        public virtual void OnHealed(GameObject healer, int healdamage)
        {
            if (Room == null)
                return;

            //if (Room == null)
            //    return;


            // 최대 체력보다 높아지면 최대체력으로.
            Stat.Hp = Math.Min(Stat.Hp + healdamage, Stat.MaxHp);

            // TODO :

            // 브로드 캐스팅
            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            changePacket.Damage = -healdamage;
            changePacket.AttackerId = healer.Id;

            Room.Broadcast(CellPos, changePacket);

        }


        // 몬스터임
        public virtual void OnDead(GameObject attacker, int damage)
        {
            if (Room == null)
                return;



            // 우선 죽음 패킷 보내고
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            diePacket.Damage = damage;
            Room.Broadcast(CellPos, diePacket);


            // Room 임시저장
            GameRoom room = Room;

            // 방에서 나가게한다.
            //room.LeaveGame(Id);
            //room.Push(room.LeaveGame,Id); // => JobQueue 화 : 바로 실행되지 않을 수 있어 문제가 있음. -> 처리를해줘야함. 처리를해줌

            room.LeaveGame(Id); // 따라서 바로 실행시켜준다.
            //room.PushAfter(3 * 1000, room.LeaveGame, Id);



            // 초기화를 시킨다
            Stat.Hp = Stat.MaxHp;
            Stat.Mp = Stat.MaxMp;
            PosInfo.State = CreatureState.Idle;

            PosInfo.MoveDir = MoveDir.Down;
            //PosInfo.PosX = 0;
            //PosInfo.PosY = 0;


            //// 20초 뒤에 자기 자신을 소환한한다.
            //room.PushAfter(20 * 1000, room.EnterGame, this, true);



            //room.EnterGame(this);
            //room.Push(room.EnterGame,this); // => JobQueue 화// => JobQueue 화 : 바로 실행되지 않을 수 있어 문제가 있음. -> 처리를해줘야함. 처리를해줌

            // 즉시
            //room.EnterGame(this, randomPos: true); // 따라서 바로 실행시켜준다.



            //GameRoom A = GameLogic.Instance.Find(4);
            //A.EnterGame(this, randomPos: true); // 따라서 바로 실행시켜준다.

        }

        // Onwer를 리턴
        public virtual GameObject GetOwner()
        {
            return this;
        }





    }
}
