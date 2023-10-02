using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTile : MonoBehaviour
{

    public int direction;

    public Sprite[] sprites;

    public int map;
    public int posX;
    public int posY;
    // public int portaId;
    public int destPortal;
    public int destMap;
    public int destPosX;
    public int destPosY;

    // Start is called before the first frame update
    void Start()
    {
        if (sprites.Length != 0)
            GetComponent<SpriteRenderer>().sprite = sprites[direction];


        Debug.Log("포탈 고유 아이디 : " + gameObject.GetInstanceID());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
