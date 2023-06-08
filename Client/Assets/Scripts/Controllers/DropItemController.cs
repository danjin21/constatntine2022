using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;


public class DropItemController : BaseController
{

    Coroutine _coDrop;

    public int DropItemLayerBack = 0;

    public Vector3 DestPos;
    public Vector3 CurrentPos;

<<<<<<< HEAD
    public override void Init()
=======
    public bool Throwing = false;

    protected override void Init()
>>>>>>> 이동_분기_5차
    {

        _sprite = GetComponent<SpriteRenderer>();


        DestPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 16.0f + 80.0f, - (DropItemLayerBack % 1000000) / 10000.000000f);

        CurrentPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(16.0f, 16.0f,  - (DropItemLayerBack%1000000)/10000.000000f);
        transform.position = CurrentPos;

        // 초기에 레이어 설정
        //_sprite.sortingOrder = -(int)PosInfo.PosY-10000;
        _sprite.sortingOrder = -(int)PosInfo.PosY-2;

        if (Throwing)
        {
            _coDrop = StartCoroutine(DropEffect());
            Throwing = false;
        }
    }

    public void ThrowingItem()
    {
    
    }






    IEnumerator DropEffect()
    {
        gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 1);

        // yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 10; i++)
        {
    
            gameObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.1f);


            yield return new WaitForSeconds(0.01f);
        }

        _coDrop = null;
    }




    IEnumerator MoveTheBall()
    {
        int jumpLayer = 2;
        _sprite.sortingOrder += jumpLayer;

        float y = 0;

        float b = 20;

        float spin = 12000.0f;

        while(transform.position.y < DestPos.y-1)
        {
            y += spin * Time.smoothDeltaTime;

            //transform.position += new Vector3(0, 150 * Time.smoothDeltaTime, 0);

            transform.position = Vector3.Lerp(transform.position, DestPos, b * Time.smoothDeltaTime);

            b += 5;

            transform.rotation = Quaternion.Euler(0, 0, y);
            yield return new WaitForSeconds(0.02f);
        }

        float a = 100;
 
        while (transform.position.y > CurrentPos.y)
        {

            y += spin * Time.smoothDeltaTime;

            //transform.position -= new Vector3(0, 150 * Time.smoothDeltaTime, 0);
            transform.position = Vector3.MoveTowards(transform.position, CurrentPos, a * Time.smoothDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, y);
            a += 100;
            yield return new WaitForSeconds(0.02f);
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);

        _sprite.sortingOrder -= jumpLayer;

        _coDrop = null;
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
