using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    
    // 서버한테 아이디를 받음 (ID,GameObject)
    public Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    // List<GameObject> _objects = new List<GameObject>();

    //int DropItem_objectCount = 0;

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0X7F;
        return (GameObjectType)type;
    }



    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
            return ;

        // 중복되게 추가가 오면 무시를 한다.
        if (_objects.ContainsKey(info.ObjectId))
            return ;

        GameObject resultObject = null;


        GameObjectType objectType = GetObjectTypeById(info.ObjectId);

        if(objectType == GameObjectType.Player)
        {
            if (myPlayer == true)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                //MyPlayer.Stat = info.StatInfo;
                // 참조값은 equal로 넣지 않고 복사인 MetgeFrom 으로 복사한다.
                MyPlayer.Stat.MergeFrom(info.StatInfo);
                MyPlayer.SyncPos();

                Debug.Log("플레이어 생서잉요" + MyPlayer);

                resultObject = go;

            }
            else
            {



                GameObject go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                //pc.Stat = info.StatInfo;
                // 참조값은 equal로 넣지 않고 복사인 MetgeFrom 으로 복사한다.
                pc.Stat.MergeFrom(info.StatInfo);
                pc.SyncPos();

                resultObject = go;
            }
        }
        else if(objectType == GameObjectType.Monster)
        {
            string A = "Creature/Monster_"+ info.StatInfo.TemplateId;

            GameObject go = Managers.Resource.Instantiate(A);
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.Id = info.ObjectId;
            mc.PosInfo = info.PosInfo;
            mc.Stat = info.StatInfo;
            mc.SyncPos();

            Debug.Log("몬스터의 TemplateId = " + mc.Stat.TemplateId);

            resultObject = go;
        }
        else if(objectType == GameObjectType.Projectile)
        {


            GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
            go.name = "Arrow";
            _objects.Add(info.ObjectId, go);

            ArrowController ac = go.GetComponent<ArrowController>();
            ac.PosInfo = info.PosInfo;
            ac.Stat = info.StatInfo;
            ac.Id = info.ObjectId; // 화살 ID 추가 20221104
            //ac.Dir = info.PosInfo.MoveDir;
            //ac.CellPos = new Vector3Int(info.PosInfo.PosX, info.PosInfo.PosY, 0);
            ac.SyncPos();


            resultObject = go;



        }
        else if (objectType == GameObjectType.DropItem)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/DropItem");
            go.name = "DropItem";
            _objects.Add(info.ObjectId, go);

            DropItemController dc = go.GetComponent<DropItemController>();
            dc.PosInfo = info.PosInfo;
            dc.Stat = info.StatInfo;
            dc.Id = info.ObjectId;

            // 드랍아이템별 같은 위치일때 레이어 구분을 위해 히스토리 남김.
            //dc.DropItemLayerBack = DropItem_objectCount;
            dc.DropItemLayerBack = info.ObjectId;
            //DropItem_objectCount += 1;


            Debug.Log("Template ID : " + dc.Stat.TemplateId + "/ ObjectId :" + dc.Id );

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(dc.Stat.TemplateId, out itemData);

            // 없으면 크래쉬

            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            dc.SetSprite(icon);

     



            //ac.Dir = info.PosInfo.MoveDir;
            //ac.CellPos = new Vector3Int(info.PosInfo.PosX, info.PosInfo.PosY, 0);
            dc.SyncPos();

            resultObject = go;
        }
        else if(objectType == GameObjectType.Npc)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Npc");

            _objects.Add(info.ObjectId, go);

            //BaseController bc = go.GetComponent<BaseController>();
            //bc.Id = info.ObjectId;
            //bc.PosInfo = info.PosInfo;
            //bc.Stat = info.StatInfo;
            //bc.SyncPos();

            //Data.NpcData npcData = null;
            //Managers.Data.NpcDict.TryGetValue(bc.Stat.TemplateId, out npcData);


            NpcController nc = go.GetComponent<NpcController>();
            nc.Id = info.ObjectId;
            nc.PosInfo = info.PosInfo;
            nc.Stat = info.StatInfo;
            nc.SyncPos();

            Data.NpcData npcData = null;
            Managers.Data.NpcDict.TryGetValue(nc.Stat.TemplateId, out npcData);

            // 없으면 크래쉬

            Sprite icon = Managers.Resource.Load<Sprite>(npcData.iconPath);
            go.transform.GetComponent<SpriteRenderer>().sprite = icon;

            // 이름 클라쪽에서 지어주기
            go.name = npcData.name;

            // 내가 가지고 있는 퀘스트들을 갖고와서, npc랑 같다면 (status -1 아닌것 중에 )
            // 물음표 보이게 해주자.
            // 아예 현재 진행중이어야 되서 완전히 같아야 한다.,

            Quest checkQuest = Managers.Quest.Find(t => t.NpcId == nc.Stat.TemplateId);

            if(checkQuest != null)
            {
                if(checkQuest.Status != -1)
                {
                    // npc의 머리위에 물음표를 붙여준다.
                    
                }
            }

            resultObject = go;
        }

        if(resultObject != null  )
        {
            CreatureController cc = resultObject.GetComponent<CreatureController>();

            // 크리쳐인경우에만 오브젝트 타일에 넣어준다.
            // DropItem이나 화살 같은건 안됨
            if (cc == null)
                return;

            // Map의 Object 리스트에 반영해준다.
            Managers.Map.ApplyMove(resultObject, resultObject.GetComponent<BaseController>().PosInfo.PosX, resultObject.GetComponent<BaseController>().PosInfo.PosY, resultObject.GetComponent<BaseController>().PosInfo.PosX, resultObject.GetComponent<BaseController>().PosInfo.PosY);

        }



    }

    //public void Add(int id, GameObject go)
    //{
    //    _objects.Add(id, go);
    //}

    //public void Remove(int id)
    //{

    //    if (MyPlayer != null && MyPlayer.Id == id)
    //        return;

    //    // 중복되게 추가가 오면 무시를 한다.
    //    if (_objects.ContainsKey(id) == false)
    //        return;

    //    GameObject go = FindById(id);
    //    if (go == null)
    //        return;

    //    _objects.Remove(id);

    //    // 맵 오브젝트 리스트에서 뺀다.
    //    Managers.Map.ApplyLeave(go, go.GetComponent<BaseController>().CellPos.x, go.GetComponent<BaseController>().CellPos.y);



    //    Managers.Resource.Destroy(go);
    //}


    public void Remove(int id, float time = 0)
    {

        if (MyPlayer != null && MyPlayer.Id == id)
            return;

        // 중복되게 추가가 오면 무시를 한다.
        if (_objects.ContainsKey(id) == false)
            return;

        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);

        // 맵 오브젝트 리스트에서 뺀다.
        Managers.Map.ApplyLeave(go, go.GetComponent<BaseController>().CellPos.x, go.GetComponent<BaseController>().CellPos.y);



        Managers.Resource.Destroy(go,time);
    }



    // Clear()로 합쳤다. ↓↓↓↓ (맨아래로 이동)
    //public void RemoveAll()
    //{
    //    Clear();
    //    MyPlayer = null;

    //}

    // 이대로 만들면 찌꺼기가 남아서 ID값이 중복된다. ↑ 위로 수정함.
    //public void RemoveMyPlayer()
    //{
    //    if (MyPlayer == null)
    //        return;

    //    Remove(MyPlayer.Id);
    //    MyPlayer = null;
    //}


    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;

    }



    public GameObject FindCreature(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects.Values)
        {
            // 화살은 충돌되는게 아니므로, Base가 아닌 Creature로
            CreatureController cc = obj.GetComponent<CreatureController>();

            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject Find(Func<GameObject,bool> condition)
    {

        foreach (GameObject obj in _objects.Values)
        {
            if (condition.Invoke(obj) )
                return obj;
        }

        return null;
    }


    public void Clear()
    {

        foreach (GameObject obj in _objects.Values)
            Managers.Resource.Destroy(obj);


         _objects.Clear();
        MyPlayer = null;
    }
}
