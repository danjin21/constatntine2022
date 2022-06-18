using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageText : MonoBehaviour
{

    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    TextMeshPro text;
    Color alpha;

    public string damage;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();

        alpha = text.color;

        text.text = damage;

        moveSpeed = 10; //10
        alphaSpeed = 0.2f;

        destroyTime = 4.0f;/*1.5f;*/

        Invoke("DestroyObject", destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        //alpha.a = Mathf.Lerp(alpha.a+2000, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
