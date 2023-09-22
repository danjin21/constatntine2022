using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class CreatureController : BaseController
{

    public HpBar _hpBar;
    MpBar _mpBar;

    public GameObject Target { get; set;  }
    public GameObject DamagePocket;

    protected Coroutine _coSkill;
    public Coroutine _coDead;


    public bool _getShot = false;
    public bool _getDie = false;

    public bool getShot
    {
        get { return _getShot; }
        set
        {
            _getShot = value;

            if(_getShot == true)
                _hpBar.transform.gameObject.SetActive(true);
        }
    }

    public bool getDie
    {
        get { return _getDie; }
        set
        {
            _getDie = value;

        }
    }


    // BaseController를 따르되, UpdateHpBar()를 추가로 실행한다.
    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            base.Stat = value;
            UpdateHpBar();
        }
    }

    // BaseController를 따르되, UpdateHpBar()를 추가로 실행한다.
    public override int Hp
    {
        get { return Stat.Hp; }
        set
        {
            base.Hp = value;
            UpdateHpBar();
        }
    }

    public override int Mp
    {
        get { return Stat.Mp; }
        set
        {
            base.Mp = value;
            UpdateHpBar();
        }
    }


    // BaseController를 따르되, UpdateHpBar()를 추가로 실행한다.
    public override int MaxHp
    {
        get { return Stat.MaxHp; }
        set
        {
            base.MaxHp = value;
            UpdateHpBar();
        }
    }

    public override int MaxMp
    {
        get { return Stat.MaxMp; }
        set
        {
            base.MaxMp = value;
            UpdateHpBar();
        }
    }




    public virtual int TotalStr { get { return 0; } }
    public virtual int TotalDex { get { return 0; } }
    public virtual int TotalInt { get { return 0; } }
    public virtual int TotalLuk { get { return 0; } }
    public virtual int TotalHp { get { return Stat.MaxHp; } }
    public virtual int TotalMp { get { return Stat.MaxMp; } }
    public virtual float TotalSpeed { get { return Stat.Speed; } }

    //[SerializeField]
    //public float _speed;


    // HpBar 생성
    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 40.0f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
        
        // 닫아놓자
        _hpBar.transform.gameObject.SetActive(false);
    }

    // HpBar 업데이트
    public virtual void UpdateHpBar()
    {
        if (_hpBar == null)
            return;

        float ratio = 0.0f;

        if(Stat.MaxHp >0)
        {
            // 3 / 2 = 1
            ratio = ( (float)Hp / Stat.MaxHp);
        }

        _hpBar.SetHpBar(ratio);

    }

    // BaseController를 따르되, AddHpBar만 추가로 해준다.
    protected override void Init()
    {
        base.Init();
        AddHpBar();

        DamagePocket = transform.Find("DamagePocket").gameObject;

    }


    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
    }

    public virtual void OnDamaged(int damage, int skillId, List<DamageInfo> DamageList, int attackerId)
    {

        // 내가 당하는거니까 이건 없어도 됨
        //if (Target == null)
        //    return;

        
        Data.Skill skillData = null;
        Managers.Data.SkillDict.TryGetValue(skillId, out skillData);

        float projectileSpeed = 0;

        //if (skillData.projectile != null)
        //    projectileSpeed = skillData.projectile.speed;




        _coSkill = StartCoroutine(CoStartDamageDelay(damage, skillId, DamageList, attackerId, projectileSpeed));

        //if (Hp != 0)
        //     effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword",transform);
        //else
        //     effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword");



    }



    IEnumerator CoStartDamageDelay(int damage, int skillId, List<DamageInfo> DamageList, int attackerId, float projectileSpeed)
    {

        Debug.Log("MY ID 1 : " + Id);
        //if (projectileSpeed > 0)
        yield return new WaitUntil(() => getShot == true);

        getDie = true;

        Debug.Log("MY ID 2: " + Id);

        if (damage >0)
            HitEffect(skillId);
        else
        {
            if(skillId == 3101002) // 힐이라면
            {
                HitEffect(skillId);
            }
        }

        // 포켓 만들기
        GameObject DamagePocketPrefab = Managers.Resource.Instantiate("Effect/DamagePocket", this.transform);
        // DamagePocketPrefab.transform.SetParent(this.transform);
        DamagePocketPrefab.transform.position = DamagePocket.transform.position;

        // 혹시 몰라서 3 초 뒤에는 사라지게
        Managers.Resource.Destroy(DamagePocketPrefab, 3.0f);


        for (int i = 0; i < DamageList.Count; i++)
        {
            DamageText(DamageList[i], skillId, attackerId, DamagePocketPrefab);

            if (this.GetType() == typeof(MonsterController))
            {
                string SoundPath; 

                // 몬스터는 효과음을 준다.
                MonsterController A = this as MonsterController;
                SoundPath = A.HitSoundPath;


                if (skillId == 1001001) // 삼격이라면 
                {
                    SoundPath = "Sounds/Hit/3";
                }

                Managers.Sound.Play(SoundPath, Define.Sound.Effect);

            }
            else
            {
                string SoundPath;

                if (skillId == 1001001) // 삼격이라면 
                {
                    SoundPath = "Sounds/Hit/3";
                    Managers.Sound.Play(SoundPath, Define.Sound.Effect);
                }

         
            }

            // 시간딜레이
            yield return new WaitForSeconds(0.1f);
        }

   

        _coSkill = null;

        getShot = false;
        getDie = false;
    }



    public void DamageText( DamageInfo damageInfo, int skillId, int attackerId, GameObject DamagePocketPrefab)
    {
        GameObject Attacker = Managers.Object.FindById(attackerId);

        if(Attacker == null)
            return;


        GameObject hudText = Managers.Resource.Instantiate("Effect/DamageText");

        if (damageInfo.Damage > 0)
        {
            Color color;

            if (Attacker.GetComponent<CreatureController>().GetType() == typeof(MonsterController))
                ColorUtility.TryParseHtmlString("#ffb9b9", out color);
            else
            {
                ColorUtility.TryParseHtmlString("#FFF820", out color);
                //ColorUtility.TryParseHtmlString("#6620FF", out color);     


            }


            hudText.GetComponent<DmgText>().damage = damageInfo.Damage.ToString();

            if (damageInfo.Kind == 2)
            {
                //ColorUtility.TryParseHtmlString("#FF00B4", out color);
                // f6a8ff
                ColorUtility.TryParseHtmlString("#ffc820", out color);
                hudText.transform.GetChild(0).GetComponent<Text>().fontSize = 13;
                //hudText.GetComponent<DmgText>().damage = "★" + damageInfo.Damage.ToString() + "　";
                hudText.GetComponent<DmgText>().damage = damageInfo.Damage.ToString();

            }


            hudText.transform.GetChild(0).GetComponent<Text>().color = color;

            // hudText.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            Canvas HudCanvas = hudText.GetComponent<Canvas>();



            //hudText.transform.parent = DamagePocket.transform;
            hudText.transform.SetParent(DamagePocketPrefab.transform);
            hudText.transform.SetAsFirstSibling();
            HudCanvas.sortingOrder = DamagePocketPrefab.transform.childCount;

            //HitEffect(skillId);



        }
        else if (damageInfo.Damage == 0)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#36F59C", out color);
            hudText.transform.GetChild(0).GetComponent<Text>().color = color;

            hudText.GetComponent<DmgText>().damage = "Miss";

            //hudText.transform.parent = DamagePocket.transform;
            hudText.transform.SetParent(DamagePocketPrefab.transform);
            hudText.transform.SetAsFirstSibling();

            //HitEffect(skillId);

        }
        else if (damageInfo.Damage < 0)
        {

            Color color;
            ColorUtility.TryParseHtmlString("#20FFAB", out color);
            hudText.transform.GetChild(0).GetComponent<Text>().color = color;

            int a = Mathf.Abs(damageInfo.Damage);
            hudText.GetComponent<DmgText>().damage = "+ " + a.ToString();

            //hudText.transform.parent = this.transform;
            hudText.transform.SetParent(DamagePocketPrefab.transform);
            hudText.transform.SetAsFirstSibling();

        }

        hudText.transform.position = transform.position + new Vector3(0, 25f, -20);
    }




    public void HitEffect(int skillId)
    {
        Vector3 CurrentPosition = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);



        switch (skillId)
        {
            case 3111002:
                {
                    GameObject effect;

                    effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword_3");
                    effect.transform.position = transform.position + new Vector3(0f, 12f, -20);
                    //effect.transform.position = CurrentPosition + new Vector3(6f, 6f, -20);
                    effect.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                    effect.GetComponent<Animator>().Play("Thunderbolt");
                    effect.transform.parent = this.transform;
                    // 게임 이펙트를 몇초 후에 삭제
                    GameObject.Destroy(effect, 0.3f);

                    //GameObject effect = Managers.Resource.Instantiate("Effect/Skill/310/3101000", this.transform);
                    //effect.transform.position = transform.position + new Vector3(0, 0, -9);
                    //effect.transform.GetChild(0).position += new Vector3(0, -15, -9);
                    //effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
                    ////effect.transform.SetParent(this.transform);
                    ////effect.transform.parent = this.transform;
                    //GameObject.Destroy(effect, 5.0f);

                    break;
                }
            case 1001001:
            case 1001000:
                {
                    GameObject effect;

                    //effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword_4");
                    effect = Managers.Resource.Instantiate("Effect/TigerGlaw");
                    //effect.transform.position = transform.position + new Vector3(6f, 2f, -20);
                    effect.transform.position = transform.position + new Vector3(-6f, 6f, -20);
                    effect.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                    effect.GetComponent<Animator>().Play("PowerStrike");
                    effect.transform.parent = this.transform;
                    // 게임 이펙트를 몇초 후에 삭제
                    //GameObject.Destroy(effect, 0.3f);
                    GameObject.Destroy(effect, 1.0f);

                    //GameObject effect = Managers.Resource.Instantiate("Effect/Skill/310/3101000", this.transform);
                    //effect.transform.position = transform.position + new Vector3(0, 0, -9);
                    //effect.transform.GetChild(0).position += new Vector3(0, -15, -9);
                    //effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
                    ////effect.transform.SetParent(this.transform);
                    ////effect.transform.parent = this.transform;
                    //GameObject.Destroy(effect, 5.0f);

                    break;
                }
            case 3101002:
                {
                    GameObject effect = Managers.Resource.Instantiate("Effect/Skill/311/" + skillId, this.transform);
                    effect.transform.position = transform.position + new Vector3(0, 0, -9);
                    effect.transform.GetChild(0).position += new Vector3(0, 0, -9);
                    //effect.transform.position = CurrentPosition + new Vector3(6f, 6f, -20);
                    effect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = _sprite.sortingOrder;
                    effect.transform.parent = this.transform;
                    // 게임 이펙트를 몇초 후에 삭제
                    GameObject.Destroy(effect, 4.0f);



                    break;
                }
            default:
                {
                    GameObject effect;

                    effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword");

                    effect.transform.position = transform.position + new Vector3(6f, 6f, -20);
                    //effect.transform.position = CurrentPosition + new Vector3(6f, 6f, -20);
                    effect.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                    effect.GetComponent<Animator>().Play("Hit_Sword");
                    effect.transform.parent = this.transform;
                    // 게임 이펙트를 몇초 후에 삭제
                    GameObject.Destroy(effect, 0.3f);

                    break;
                }
              
        }




    }








    public virtual void OnDead(int damage)
    {


  

        _coDead = StartCoroutine(CoStartDeadDelay(damage));

   
    }
    IEnumerator CoStartDeadDelay(int damage)
    {

        float deleteEffectTime = 0.5f;

        //if (projectileSpeed > 0)
        yield return new WaitUntil(() => getDie == true);


        State = CreatureState.Dead;
        Vector3 CurrentPosition = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);


        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position + new Vector3(0, 0, -1);
        //effect.transform.position = CurrentPosition + new Vector3(0, 0, -1);
        effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        effect.GetComponent<Animator>().Play("START");
        effect.transform.parent = this.transform;

        // 게임 이펙트를 몇초 후에 삭제
        GameObject.Destroy(effect, deleteEffectTime);

        // 혹시모르니 한번은 실행되도록
        UpdateDead();

        getDie = false;

        yield return new WaitForSeconds(deleteEffectTime);


        // Hp바 비활성화

        _hpBar.transform.gameObject.SetActive(false);

        _coDead = null;

    }

    public virtual void UseSkill(int skillId)
    {

    }


    public void OnHit()
    {
        GameObject effect = null;

        //Debug.Log($"Dir = {Dir}");

        //if (Dir == MoveDir.Left)
        //{
        //    effect = Managers.Resource.Instantiate("Effect/AttackEffect_Left", transform);
        //    effect.transform.position = transform.position + new Vector3(-16f, 0, -1);
        //    effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        //    Debug.Log("??1");


        //}
        //else if (Dir == MoveDir.Right)
        //{
        //    effect = Managers.Resource.Instantiate("Effect/AttackEffect_Right", transform);
        //    effect.transform.position = transform.position + new Vector3(16f, 0, -1);
        //    effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        //    Debug.Log("??");
        //}
        //else if (Dir == MoveDir.Down)
        //{
        //    effect = Managers.Resource.Instantiate("Effect/AttackEffect_Down", transform);
        //    effect.transform.position = transform.position + new Vector3(-4f, 0, -1);
        //    effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        //    Debug.Log("??");
        //}
        //else if (Dir == MoveDir.Up)
        //{
        //    effect = Managers.Resource.Instantiate("Effect/AttackEffect_Up", transform);
        //    effect.transform.position = transform.position + new Vector3(2f, 18f, 1);
        //    effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        //    Debug.Log("??");
        //}
        // 게임 이펙트를 몇초 후에 삭제
        GameObject.Destroy(effect, 0.2f);


    }

}
