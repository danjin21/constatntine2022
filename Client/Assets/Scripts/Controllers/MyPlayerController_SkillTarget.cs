using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using WindowsInput;
using System;

public class MyPlayerController_SkillTarget : MonoBehaviour
{

    Vector3Int targetPosition;

    Vector2 Dir;

    public BaseController target;

    MyPlayerController MyPlayerController;

    public List<BaseController> objects = new List<BaseController>();

    void Start()
    {
        MyPlayerController = GetComponent<MyPlayerController>();

        MakeTargetBox();        
    }

    // Update is called once per frame
    void Update()
    {


        if(MyPlayerController.IsTargetChoice != -1)
        {



            if( Input.GetKeyDown(KeyCode.DownArrow))
            {
                Dir = new Vector2(0, -1);
                FindObject(1);
            }
            else if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Dir = new Vector2(-1, 0);
                FindObject(2);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Dir = new Vector2(1, 0);
                FindObject(3);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Dir = new Vector2(0, 1);
                FindObject(4);
            }
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                // 채팅창 켜져있을때를 봐야할듯..


                // 스킬을 보낸다 오브젝트랑 같이해서.
                //스킬사용
                C_Skill skillPacket = new C_Skill() { Info = new SkillInfo() };
                skillPacket.Info.SkillId = MyPlayerController.IsTargetChoice; // 힐
                skillPacket.TargetId = -1;

                if (target != null)
                    skillPacket.TargetId = target.Id;

                Managers.Network.Send(skillPacket);

                MyPlayerController.IsTargetChoice = -1;

                // 자체 쿨타임
                MyPlayerController.SkillCool();

                effect.SetActive(false);

            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                MyPlayerController.IsTargetChoice = -1;

                effect.SetActive(false);
            }
        }


    }

    public void FindObject(int dir)
    {

        BaseController shortObject = target ;
        float distance = 100000;

        GameObject[,] objects = Managers.Map.GetObject();        


        foreach (GameObject B in objects)
        {
            if (B == null)
                continue;

            BaseController A = B.GetComponent<BaseController>();


            switch (dir)
            {
                case 1: // down
                    if (A.CellPos.y >= target.CellPos.y)
                        continue;
                    break;
                case 2: // left
                    if (A.CellPos.x >= target.CellPos.x)
                        continue;
                    break;
                case 3: // right
                    if (A.CellPos.x <= target.CellPos.x)
                        continue;
                    break;
                case 4: // up
                    if (A.CellPos.y <= target.CellPos.y)
                        continue;
                    break;

            }

            // 자기는 제외
            if (A == target)
                continue;


            float ds = Vector3.Distance(A.CellPos, target.CellPos);

            if (ds < distance)
            {
                shortObject = A;
                distance = Math.Min(ds, distance);
            }


        }

        target = shortObject;
        TargetBox();



        Debug.Log($"오브젝트 위치 : {target.CellPos.x} / {target.CellPos.y} / {target.Id}");
        //foreach (BaseController A in objects)
        //{
          
        //}

 
        
    }


    public GameObject effect;

    public void MakeTargetBox()
    {
        effect = Managers.Resource.Instantiate("Effect/TigerGlaw_select");
        effect.GetComponent<Animator>().Play("Hit_Sword");
        effect.GetComponent<SpriteRenderer>().sortingOrder = 32222;

        effect.SetActive(false);


        // 게임 이펙트를 몇초 후에 삭제
        // GameObject.Destroy(effect, 0.3f);
    }

    public void TargetBox()
    {
        if(effect == null)
        {
            MakeTargetBox();
        }

        effect.transform.position = target.transform.position + new Vector3(0, 0, -20);
        effect.transform.parent = target.transform;

        effect.SetActive(true);



    }
}
