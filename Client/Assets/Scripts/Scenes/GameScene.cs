using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;

    protected override void Init()
    {

        base.Init();

        SceneType = Define.Scene.Game;


        //Managers.Map.LoadMap(2);


        //Screen.SetResolution(1280, 960, false);

        Screen.SetResolution(1280, 960, false);
        //Screen.SetResolution(1920, 1080, false);



        //Screen.SetResolution(1152, 864, true);
        //Screen.SetResolution(1024, 768, true);
        //Screen.SetResolution(640, 480, false);
        //Screen.SetResolution(1280, 960, true);
        //Screen.SetResolution(800, 600, true);
        //Screen.SetResolution(880, 660, false);

        //Screen.SetResolution(1152, 864, false);


        //Screen.SetResolution(1920, 1440, false);
        //Screen.SetResolution(1920, 1080, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();


  

        //GameObject player = Managers.Resource.Instantiate("Creature/Player");
        //player.name = "Player";
        //Managers.Object.Add(player);

        //for(int i = 0; i < 5; i ++)
        //{
        //    GameObject monster = Managers.Resource.Instantiate("Creature/Monster");
        //    monster.name = $"Monster_{i+1}";

        //    // 랜덤 위치 스폰 ( 일단 겹쳐도 OK )
        //    Vector3Int pos = new Vector3Int()
        //    {
        //        x = Random.Range(-15, 15),
        //        y = Random.Range(-11, 11)
        //    };

        //    MonsterController mc = monster.GetComponent<MonsterController>();
        //    mc.CellPos = pos;

        //    Managers.Object.Add(monster);
        //}




        //Managers.UI.ShowSceneUI<UI_Inven>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        ////Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);
    }

    public override void Clear()
    {
        
    }

  
}
