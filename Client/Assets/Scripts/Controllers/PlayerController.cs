using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using UnityEngine.UI;

public class PlayerController : CreatureController
{




    protected Coroutine _coSkill;
    public int _skillId = -1;

    protected override void Init()
    {
        base.Init();
        // 소환될때 바라보는 방향 갱신        
        AddName();

    }



    // HpBar 생성
    protected void AddName()
    {
        Text Name = transform.Find("Canvas/NameBox/NameText").GetComponent<Text>();
        Name.text = transform.name;
    }

    public void ChildAllFlipX(bool flip)
    {


        // 계속해서 flip 하는것 방지 처음 바뀔때만 한번 바뀌게 함
        if (transform.GetChild(2).GetComponent<SpriteRenderer>().flipX == flip )
            return;


        // 모든 애들을 플립한다.
        foreach(Transform child in transform)
        {

            if(child.GetComponent<SpriteRenderer>() !=null)
            child.GetComponent<SpriteRenderer>().flipX = flip;

        }
    }


    public void ChildAllFlipX_Right(bool flip)
    {


        // 계속해서 flip 하는것 방지 처음 바뀔때만 한번 바뀌게 함
        if (transform.GetChild(2).GetComponent<SpriteRenderer>().flipX == flip)
            return;

        int i = 0;

        // 1과 5 ( 몸과 옷만 빼고 flip을 한다.)
        foreach (Transform child in transform)
        {

            if(i != 1 && i != 5 && i !=6 && i!=7 ) // 바디,아머,무기,헬멧
            {
                if (child.GetComponent<SpriteRenderer>() != null)
                    child.GetComponent<SpriteRenderer>().flipX = flip;
            }

            i++;

        }
    }

    protected override void UpdateIdle()
    {

        base.UpdateIdle();

       

    }

