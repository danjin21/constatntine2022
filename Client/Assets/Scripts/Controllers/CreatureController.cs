using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class CreatureController : BaseController
{

    HpBar _hpBar;
    MpBar _mpBar;

    public GameObject Target { get; set;  }
    public GameObject DamagePocket;

    protected Coroutine _coSkill;

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

    public virtual void OnDamaged(int damage, int skillId, List<int> DamageList)
    {

        // 내가 당하는거니까 이건 없어도 됨
        //if (Target == null)
        //    return;


        _coSkill = StartCoroutine(CoStartDamageDelay(damage, skillId, DamageList));

        //if (Hp != 0)
        //     effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword",transform);
        //else
        //     effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword");



    }


    IEnumerator CoStartDamageDelay(int damage, int skillId, List<int> DamageList)
    {
        if(damage >0)
            HitEffect(skillId);

        for (int i = 0; i < DamageList.Count; i++)
        {
            DamageText(DamageList[i], skillId);

            if (this.GetType() == typeof(MonsterController))
            {
                // 몬스터는 효과음을 준다.
                MonsterController A = this as MonsterController;
                Managers.Sound.Play(A.HitSoundPath, Define.Sound.Effect);
            }

            // 시간딜레이
            yield return new WaitForSeconds(0.10f);
        }

        _coSkill = null;

    }


    public void DamageText( int damage, int skillId)
    {
        GameObject hudText = Managers.Resource.Instantiate("Effect/DamageText");

        if (damage > 0)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#FFF820", out color);
            hudText.transform.GetChild(0).GetComponent<Text>().color = color;

            hudText.GetComponent<DmgText>().damage = damage.ToString();


            //hudText.transform.parent = DamagePocket.transform;
            hudText.transform.SetParent(DamagePocket.transform);

            //HitEffect(skillId);

        }
        else if (damage == 0)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#36F59C", out color);
            hudText.transform.GetChild(0).GetComponent<Text>().color = color;

            hudText.GetComponent<DmgText>().damage = "Miss";

            //hudText.transform.parent = DamagePocket.transform;
            hudText.transform.SetParent(DamagePocket.transform);

            //HitEffect(skillId);

        }
        else if (damage < 0)
        {

            Color color;
            ColorUtility.TryParseHtmlString("#20FFAB", out color);
            hudText.transform.GetChild(0).GetComponent<Text>().color = color;

            int a = Mathf.Abs(damage);
            hudText.GetComponent<DmgText>().damage = "+ " + a.ToString();

            //hudText.transform.parent = this.transform;
            hudText.transform.SetParent(DamagePocket.transform);

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

                    effect = Managers.Resource.Instantiate("Effect/Hit_Effect_Sword_4");
                    effect.transform.position = transform.position + new Vector3(6f, 2f, -20);
                    effect.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                    effect.GetComponent<Animator>().Play("PowerStrike");
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
        State = CreatureState.Dead;

        Vector3 CurrentPosition = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);


        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position + new Vector3(0, 0, -1);
        //effect.transform.position = CurrentPosition + new Vector3(0, 0, -1);
        effect.GetComponent<SpriteRenderer>().sortingOrder = _sprite.sortingOrder;
        effect.GetComponent<Animator>().Play("START");
        effect.transform.parent = this.transform;

        // 게임 이펙트를 몇초 후에 삭제
        GameObject.Destroy(effect, 0.5f);

        // Hp바 비활성화

        _hpBar.transform.gameObject.SetActive(false);
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
