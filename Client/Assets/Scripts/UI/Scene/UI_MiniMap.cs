using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniMap : UI_Base
{
    MyPlayerController player;
    MyPlayerController_SkillTarget MySkillTarget;

    enum Texts
    {
        mapText,
        miniMapNameText,
    }

    // Start is called before the first frame update
    public void RefreshUI()
    {
        MyPlayerController player = Managers.Object.MyPlayer;

        MapInfoData mapData = null;
        Managers.Data.MapDict.TryGetValue(player.Stat.Map, out mapData);

        Get<Text>((int)Texts.miniMapNameText).text = $"{mapData.name}";

        player = Managers.Object.MyPlayer;
        MySkillTarget = player.GetComponent<MyPlayerController_SkillTarget>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));


    }


    public void DrawCollision()
    {

        // @ : 너비
        // 나의 위치로부터 위 @칸, 아래 @칸, 좌 @칸, 우 @칸의 너비까지의 충돌 부분을 구한다.
        // 이동할때마다 반복한다.


        bool[,] col = Managers.Map.GetCollision();

        string arrayText = GetArrayText(col);

        Debug.Log(arrayText);

    }


    private string GetArrayText(bool[,] array)
    {
        string text = "";

        // 배열 요소를 문자열로 변환하여 텍스트에 추가
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                text += array[i, j] ? "1 " : "0 ";
            }

            // 각 행의 끝에 개행 문자 추가
            text += "\n";
        }

        return text;
    }



    public void DrawCollision_center(int x, int y)
    {

        // @ : 너비
        // 나의 위치로부터 위 @칸, 아래 @칸, 좌 @칸, 우 @칸의 너비까지의 충돌 부분을 구한다.
        // 이동할때마다 반복한다.




        bool[,] col = Managers.Map.GetCollision();
        bool[,] portals = Managers.Map.GetPortal();
        GameObject[,] objects = Managers.Map.GetObject();

        string arrayText_center = GetPartialArrayText(col, portals, objects, x, y, 20, 22);

        // Debug.Log(arrayText_center);

        Get<Text>((int)Texts.mapText).text = $"{arrayText_center}";

    }

    private string GetPartialArrayText(bool[,] array, bool[,] portals , GameObject[,] objects, int centerX, int centerY, int rangeX, int rangeY)
    {
        string text = "";

        // 좌측 상단 기준으로 특정 범위만큼의 배열 요소를 문자열로 변환하여 텍스트에 추가
        int startX = Mathf.Max(centerX - rangeX, 0);
        int startY = Mathf.Max(centerY - rangeY, 0);
        int endX = Mathf.Min(centerX + rangeX, array.GetLength(0) - 1);
        int endY = Mathf.Min(centerY + rangeY, array.GetLength(1) - 1);

        // 맵이 화면보다 작을 경우에만 고정을 시켜준다.
        //if (array.GetLength(0) - 1 < rangeX*2 || array.GetLength(1) - 1 < rangeY*2)
        {
            // X 값 
            if (startY == 0 && startY + endY != array.GetLength(1) - 1)
                endY = Mathf.Min(array.GetLength(1) - 1 - startY, rangeY * 2);
            if (endY == array.GetLength(1) - 1 && startY + endY > array.GetLength(1) - 1)
            {
                startY += (endY - startY) - (rangeY*2);

                startY = Mathf.Max(startY, 0);

                //startY += (array.GetLength(1) - 1) - (startY + endY);
            }

            // Y 값 
            if (startX == 0 && startX + endX != array.GetLength(0) - 1)
                endX = Mathf.Min( array.GetLength(0) - 1 - startX, rangeX * 2);
            if (endX == array.GetLength(0) - 1 && startX + endX > array.GetLength(0) - 1)
            {
                startX += (endX - startX) - (rangeX * 2);
                startX = Mathf.Max(startX, 0);

                //startX += (array.GetLength(0) - 1) - (startX + endX);
            }
        }

        Debug.Log($"startX:{startX} / endX:{endX} / startY:{startY} / endY:{endY} / X Length : {array.GetLength(0) } / Y Length : {array.GetLength(1) }");

        //// 내 캐릭터 선택창 지워준다
        //if (MySkillTarget != null && MySkillTarget.objects != null)
        //{
        //    MySkillTarget.objects.Clear();
        //    MySkillTarget.objects.Add((BaseController)Managers.Object.MyPlayer);

        //}

        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                if (i == centerX && j == centerY)
                {
                    text += "<color=red>■</color>";
                }
                else
                {
                    //text += array[i, j] ? "■" : "□";

                    if (array[i, j] == true)
                        text += "■";
                    else
                    {
                        if (portals[i, j] == true)
                            text += "<color=#44d2ba>■</color>";
                        else
                        {
                            if(objects[i,j] !=null)
                            {

                                BaseController A = null;
                                A = objects[i, j].GetComponent<BaseController>();

                                //// 내 캐릭터 선택창에 넣어준다.
                                //if (MySkillTarget != null && MySkillTarget.objects != null)
                                //    MySkillTarget.objects.Add(A);

                                string colorString = "";
                                if (A.GetType() == typeof(NpcController))
                                {
                                    colorString = "yellow";
                                }
                                else if (A.GetType() == typeof(PlayerController))
                                {
                                    colorString = "orange";
                                }
                                else if (A.GetType() == typeof(MonsterController))
                                {
                                    colorString = "white";
                                }
                                else
                                {
                                    colorString = "white";
                                }

                                text += $"<color={colorString}>■</color>";
                            }
                            else
                                text += "□";
                        }
                    }




                }
            }

            // 각 행의 끝에 개행 문자 추가
            text += "\n";
        }

        return text;
    }

}
