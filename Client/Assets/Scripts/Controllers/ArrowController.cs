using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
    protected override void Init()
    {

        switch(Dir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        State = CreatureState.Moving;
 

        base.Init();

        //_speed = 500.0f;

    }

    protected override void UpdateIdle()
    {



        //// 멈춰있을때 위치가 다르면, 서버의 위치대로 저장해준다. BaseCreature들 모두
        //if (Managers.Map != null)
        //{
        //    Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);

        //    if (transform.position != destPos)
        //    {
        //        transform.position = destPos;
        //    }
        //}




    }







    protected override void UpdateMoving()
    {


        //// 이동이 아니면 리턴한다
        //if (State != CreatureState.Moving)
        //    return;


        float run = 0.55f;

        switch (Dir)
        {
            case MoveDir.Up:
                transform.position += new Vector3(0, run, 0) * Speed * Time.smoothDeltaTime;
                break;
            case MoveDir.Down:
                transform.position += new Vector3(0, -run, 0) * Speed * Time.smoothDeltaTime;
                break;
            case MoveDir.Left:
                transform.position += new Vector3(-run, 0, 0) * Speed * Time.smoothDeltaTime;
                break;
            case MoveDir.Right:
                transform.position += new Vector3(run, 0, 0) * Speed * Time.smoothDeltaTime;
                break;
        }




    }




    protected override void UpdateAnimation()
    {
       
    }

    //// 이동 가능한 상태일 때, 실제 좌표를 이동한다.
    //protected override void MoveToNextPos()
    //{


    //        Vector3Int destPos = CellPos;

    //        switch (Dir)
    //        {
    //            case MoveDir.Up:
    //                destPos += Vector3Int.up;

    //                break;

    //            case MoveDir.Down:
    //                destPos += Vector3Int.down;

    //                break;

    //            case MoveDir.Left:
    //                destPos += Vector3Int.left;

    //                break;

    //            case MoveDir.Right:
    //                destPos += Vector3Int.right;

    //                break;
    //        }


    //        if (Managers.Map.CanGo(destPos))
    //        {

    //            GameObject go = Managers.Object.Find(destPos);

    //            if (go == null)
    //            {
    //                CellPos = destPos;
    //            }
    //            else
    //            {

    //                CreatureController cc = go.GetComponent<CreatureController>();
    //                if(cc != null)
    //                {
    //                    cc.OnDamaged();
    //                }

    //                Managers.Resource.Destroy(gameObject);
    //            }


    //        }
    //        else
    //        {
    //            Managers.Resource.Destroy(gameObject);
    //        }

        
    //}



}