    protected override void UpdateAnimation()
    {
        if (_animator == null || _sprite ==null)
            return;


        if (State == CreatureState.Idle )
        {
            switch (Dir)
            {


                case MoveDir.Up:
                    //ChildAllFlipX(false);
                    _animator.Play("Idle_Up");
                    //_sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    //ChildAllFlipX(false);
                    _animator.Play("Idle_Down");
                    //_sprite.flipX = false;
                    break;


                case MoveDir.Left:
                    //ChildAllFlipX(false);
                    _animator.Play("Idle_Left");
                    //_sprite.flipX = false;
                    break;


                case MoveDir.Right:
                    //ChildAllFlipX_Right(true);
                    //ChildAllFlipX(false);
                    _animator.Play("Idle_Right");
                    //_sprite.flipX = true;
                    break;
            }

        }
        else if (State == CreatureState.Moving  )
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    //ChildAllFlipX(false);
                    _animator.Play("Walk_Up");
                    //_sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    //ChildAllFlipX(false);
                    _animator.Play("Walk_Down");
                    //_sprite.flipX = false; 
                    break;
                case MoveDir.Left:
                    //ChildAllFlipX(false);
                    _animator.Play("Walk_Left");
                    //_sprite.flipX = false;
                    break;
                case MoveDir.Right:

                    //ChildAllFlipX_Right(true);
                    //ChildAllFlipX(false);
                    _animator.Play("Walk_Right");
                    //_sprite.flipX = true;
                    break;

            }

        }
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    //ChildAllFlipX(false);
                    SkillAnimation("Up");
                    //_sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    //ChildAllFlipX(false);
                    SkillAnimation("Down");
                    //_sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    //ChildAllFlipX(false);
                    SkillAnimation("Left");
                    //_sprite.flipX = false;
                    break;
                case MoveDir.Right:

                    //// 그냥 남 플레이어는 계속 update 하므로 몸과 옷은 flip 안하는 함수로 진행
                    //if (this.GetType() == typeof(PlayerController))
                    //    ChildAllFlipX_Right(true);
                    //else
                    //    ChildAllFlipX(true);

                    //ChildAllFlipX_Right(true);
                    //ChildAllFlipX(false);

                    SkillAnimation("Right");
                    //_sprite.flipX = false;   /*_sprite.flipX = true;*/
                    break;

            }
        }
        else if (State == CreatureState.Dead)
        {
            // 어차피 죽으면 바로 사라짐.
            //_animator.Play("DIE");
            //_sprite.flipX = false;
        }
        else
        {

        }
    }


    public void SkillAnimation(string A)
    {
        if (_skillId == -1)
            return;

        switch (_skillId)
        {

            case 9001000: // 기본공격
            case 1001000: // 이격
            case 1001001: // 삼격
                _animator.Play("Attack_" + $"{A}");
                break;
            case 2001000: // 더블샷
                _animator.Play("Attack_Weapon_"+$"{A}");
                break;
            case 3111002: // 썬더볼트
            case 3101002: // 힐
                _animator.Play("Spell_" + $"{A}");
                break;
            case 9001001: // 줍기
                _animator.Play("Drop_" + $"{A}");
                break;
            case 3101000: // 텔레포트

                //_animator.Play("Spell_" + $"{A}");

                break;
        }

        _skillId = -1;
    }




    protected override void UpdateController()
    {
        base.UpdateController();

        // 진형 추가 // Player일 경우에만 애니메이션은 바로 업데이트해준다.
        if (this.GetType() == typeof(PlayerController))
            UpdateAnimation();
    }


    public void LevelUp()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/LevelUp/LevelUp_Effect");
        effect.transform.position = transform.position + new Vector3(0, 0, -9);
        effect.transform.GetChild(0).position += new Vector3(0, 0, -9);
        effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
        effect.transform.SetParent(this.transform);

        // 게임 이펙트를 몇초 후에 삭제
        GameObject.Destroy(effect, 4.0f);

        // 레벨업 텍스트

        GameObject hudText = Managers.Resource.Instantiate("Effect/LevelUpText");

        Color color;
        ColorUtility.TryParseHtmlString("#FFFFFF", out color);
        hudText.transform.GetChild(0).GetComponent<Text>().color = color;

        hudText.GetComponent<LevelUpText>().LevelUpMessage = "Level Up";
        hudText.transform.position = transform.position + new Vector3(0, 30, 0);
        hudText.transform.SetParent(transform);

    }


    public override void UseSkill(int skillId)
    {
        // 스킬아이디 갱신
        _skillId = skillId;

        if (State == CreatureState.Moving)
        {
            // MyPlayer도 해버리면 스킬 모션 씹어버림
            if (this.GetType() == typeof(PlayerController))
                return;
        }


        Data.Skill skillData = null;
        Managers.Data.SkillDict.TryGetValue(skillId, out skillData);

        if (skillId == 9001000 || skillId == 1001001 || skillId == 1001000 )
        {

            Managers.Sound.Play("Sounds/Skill/9001000", Define.Sound.Effect);
            _coSkill = StartCoroutine("CoStartPunch");

            //if( skillId == 1001001 || skillId == 1001000)
            //{

            //    GameObject effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword_3");
            //    effect.transform.position = transform.position + new Vector3(0, 0, -9);
            //    //effect.transform.GetChild(0).position += new Vector3(0, 0, -9);
            //    //effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
            //    effect.transform.SetParent(this.transform);

            //    // 게임 이펙트를 몇초 후에 삭제
            //    GameObject.Destroy(effect, 4.0f);
            //}


        }
        else if(skillId == 2001000)
        {
            _coSkill = StartCoroutine("CoStartShootArrow");

            Managers.Sound.Play(skillData.soundPath, Define.Sound.Effect);

            Vector3 CurrentPosition = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);


            GameObject effect = Managers.Resource.Instantiate("Effect/Skill/400/4001000");
            effect.transform.position = transform.position + new Vector3(0, 0, -9);
            effect.transform.GetChild(0).position += new Vector3(0, 0, -9);
            effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
            effect.transform.SetParent(this.transform);

            // 게임 이펙트를 몇초 후에 삭제
            GameObject.Destroy(effect, 4.0f);


        }
        else if(skillId == 3101002 || skillId == 3111002)
        {
            _coSkill = StartCoroutine("CoStartHeal");


            Managers.Sound.Play(skillData.soundPath, Define.Sound.Effect);

            Vector3 CurrentPosition = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);


            GameObject effect = Managers.Resource.Instantiate("Effect/Skill/311/"+ skillId , this.transform);
            effect.transform.position = transform.position + new Vector3(0, 0, -9);
            effect.transform.GetChild(0).position += new Vector3(0, 0, -9);
            //effect.transform.position = CurrentPosition + new Vector3(0, 0, -1);
            //effect.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;

            effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;

            //effect.transform.GetChild(0).GetComponent<Animator>().Play("START");
            //effect.transform.SetParent(this.transform);

            // 게임 이펙트를 몇초 후에 삭제
            GameObject.Destroy(effect, 4.0f);

        }
        else if (skillId == 3101000)
        {
            _coSkill = StartCoroutine("CoStartSkill");

            Managers.Sound.Play(skillData.soundPath, Define.Sound.Effect);

            GameObject effect = Managers.Resource.Instantiate("Effect/Skill/310/3101000", this.transform);
            effect.transform.position = transform.position + new Vector3(0, 0, -9);
            effect.transform.GetChild(0).position += new Vector3(0, -15, -9);
            effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
            //effect.transform.SetParent(this.transform);
            //effect.transform.parent = this.transform;
            GameObject.Destroy(effect, 5.0f);
        }
        else if (skillId == 9001001)
        {
            // 줍기
            _coSkill = StartCoroutine("CoStartDrop");

            Managers.Sound.Play(skillData.soundPath, Define.Sound.Effect);
        }


        UpdateAnimation();

        if (skillId != 3101000)
            Managers.Object.MyPlayer.SkillCool();



  
    }



    protected virtual void CheckUpdatedFlag()
    {
        // 실제로는 MyPlayerController의  CheckUpdateFlag만 작동한다.
    }


    // 언젠가는 시간 확인하는걸 서버에서 하는걸로 바꿔줘야한다.

    IEnumerator CoStartPunch()
    {

        // 피격 판정 (서버에서 한다)
        //GameObject go = Managers.Object.Find(GetFrontCellPos());
        //if(go != null)
        //{
        //    CreatureController cc = go.GetComponent<CreatureController>();
        //    if (cc != null)
        //    {
        //        cc.OnDamaged();
        //    }
        //}

        // 대기 시간

        State = CreatureState.Skill;

        yield return new WaitForSeconds(0.5f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.

        Debug.Log("OK! GO !");
        State = CreatureState.Idle;
        _coSkill = null;

        CheckUpdatedFlag(); // 나의 캐릭터 State 상태를 여기서 서버에 보내준다.


    }

    IEnumerator CoStartShootArrow()
    {
        //서버에서 처리
        //GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        //ArrowController ac = go.GetComponent<ArrowController>();
        //ac.Dir = Dir;
        //ac.CellPos = CellPos;

        // 대기 시간
        State = CreatureState.Skill;
        //yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;

        CheckUpdatedFlag(); // 나의 캐릭터 State 상태를 여기서 서버에 보내준다.
    }


    IEnumerator CoStartHeal()
    {

        // 대기 시간

        State = CreatureState.Skill;

        yield return new WaitForSeconds(0.5f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.

        Debug.Log("OK! GO !");
        State = CreatureState.Idle;
        _coSkill = null;

        CheckUpdatedFlag(); // 나의 캐릭터 State 상태를 여기서 서버에 보내준다.
    }



    IEnumerator CoStartDrop()
    {

        // 대기 시간

        State = CreatureState.Skill;

        //yield return new WaitForSeconds(0.5f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.
        yield return new WaitForSeconds(0.5f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.


        Debug.Log("OK! GO !");
        State = CreatureState.Idle;
        _coSkill = null;

        CheckUpdatedFlag(); // 나의 캐릭터 State 상태를 여기서 서버에 보내준다.
    }


    IEnumerator CoStartSkill()
    {

        // 대기 시간

        State = CreatureState.Skill;

        //yield return new WaitForSeconds(0.5f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.
        yield return new WaitForSeconds(0.0f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.


        Debug.Log("OK! GO !");

        ////// 진형 추가 // MyPlayer일 경우에만 애니메이션은 바로 업데이트해준다.
        //if (transform.GetComponent<MyPlayerController>() != null)
        //{
        //    if (transform.GetComponent<MyPlayerController>()._moveKeyPressed == true)
        //    {
        //        State = CreatureState.Moving;
        //        State = CreatureState.Idle;
        //    }
        //    else
        //        State = CreatureState.Idle;
        //}
        //else
            //State = CreatureState.Idle;
        //else
        //{
        //    State = CreatureState.Moving;
        //}

        State = CreatureState.Idle;

        _coSkill = null;

        CheckUpdatedFlag(); // 나의 캐릭터 State 상태를 여기서 서버에 보내준다.
    }

    public override void OnDamaged(int damage, int skillId, List<int> DamageList, int attackerId)
    {
        base.OnDamaged(damage,skillId, DamageList, attackerId);
        Debug.Log("Player Hit !");
    }


    public override void OnDead(int damage)
    {
        base.OnDead(damage);
    }




















}
