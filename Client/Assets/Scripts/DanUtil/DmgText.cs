using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DmgText : MonoBehaviour
{

    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    Text text;
    Color alpha;
    Color alpha_2;
    Outline outline;
    Shadow shadow;

    public string damage;

    public bool isFade;

    // Start is called before the first frame update
    void Start()
    {
        text = transform.GetChild(0).GetComponent<Text>();

        alpha = text.color;
        outline = transform.GetChild(0).GetComponent<Outline>();
        shadow = transform.GetChild(0).GetComponent<Shadow>();

        text.text = damage;

        moveSpeed = 10; //10
        //alphaSpeed = 2.0f;
        alphaSpeed = 1.0f;

        destroyTime = 2.5f;/*1.5f;*/

        isFade = false;

        Invoke("FadeIn", 0.0f);
        Invoke("DestroyObject", destroyTime);

        Invoke("GetFree", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {


        //outline.effectDistance = new Vector2(alpha.a * 0.1f, -alpha.a * 0.1f);

        //shadow.effectDistance = new Vector2(alpha.a * 0.1f, -alpha.a * 0.1f);

        if(isFade)
        {
            transform.GetChild(0).transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed*1.5f);
            text.color = alpha;
            outline.effectColor = new Color(0, 0, 0, alpha.a * 0.3f);
            shadow.effectColor = new Color(0, 0, 0, alpha.a * 0.3f);
        }


    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void FadeIn()
    {
        isFade = true;
    }

    public void GetFree()
    {
        //this.transform.parent = this.transform.parent.transform.parent;
        this.transform.SetParent(this.transform.parent.transform.parent);
    }
}
