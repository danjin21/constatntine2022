using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;
using Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
    
    #if UNITY_EDITOR



    // % (Ctrl) , #(Shift) , & (alt)


    private static void GenerateMap_Test()
    {
        //if (EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Cancel"))
        //{
        //    new GameObject("Hello World");
        //}

        GameObject go = GameObject.Find("Map0");

        if (go == null)
            return;

        Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

        if (tm == null)
            return;

   
        using (var writer = File.CreateText("Assets/Resources/Map/Map0.txt"))
        {
            writer.WriteLine(tm.cellBounds.xMin);
            writer.WriteLine(tm.cellBounds.xMax);
            writer.WriteLine(tm.cellBounds.yMin);
            writer.WriteLine(tm.cellBounds.yMax);

            for(int y = tm.cellBounds.yMax-1; y>=tm.cellBounds.yMin; y--)
            {
                for(int x = tm.cellBounds.xMin; x<= tm.cellBounds.xMax-1; x++)
                {
                    TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));

                    if (tile != null)
                        writer.Write("1");
                    else
                        writer.Write("0");
                }
                writer.WriteLine();
            }

        }


    }


    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        //if (EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Cancel"))
        //{
        //    new GameObject("Hello World");
        //}

        GenerateByPath("Assets/Resources/Map");
        GenerateByPath("../Common/MapData");

    }



    private static void GenerateByPath(string pathPrefix)
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameObjects)
        {

            Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);
            Tilemap tmPortal = Util.FindChild<Tilemap>(go, "Tilemap_Portal", true);

            using (var writer = File.CreateText($"{pathPrefix}/{go.name}.txt"))
            {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                for (int y = tmBase.cellBounds.yMax-1; y >= tmBase.cellBounds.yMin; y--)
                {
                    for (int x = tmBase.cellBounds.xMin; x < tmBase.cellBounds.xMax; x++)
                    {

                        // Base 로 초기 구성
                        TileBase tileBase = tmBase.GetTile(new Vector3Int(x, y, 0));

                        
                        if(tileBase != null)
                        {
                            // Base 가 있으면 그 안에서 타일을 구분한다

                            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));

                            if (tile != null)
                            {


                                TileBase tile2 = tmPortal.GetTile(new Vector3Int(x, y, 0));

                                if (tile2 != null)
                                    writer.Write("2");
                                else
                                    writer.Write("1");
                            }
                            else
                            {

                                TileBase tile2 = tmPortal.GetTile(new Vector3Int(x, y, 0));

                                if (tile2 != null)
                                    writer.Write("2");
                                else
                                    writer.Write("0");
                            }


                        }
                        else
                        {

                            TileBase tile2 = tmPortal.GetTile(new Vector3Int(x, y, 0));

                            if (tile2 != null)
                                writer.Write("2");
                            else
                                writer.Write("1");

                        }





                    }
                    writer.WriteLine();
                }

            }
        }

    }



    [MenuItem("Tools/GenerateMap_Portal")]
    private static void GenerateMap_Portal()
    {
        //if (EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Cancel"))
        //{
        //    new GameObject("Hello World");
        //}

        GenerateByPath_Portal("Assets/Resources/Data");


    }



    private static void GenerateByPath_Portal(string pathPrefix)
    {

        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        using (var writer = File.CreateText($"{pathPrefix}/PortalData.json"))
        {
            writer.Write("{");
            writer.WriteLine();
            writer.Write(" \"portals\":[ ");
            writer.WriteLine();




            foreach (GameObject go in gameObjects)
            {

                Tilemap tmPortalInfo = Util.FindChild<Tilemap>(go, "Tilemap_Portal", true);

                if (tmPortalInfo == null)
                    continue;

                GameObject portalObj = tmPortalInfo.gameObject;

                foreach(Transform child in portalObj.transform)
                {
                    PortalTile pt = child.GetComponent<PortalTile>();

                    // 위치만 가지고, posX 와 posY 와 map 정보를 가져온다.

                    float posXF;
                    float posYF;
                    int mapId;
                    int posX;
                    int posY;

                    posXF = (child.position.x / 32);
                    posYF = (child.position.y / 32);
                    string mapIdString = go.name.Substring(4);
                    mapId = int.Parse(mapIdString);

                    if (posXF >= 0)
                        posX = (int)Math.Truncate(posXF);
                    else
                        posX = -(int)Math.Ceiling(-posXF);

                    if (posYF >= 0)
                        posY = (int)Math.Truncate(posYF);
                    else
                        posY = -(int)Math.Ceiling(-posYF);

                    int portalId = int.Parse(child.name);




                    writer.Write("{");
                    writer.WriteLine();
                    writer.Write($" \"portalId\": \"{mapId}{ portalId.ToString("000") }\", \"posX\": \"{posX}\", \"posY\": \"{posY}\", \"map\": \"{mapId}\", \"destPortal\": \" {pt.destMap}{(pt.destPortal).ToString("000")}\",  \"destPosX\": \"{pt.destPosX}\", \"destPosY\": \"{pt.destPosY}\", \"destMap\": \"{pt.destMap}\",\"direction\": \"{pt.direction}\",");
                    writer.WriteLine();
                    writer.Write("},");
                    writer.WriteLine();

                }

            }

            writer.Write("]");
            writer.WriteLine();
            writer.Write("}");
            writer.WriteLine();


        }

    }


    // NpcData와 NPC 클래스는 JSON 구조에 맞춰 정의되어야 함
    [System.Serializable]
    public class NpcData
    {
        public List<NPC> npcs;
    }

    [System.Serializable]
    public class NPC
    {
        public string npcId;
        public string name;
        public string map;
        public List<Chat> chats;
        public string quest;
        public string posX;
        public string posY;
        public string iconPath;
        public List<Product> products;
        public string merchant;
    }

    [System.Serializable]
    public class Chat
    {
        public string index;
        public string chat;
        // 다른 필요한 필드 추가 가능
    }

    [System.Serializable]
    public class Product
    {
        public string templateId;
        // 다른 필요한 필드 추가 가능
    }



    [MenuItem("Tools/GenerateMap_Object")]
    private static void GenerateMap_Object()
    {


        GenerateByPath_Object("Assets/Resources/Data");


    }


    private static void GenerateByPath_Object(string pathPrefix)
    {

        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");


        string jsonPath = $"{pathPrefix}/NpcData.json";
        string jsonText = File.ReadAllText(jsonPath);

        // JSON 데이터를 NPCData 객체로 변환
        NpcData npcData = JsonUtility.FromJson<NpcData>(jsonText);







        foreach (GameObject go in gameObjects)
        {

            Tilemap tmObjectInfo = Util.FindChild<Tilemap>(go, "Tilemap_Object", true);

            if (tmObjectInfo == null)
                continue;

            GameObject Obj = tmObjectInfo.gameObject;

            foreach (Transform child in Obj.transform)
            {
                ObjectTile Object = child.GetComponent<ObjectTile>();

                if (Object == null)
                    continue;

                // 위치만 가지고, posX 와 posY 와 map 정보를 가져온다.

                float posXF;
                float posYF;
                int Id;
                int posX;
                int posY;
                int mapId;

                string mapIdString = go.name.Substring(4);
                mapId = int.Parse(mapIdString);


                posXF = (child.position.x / 32);
                posYF = (child.position.y / 32);
                Id = Object.Id;

                if (posXF >= 0)
                    posX = (int)Math.Truncate(posXF);
                else
                    posX = -(int)Math.Ceiling(-posXF);

                if (posYF >= 0)
                    posY = (int)Math.Truncate(posYF);
                else
                    posY = -(int)Math.Ceiling(-posYF);

                // NPC 데이터에서 ID가 일치하는 NPC 찾기
                foreach (var npc in npcData.npcs)
                {
                    if (npc.npcId == Id.ToString())
                    {
                        // posX와 posY 업데이트
                        npc.posX = posX.ToString();
                        npc.posY = posY.ToString();
                        npc.map = mapId.ToString();
                        break;
                    }
                }



            }


            // 수정된 NPCData 객체를 다시 JSON 문자열로 변환
            string updatedJsonText = JsonUtility.ToJson(npcData);

            // 업데이트된 JSON 문자열을 파일에 저장
            File.WriteAllText(jsonPath, updatedJsonText);

        }


    }







#endif
}
