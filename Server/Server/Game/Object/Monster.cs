using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public int TemplateId { get; private set; }

        public bool _isPatrol = false;

        // 생성자
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

         
        }


        public void Init(int templateId)
        {

            TemplateId = templateId;

            // TemplateId에서 해당 몬스터의 데이터를 가지고 온다.
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);
            
            // 데이터 메모장에 있는 정보로 넣어준다.
            Stat.MergeFrom(monsterData.stat);

            // 다만 체력은 최대 체력과 같게 따로 설정해준다.
            Stat.Hp = monsterData.stat.MaxHp;
            Stat.Mp = monsterData.stat.MaxMp;
            Stat.Exp = monsterData.stat.Exp;

            // 타겟도 초기화 시켜준다.
            _target = null;

            // 
            _isPatrol = false;

            //temp
            //Stat.Level = 1;
            //Stat.Hp = 100;
            //Stat.MaxHp = 100;
            //Stat.Speed = 50.0f;

            State = CreatureState.Idle;

            updateTime = 200;
        }


        // 가장 좋아하는 인공지능 (인디에서 많이 씀 )
        // FSM (Finite State Machine)

        IJob _job;

        int updateTime = 200;

        public override void Update()
        {

            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    {
                        if (!_isPatrol)
                            UpdateMoving();
                        else
                            UpdatePatrol();
                    }
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;

            }
            // 5프레임 ( 0.2초마다 한번씩 Update)

            if (Room != null)
                _job = Room.PushAfter(updateTime, Update);

        }

        Player _target; // 나중에 ID로 하는게 좋을듯.
        bool IsProjectile = false; // 뭐로 공격받았는지.

        // 사정거리
        int _searchCellDist = 10;
        int _chaseCellDist = 20;

        long _nextSearchTick = 0;



        // Idle : 매 1초마다 서칭해서 걸리면 이동되게 하는거
        protected virtual void UpdateIdle()
        {

 

            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 3000; // 0.2초가 지났으면 0.2초더 증가시켜준다.

            // 이미 타겟이 있는 경우에는 거리를 보지 않는다.

            if (_target == null)
            {
                Player target = Room.FindClosestPlayer(CellPos, _searchCellDist);

                if (target == null)
                {
                    State = CreatureState.Moving;
                    _isPatrol = true;
                    return;
                }

                _target = target;
                State = CreatureState.Moving;
                _isPatrol = false;
            }
            else if (_target != null)
            {
                State = CreatureState.Moving;
                _isPatrol = false;
            }

            

        }

        int _skillRange = 1;
        //public long _nextMoveTick = 0;

        public long _moveDelayTick = 0;

        protected virtual void UpdateMoving()
        {

            if (_nextMoveTick > Environment.TickCount64)
            {
                return;
            }

            int moveTick = (int)(32 * 1000 / Speed);

            // Speed : 1초 동안 몇칸을 움직이냐는 개념
            // moveTick = 작을수록 빠른거임. 



            //Console.WriteLine($"2- 이동중인 진행률 = " + _nextMoveTick + "/" + Environment.TickCount64 + "/moveTick = " + moveTick);
            // 내가 쫓고 있는 플레이어가 없거나 다른방으로 갈 때.
            if (_target == null || _target.Room != Room)
            {

                 _target = null;

                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;

            // 거리가 너무 멀어졌을때
            int dist = dir.cellIdistFromZero;

            if (dist == 0 || dist > _chaseCellDist)
            {
                // 화살 맞은 거라면 멀어도 가게 한다.
                if (IsProjectile)
                    return;

                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 우선 오브젝트 무시하고 길까지 가는걸로 = 그래야 플레이어한테 가니까. => true로 해서 충돌체로
            // 생각하게 하여 피해서 가게 만든다. ( checkObjects => true 로 )

            // 길찾기를 했을때 너무 적거나 너무 멀때
           
            List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObjects: true);

            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 스킬로 넘어갈지 체크
            if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {



                // 스킬의 타겟팅 방향 주시

                MoveDir lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                }

                _coolTick = 0;
                State = CreatureState.Skill;
                BroadcastMove();
                return;
            }

            // 조금 딜레이 주기

            if (_moveDelayTick > Environment.TickCount64 && (int)Stat.Dex > 0)
            {
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }


            _moveDelayTick = Environment.TickCount64 + (int)(Stat.Dex /*몬스터의 움직임 딜레이*/);

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);

            // 다음 위치가 포탈이면 리턴한다.
            if (Room.Map.IsPortal(path[1]) != null)
            {
                return;
            }


            if (Room.Map.ApplyMove(this, path[1]) == true)
            {

                // 이때부터 서버상에서 거리를 계산하게 만든다. 실제로 이동되는 순간
                _nextMoveTick = Environment.TickCount64 + moveTick;

                BroadcastMove();
            }

        }




        protected virtual void UpdatePatrol()
        {


            if (_nextMoveTick > Environment.TickCount64)
            {
                return;
            }

            int moveTick = (int)(32 * 1000 / Speed);

            // Speed : 1초 동안 몇칸을 움직이냐는 개념
            // moveTick = 작을수록 빠른거임. 

            int where = new Random().Next(0, 8);
            
            Vector2Int randPos = new Vector2Int(0,0);

            switch(where)
            {
                case 0:
                    randPos = CellPos + new Vector2Int(0, 1);
                    break;
                case 1:
                    randPos = CellPos + new Vector2Int(0, -1);
                    break;
                case 2:
                    randPos = CellPos + new Vector2Int(1, 0);
                    break;
                case 4:
                    randPos = CellPos + new Vector2Int(-1, 0);
                    break;
                default:
                    randPos = CellPos + new Vector2Int(0, 0);
                    break;
            }


            Vector2Int dir = randPos - CellPos;

            // 거리가 너무 멀어졌을때
            int dist = dir.cellIdistFromZero;

            if (dist == 0 || dist > _chaseCellDist)
            {
                // 화살 맞은 거라면 멀어도 가게 한다.
                if (IsProjectile)
                    return;

                //_target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 우선 오브젝트 무시하고 길까지 가는걸로 = 그래야 플레이어한테 가니까. => true로 해서 충돌체로
            // 생각하게 하여 피해서 가게 만든다. ( checkObjects => true 로 )

            // 길찾기를 했을때 너무 적거나 너무 멀때

            List<Vector2Int> path = Room.Map.FindPath(CellPos, randPos, checkObjects: true);

            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                //_target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }


            // 조금 딜레이 주기

            if (_moveDelayTick > Environment.TickCount64 && (int)Stat.Dex >0)
            {
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            _moveDelayTick = Environment.TickCount64 + (int)(Stat.Dex /*몬스터의 움직임 딜레이*/);

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);

            // 다음 위치가 포탈이면 리턴한다.
            if(Room.Map.IsPortal(path[1]) != null)
            {
                return;
            }

            if (Room.Map.ApplyMove(this, path[1]) == true)
            {

                // 이때부터 서버상에서 거리를 계산하게 만든다. 실제로 이동되는 순간
                _nextMoveTick = Environment.TickCount64 + moveTick;

                BroadcastMove();
            }

        }



        void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다.

            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;


            Room.Broadcast(CellPos, movePacket);
            //Room.Push(Room.Broadcast, CellPos, movePacket);



            //Console.WriteLine($"몬스터의 위치 {PosInfo.PosX}/ {PosInfo.PosY}");

        }

        long _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟인지

                if (_target == null || _target.Room != Room || _target.Hp < 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadcastMove(); // 방향 갱신
                    return;
                }

                // 스킬이 아직 사용 가능한지


                Vector2Int dir = (_target.CellPos - CellPos); // 방향벡터
                int dist = dir.cellIdistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));

                if (canUseSkill == false)
                {

                    //_target = null;   // 무빙상태에서 판단하게 지웠다.
                    State = CreatureState.Moving;
                    BroadcastMove(); // 방향 갱신
                    return;
                }

                // 타겟팅 방향 주시

                MoveDir lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadcastMove();

                    // 리턴은 X
                }

                // 스킬정보를 불러온다.
                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(9001000, out skillData);

                int realDamage = 0;
                // 데미지 판정
                realDamage = _target.OnDamaged(this, skillData.damage + TotalAttack, skillData.id, 1, this);

                // 스킬 사용 Broadcast
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                skill.TargetId = _target.Id;
                skill.Damage = realDamage /*skillData.damage + TotalAttack*/;

                Room.Broadcast(CellPos, skill);
                //Room.Push(Room.Broadcast, CellPos, skill);

                // 스킬 쿨타임 적용

                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;



        }

        protected virtual void UpdateDead()
        {

        }

        public override int OnDamaged(GameObject attacker, int damage, int skillId, int shot = 1, GameObject Owner = null)
        {
            _target = (Player)(attacker.GetOwner());

            // 맞았으니까 IsPatrol 은 지워준다.
            _isPatrol = false;

            // 투사체로 맞은거라면 표시를 해준다.
            if (attacker.ObjectType == GameObjectType.Projectile)
                IsProjectile = true;
            else
                IsProjectile = false;

            return base.OnDamaged(attacker, damage, skillId,shot,Owner);
        }

        public override void OnDead(GameObject attacker, int damage)
        {
            // 지금 죽자마자 룸에 들어오기 때문에, 룸에 들어 오기전에 job 을 캔슬 해줘야,
            // 업데이트 가 계속 쌓이는 것을 방지할 수 있다.

            if (_job != null)
            {
                _job.Cancel = true;
                _job = null;
            }
            

   

            // 아이템 생성
            GameObject owner = attacker.GetOwner();

            if(owner.ObjectType == GameObjectType.Player)  // pet이든 projectile 이든 포함이 될 수도 있다. 그래서 애들한테 owner를 불러오는 식
            {
                RewardData rewardData = GetRandomReward();

                Player player = (Player)owner;

                // 경험치 넣고 디비에 저장해주세요~

                DbTransaction.ExpPlayer(player, Stat.Exp, Room);

                if (rewardData != null)
                {


                    // 원래 바로 보상해주는 부분
                    //// 아이템 넣고 디비에 저장해주세요~
                    //DbTransaction.RewardPlayer(player, rewardData, Room);

                    GameRoom room = Room;

                    if (room == null)
                        return;

                    // 골드의 경우 랜덤으로 지급

                    if (rewardData.itemId == 99999)
                    {

                        int rand = new Random().Next(-10, 11);
                        rewardData.count += rand;
                        //rewardData.count += 10;
                    }

                    // 아이템 드랍

                    DropItem dropItems = ObjectManager.Instance.Add<DropItem>();

                    dropItems.Init(player, rewardData, Room, 0);


                    // 이동중
                    if (State == CreatureState.Moving)
                    {



                        long Dec = _nextMoveTick - Environment.TickCount64;
                        Console.WriteLine("걷는도중의 Dec = " + Dec);

                        int moveTick = (int)(32 * 1000 / Speed);

                        // 반 이상도 못왓다는거임
                        if (Dec >= moveTick / 2.3 && Dec != moveTick /*아직 이동안한 상태*/)
                        {
                            // 만약에 투사체로 공격한 것이 아니라면, 근접공격이기 때문에 무조건 그 위치에 떨궈야 한다.
                            if(attacker.ObjectType != GameObjectType.Projectile)
                            {
                                dropItems.CellPos = new Vector2Int(CellPos.x, CellPos.y);
                            }
                            else // 만약에 투사체로 공격한 거라면, 사실적으로 해주기 위해 그 뒤에 떨구게 한다.
                            {
                                Vector2Int A = GetBackCellPos();
                                dropItems.CellPos = A;
                            }

                        }
                        else if (Dec < moveTick / 2.3) // 반 넘어 왔다는 왔다는거임
                        {
                            dropItems.CellPos = new Vector2Int(CellPos.x, CellPos.y);

                        }
                    }
                    else
                    {
                        dropItems.CellPos = new Vector2Int(CellPos.x, CellPos.y);

                        long Dec = _nextMoveTick - Environment.TickCount64;
                        Console.WriteLine("멈춰있는중의 Dec = " + Dec);
                    }


                    //dropItems.CellPos = new Vector2Int(CellPos.x, CellPos.y);


                    room.Push(room.EnterGame, dropItems, false); // false 는 랜덤값

                    //Console.WriteLine($"아이템 생성 위치 : ({dropItems.CellPos.x},{dropItems.CellPos.y}) 아이템 레이어 {dropItems.Order}");



      




                    // Item.MakeItem();
                    // player.Inven.Add();
                }
            }

            // 타겟을 초기화시켜준다.
            _target = null;

            base.OnDead(attacker, damage);
        }

        RewardData GetRandomReward()
        {
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);

            //int rand = new Random().Next(0, 101);  // 포함 , 포함되지 않는 숫자
            int rand = new Random().Next(1, 101);  // 포함 , 포함되지 않는 숫자
            // 0 부터 하면 1%의 확률로 가능성이 0 인 아이템이 나올 수 있음.

            // random = 0~100 -> 42
            // 10 10 10 10 10
            // 10 20 30 40 50

            int sum = 0;
            
            foreach(RewardData rewardData in monsterData.rewards)
            {
                sum += rewardData.probability;

                if(rand <= sum)
                {
                    // @##### 지금 무슨 값을 매개변수로 줘서 변수가 바뀌는 현상 때문에 그냥 매번 만들어줌.

                    RewardData newData = new RewardData();
                    newData.itemId = rewardData.itemId;
                    newData.count = rewardData.count;
                    newData.probability = rewardData.probability;
                    //newData.itemInfo = rewardData.itemInfo;


                    ItemData itemData = null;
                    DataManager.ItemDict.TryGetValue(newData.itemId, out itemData);


                    // 에러 수정 이거 뭐지..
                    if (itemData == null)
                        return null; 


                    Console.WriteLine($"야야야야야/{newData.itemId}");


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
                    else if(itemData.itemType == ItemType.Consumable)
                    {
                        // 그냥 사용할때마다 Data 접근해서 쓰게하자
                        //newData.itemInfo.Enhance = itemData.Enhance;
                        //newData.itemInfo.Hp = itemData.Hp;
                        //newData.itemInfo.Mp = itemData.Mp;
                    }


                    //return rewardData;
                    return newData;
                }
            }

            return null;

        }


        public void DropItem()
        {

        }

    }
}
