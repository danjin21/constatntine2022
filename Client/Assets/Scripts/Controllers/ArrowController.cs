using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
    float run;

    public float count_checkDistance;

    public int shot;
    public List<ArrowController> childArrowList = new List<ArrowController>();
    public bool IsParent;

    protected override void Init()
    {

        if (IsParent)
        {
            for (int i = 0; i < shot - 1; i++)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
                go.name = "Arrow" + i;
                ArrowController ac = go.GetComponent<ArrowController>();
                ac.PosInfo = this.PosInfo;
                ac.Stat = this.Stat;
                ac.SyncPos();
                ac.IsParent = false;
                childArrowList.Add(ac);
                Debug.Log("child 수 " + childArrowList.Count);
                ac.gameObject.SetActive(false);

            }

            StartCoroutine(coMultiShot(0.1f));
        }

        switch (Dir)
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

        // Debug.Log("화살의 ID = " + Id);
        //_speed = 500.0f;

        run = 1.0f;

        count_checkDistance = 0;

        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    IEnumerator coMultiShot(float time)
    {
        foreach (ArrowController child in childArrowList)
        {
            yield return new WaitForSeconds(time);
            child.gameObject.SetActive(true);
        }
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




    protected  void UpdateMoving_2()
    {

        //// 이동이 아니면 리턴한다
        //if (State != CreatureState.Moving)
        //    return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 36.0f, 0);
        Vector3 moveDir = destPos - transform.position;





        // 도착 여부 체크
        float dist = moveDir.magnitude;


        Vector3Int destPosInt = new Vector3Int((int)destPos.x, (int)destPos.y, (int)destPos.z);

        if (dist < Speed * Time.smoothDeltaTime)
        {
           

            if (Managers.Map.CanGo(destPosInt))
                transform.position = destPosInt;

            //GameObject CurrentObject;

            //CurrentObject = Managers.Object.FindCreature(destPosInt);

            //if (CurrentObject != null && CurrentObject != gameObject)
            //{
               
            //    Managers.Object.Remove(Id);
            //}
           

            MoveToNextPos();

        }
        else
        {
            Vector3 A = transform.position;

            // 살짝 다 안왔을 때에는 채워주고 난뒤에 이동시킨다.
            // 0.00001 <-- 이부분은 time.deltatime 으로 조정해줘야ㅕ할듯
            if (Mathf.Abs(moveDir.x) > 0.000001f && Mathf.Abs(moveDir.y) > 0.000001f)
            {

                if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
                {
                    transform.position = new Vector3(destPos.x, transform.position.y, transform.position.z);
                }
                else if (Mathf.Abs(moveDir.y) < Mathf.Abs(moveDir.x))
                {
                    transform.position = new Vector3(transform.position.x, destPos.y, transform.position.z);
                }
            }
            else
            {
                transform.position += moveDir.normalized * Speed * Time.smoothDeltaTime * run;

            }
        }


    }


    float a;

    protected override void UpdateMoving()
    {
        a = 0.105f;


        count_checkDistance += Time.smoothDeltaTime;



        if (count_checkDistance < a) // 0.05f 0.24f 0.14f
            return;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);

        if (count_checkDistance < a*2) // 0.05f 0.24f 0.14f
            return;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.22f);

        if (count_checkDistance < a*3) // 0.05f 0.24f 0.14f
            return;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.45f);

        if (count_checkDistance < a*4) // 0.24f
            return;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);



        Vector3 destPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3Int destPosInt = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);


        Vector3Int CellPosInt = Managers.Map.CurrentGrid.WorldToCell(destPosInt) - new Vector3Int ( 0, 1,0);
        //Debug.Log("망치망치" + CellPos + "/" + CellPosInt +$"{destPos} / {destPosInt}");


        if (Managers.Map.CanGo(CellPosInt))
        {

        }
        else
        {
            Managers.Object.Remove(Id);
            removeChild();
        }



        if (Managers.Map.Find(CellPosInt) != null)
        {
            //Debug.Log("화살막힘4" + Managers.Map.Find(CellPosInt).name);
            Managers.Object.Remove(Id);
            removeChild();
        }

        //// 이동이 아니면 리턴한다
        //if (State != CreatureState.Moving)
        //    return;


        //float run = 0.55f;
        //float run = 0.55f;

        //float run = 0.67f;

        //float run = 0.57f;

        float run = 0.6f;

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







        // 진짜 Myplayer처럼 셀단위로 움직이게 하거나..
        // 그냥 겹치는 경우 오브젝트를 확인..

        //Vector3Int A = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);


        //Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(A) + new Vector3(16.0f, 36.0f, 0);


        //Vector3Int destPosInt = new Vector3Int((int)destPos.x, (int)destPos.y, (int)destPos.z);
        //Debug.Log("화살의 위치" + destPosInt);

        //if (Managers.Map.CanGo(destPosInt))
        //{

        //}
        //else
        //{
        //    Debug.Log("호우우");
        //    //Managers.Object.Remove(Id);
        //    //Managers.Resource.Destroy(gameObject);
        //}


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

    protected override void MoveToNextPos()
    {
        // 화살은 base.MoveToNextPos 안해준다.

        //if (Managers.Map.CanGo(CellPos))
        //{
          

        //}
        //else
        //{
        //    Debug.Log("화살막힘3");
        //    Blocked = true;
        //    Managers.Object.Remove(Id);
        //}

        

        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;

                break;

            case MoveDir.Down:
                destPos += Vector3Int.down;


                break;

            case MoveDir.Left:
                destPos += Vector3Int.left;

                break;

            case MoveDir.Right:
                destPos += Vector3Int.right;

                break;
        }


        if (Managers.Map.Find(destPos) != null)
        {
            Debug.Log("화살막힘4" + Managers.Map.Find(destPos).name);
            Managers.Object.Remove(Id);
            removeChild();
        }


        if (Managers.Map.CanGo(destPos))
        {
            CellPos = destPos;
        }
        else
        {
            Debug.Log("화살막힘2");
            Managers.Object.Remove(Id);
            removeChild();
        }


    }


    public void removeChild()
    {
        if (IsParent == false)
            return;

        // 서버에서 먼저 지워질경우가 있어서.. 그 경우에는..
        // 그.. S_DespawnHandler 부분에서 이 함수 실행하게 만든다.


        int i = 1;

        Debug.Log($"childArrowList Count = " + childArrowList.Count);

        List<ArrowController> childArrowList_Temp = new List<ArrowController>();
        childArrowList_Temp = childArrowList;

        
        foreach (ArrowController child in childArrowList_Temp) // 임시 리스트
        {

            // 실제로도 지워준다
            Managers.Resource.Destroy(child.gameObject, 0.1f * i);

            i++;

            Debug.Log($"@@@@{i}");

        }

    }

}
