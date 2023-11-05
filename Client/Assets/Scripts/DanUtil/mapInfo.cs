using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class mapInfo : MonoBehaviour
{
    public int mapId;
    public string mapName;
    public string townName;

    public Vector2 cameraCenter;
    public Vector2 size;

    public int TileSize = 32;
    public int paddingX = 5; // 10
    public int paddingY = 10; // 15

    float height;
    float width;

    MyPlayerController mc;


    public void Start()
    {

        paddingX = 15;
        paddingY = 10;

        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;

        mc = Managers.Object.MyPlayer;



        // yourGameObject 불러오기
        GameObject yourGameObject = transform.GetChild(0).gameObject;


        // Transform 컴포넌트 가져오기
        Transform targetTransform = yourGameObject.transform;

        // Renderer 또는 Collider 컴포넌트 가져오기
        TilemapRenderer renderer = yourGameObject.GetComponent<TilemapRenderer>();
        Collider collider = yourGameObject.GetComponent<Collider>();

        // Renderer 또는 Collider가 없다면 오류 처리
        if (renderer == null && collider == null)
        {
            Debug.LogError("Renderer 또는 Collider 컴포넌트가 없습니다.");
            return;
        }

        // Bounds 중심 위치 구하기
        Vector3 center = renderer != null ? renderer.bounds.center : collider.bounds.center;

        // World 좌표로 변환 (로컬 좌표에서 월드 좌표로 변환)
        Vector3 centerInWorld = targetTransform.TransformPoint(center);

        // 중심 위치 사용
        Debug.Log("가운데 위치: " + centerInWorld);


        // 크기 정보를 출력하거나 다른 용도로 사용할 수 있습니다.
        Debug.Log("Object Size: " + renderer.bounds.size);

        cameraCenter = new Vector2(centerInWorld.x, centerInWorld.y);
        size = new Vector2(renderer.bounds.size.x , renderer.bounds.size.y );

        // size 최소값은 카메라 사이즈보다 크게.

        float cameraSizeX = Camera.main.orthographicSize *2* Camera.main.aspect ;

        if(size.x < cameraSizeX)
        {
            size.x = cameraSizeX;
        }

        float cameraSizeY = Camera.main.orthographicSize * 2;

        if (size.y < cameraSizeY)
        {
            size.y = cameraSizeY;
        }

        size = new Vector2(size.x + TileSize * paddingX, size.y + TileSize * paddingY);

  

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cameraCenter, size);
    }

    public void LateUpdate()
    {

        Camera.main.transform.position = new Vector3(mc.transform.position.x + 128.0f, mc.transform.position.y - 96.0f, -1000);



        //if (mc !=null)
        //{
        //    Camera.main.transform.position = new Vector3(mc.transform.position.x + 128.0f, mc.transform.position.y - 96.0f, -1000);

        //    float lx = size.x * 0.5f - width;
        //    float clampX = Mathf.Clamp(Camera.main.transform.position.x, -lx + cameraCenter.x, lx + cameraCenter.x);

        //    float ly = size.y * 0.5f - height;
        //    float clampY = Mathf.Clamp(Camera.main.transform.position.y, -ly + cameraCenter.y, ly + cameraCenter.y);


        //    Camera.main.transform.position = new Vector3((int)clampX, (int)clampY, -1000);

        //}




    }

}
