using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : BaseController
{

    public ProjectileInfo projectileInfo = new ProjectileInfo();
    public GameObject target;
    public int distance;
    Vector3Int destCellPos;
    Vector3 destPos;

    protected override void Init()
    {

        PosInfo = projectileInfo.PosInfo;
        SyncPos();

        switch (Dir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                destCellPos = CellPos + new Vector3Int(0, distance, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                destCellPos = CellPos + new Vector3Int(0, -distance, 0);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                destCellPos = CellPos + new Vector3Int(-distance, 0, 0);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                destCellPos = CellPos + new Vector3Int(distance, 0, 0);
                break;
        }

        State = CreatureState.Moving;

        destPos = Managers.Map.CurrentGrid.CellToWorld(destCellPos) + new Vector3(16.0f, 36.0f, 0);

        if (target != null)
        {
            destPos = target.transform.position;
        }

        Speed = 1000;

        //this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);


    }

    float run = 1.0f;

    protected override void UpdateMoving()
    {

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

        Vector3 moveDir = destPos - transform.position;
        float dist = moveDir.magnitude;

        if (dist < Speed * Time.smoothDeltaTime)
        {
            Managers.Resource.Destroy(this.gameObject);

            if (target != null)
            {
                Debug.Log(target.GetComponent<CreatureController>().getShot);
                target.GetComponent<CreatureController>().getShot = true;
                Debug.Log(target.GetComponent<CreatureController>().getShot);
                Debug.Log("targetID : " + target.GetComponent<CreatureController>().Id);
            }
        }

        Debug.Log($"dist : {dist}");

    }


}
