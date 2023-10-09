using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;

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



    [MenuItem("Tools/GenerateMap_Portal_YiGwangSuk")]
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


                    writer.Write("{");
                    writer.WriteLine();
                    writer.Write($" \"portalId\": \"{child.name}\", \"posX\": \"{posX}\", \"posY\": \"{posY}\", \"map\": \"{mapId}\", \"destPortal\": \"{pt.destPortal}\",  \"destPosX\": \"{pt.destPosX}\", \"destPosY\": \"{pt.destPosY}\", \"destMap\": \"{pt.destMap}\",\"direction\": \"{pt.direction}\",");
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



#endif
}
