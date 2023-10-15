using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{

    public MonsterController mc = new MonsterController();

    int kind;

    // Start is called before the first frame update
    void Start()
    {
        mc = GetComponent<MonsterController>();

        kind = mc.Stat.TemplateId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //========================================//

    public void Walk_Down_1()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][0];
        Debug.Log("바뀌고 있냐?" + Managers.Anim.MobSprites[kind - 1][0]);
        Debug.Log("응" + GetComponent<SpriteRenderer>().sprite);
    }

    public void Walk_Down_2()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][1];
        Debug.Log("바뀌고 있냐?" + Managers.Anim.MobSprites[kind - 1][1]);
        Debug.Log("응" + GetComponent<SpriteRenderer>().sprite);
    }

    public void Walk_Down_3()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][2];
        Debug.Log("바뀌고 있냐?" + Managers.Anim.MobSprites[kind - 1][2]);
        Debug.Log("응" + GetComponent<SpriteRenderer>().sprite);
    }

    //========================================//

    public void Walk_Up_1()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][9];
    }

    public void Walk_Up_2()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][10];
    }

    public void Walk_Up_3()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][11];

    }

    //========================================//

    public void Walk_Right_1()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][6];

    }

    public void Walk_Right_2()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][7];

    }

    public void Walk_Right_3()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][8];
    }

    //========================================//

    public void Walk_Left_1()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][3];
    }

    public void Walk_Left_2()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][4];
    }

    public void Walk_Left_3()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][5];
    }

    //========================================//

    public void Die()
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Anim.MobSprites[kind - 1][12];
    }





}
