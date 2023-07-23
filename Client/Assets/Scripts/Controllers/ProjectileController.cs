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
    public int shots;

    protected Coroutine _coChildShot;

    protected override void Init()
    {

        // 혹시 몰라서 10 초 뒤에는 사라지게
        Managers.Resource.Destroy(this.gameObject, 10.0f);

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

        if(shots >1)
            _coChildShot = StartCoroutine("CoStartChildShot", shots);

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

            if (target != null)
            {
                Debug.Log(target.GetComponent<CreatureController>().getShot);
                target.GetComponent<CreatureController>().getShot = true;
                Debug.Log(target.GetComponent<CreatureController>().getShot);
                Debug.Log("targetID : " + target.GetComponent<CreatureController>().Id);
            }

            Managers.Resource.Destroy(this.gameObject);

        }

        Debug.Log($"dist : {dist}");

    }


    IEnumerator CoStartChildShot(int shot)
    {
        List<GameObject> childArrows = new List<GameObject>();

        for(int i=0;i<shot-1;i++)
        {

            GameObject projectile = Managers.Resource.Instantiate("Creature/Projectile");
            projectile.name = "Projectile";

            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.projectileInfo = projectileInfo;
            pc.distance = distance;
            pc.shots = -1;

            // 타겟이 있다면
            if (target != null)
                pc.target = target;

            childArrows.Add(projectile);
            projectile.SetActive(false);

        }


        foreach (GameObject child in childArrows)
        {
            yield return new WaitForSeconds(0.1f);
            child.gameObject.SetActive(true);
        }


        //yield return new WaitForSeconds(0.5f); // State에 대한 딜레이 | 클라이언트 측에서도 남발하지못하게 해줘야한다.

        _coChildShot = null;



    }


}
