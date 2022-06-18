using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatDelete : MonoBehaviour
{




    public float delayTime = 5;
    public float timer = 5f;

    void Update()

    {

 
            if(timer <5)
            timer += Time.deltaTime;

            if (timer >= delayTime)
            {
                transform.GetComponent<Text>().text = "";
                transform.parent.GetComponent<Image>().enabled = false;
            }

    }



}
