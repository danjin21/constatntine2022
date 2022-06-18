using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontPixelPerfect : MonoBehaviour
{
    [SerializeField]
    public Font[] fonts;

    // Start is called before the first frame update
    void Start()
    {
        //폰트가 Pixel Perfect 하게 보이도록 설정
        for (int i = 0; i < fonts.Length; i++)
        {
            fonts[i].material.mainTexture.filterMode = FilterMode.Point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
