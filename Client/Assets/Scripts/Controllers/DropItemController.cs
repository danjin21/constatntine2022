using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;


public class DropItemController : BaseController
{

    Coroutine _coSkill;

    public int DropItemLayerBack = 0;

    public Vector3 DestPos;
    public Vector3 CurrentPos;

    protected override void Init()
    {

        _sprite = GetComponent<SpriteRenderer>();

        float layer = 20f / Id;

        DestPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 16.0f + 80.0f, -(DropItemLayerBack % 10000000) / 100000.000000f);

        CurrentPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 16.0f,  - (DropItemLayerBack%10000000)/100000.000000f);
        transform.position = CurrentPos;

        // 초기에 레이어 설정
        _sprite.sortingOrder = -(int)PosInfo.PosY-10000;

        

    }


    IEnumerator MoveTheBall()
    {

        float y = 0;

        while(transform.position.y < DestPos.y-5)
        {
            y += 4000.0f * Time.smoothDeltaTime;

            //transform.position += new Vector3(0, 150 * Time.smoothDeltaTime, 0);

            transform.position = Vector3.Lerp(transform.position, DestPos, 20*Time.smoothDeltaTime);

            transform.rotation = Quaternion.Euler(0, 0, y);
            yield return new WaitForSeconds(0.02f);
        }

        float a = 3;
 
        while (transform.position.y > CurrentPos.y)
        {

            y += 4000.0f * Time.smoothDeltaTime;

            //transform.position -= new Vector3(0, 150 * Time.smoothDeltaTime, 0);
            transform.position = Vector3.MoveTowards(transform.position, CurrentPos, a * Time.smoothDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, y);
            a += 50;
            yield return new WaitForSeconds(0.02f);
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);

    }


    protected override void UpdateIdle()
    {

    }




    protected override void UpdateController()
    {

    }


    public virtual void OnDamaged(int damage)
    {



        Managers.Object.Remove(damage);

    }




}
