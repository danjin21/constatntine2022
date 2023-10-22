using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerCore;
using System;
using System.Net;
using Google.Protobuf;
using TMPro;
using UnityEngine.EventSystems;
using WindowsInput;
using System.Text.RegularExpressions;



public class AnimationManager
{
    Texture2D[] sprites;


    public List<Sprite[]> MobSprites = new List<Sprite[]>();

    public Sprite[] MobSprites_6;

    public void Init()
    {
        sprites = Resources.LoadAll<Texture2D>("Textures/Monster/MonsterSprites");

        // 순서 정렬 1,10,11 -> 1,2,3,4,~10,11
        Array.Sort(sprites, delegate (Texture2D x, Texture2D y) { return int.Parse(x.name).CompareTo(int.Parse(y.name)); });


        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].GetType() == typeof(Texture2D))
            {


                Sprite[] MobSpritesElement = Resources.LoadAll<Sprite>($"Textures/Monster/MonsterSprites/{sprites[i].name}");

                //Debug.Log("경로 : " + $"Textures/Monster/MonsterSprites/{sprites[i].name}");
                //Debug.Log("순서 " + sprites[i].ToString());
                //Debug.Log("팟 " + MobSpritesElement.Length);

                //foreach (Sprite p in MobSpritesElement)
                //{
                //    Debug.Log("파일들 " + p.ToString());
                //}

             

                MobSprites.Add(MobSpritesElement);

            }

        }





    }
}