using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// 예시
// 새롭게 추가된 데이터 클래스
public class CustomData
{
    int sortingOrder;
    Vector3Int cellPos;

    public int SortingOrder
    {
        get { return sortingOrder; }
        set { sortingOrder = value; }
    }

    public Vector3Int CellPos
    {
        get { return cellPos; }
        set { cellPos = value; }
    }

}

public class TileMapLayer_Dan : MonoBehaviour
{
    Tilemap tileMap;
    TilemapRenderer tileMapRenderer;

    Vector3 localPos; // 로컬 포지션
    Vector3 worldPos; // 월드 포지션

    [SerializeField]
    public Dictionary<Vector3Int, CustomData> dataOnTiles = new Dictionary<Vector3Int, CustomData>(); // 타일의 데이터

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
        tileMapRenderer = GetComponent<TilemapRenderer>();

        Debug.Log("wow");

        Vector3Int worldToCellPos = tileMap.WorldToCell(worldPos);
        Vector3Int localToCellPos = tileMap.LocalToCell(localPos);

        foreach(Vector3Int pos in tileMap.cellBounds.allPositionsWithin)
        {
            // 해당 좌표에 타일이 없으면 넘어간다.
            if (!tileMap.HasTile(pos)) continue;
            // 해당 좌표의 타일을 얻는다.
            var tile = tileMap.GetTile<TileBase>(pos);

            var go = tileMap.GetInstantiatedObject(pos);

            Debug.Log($"{tile} / {go}");

            // 정보 초기화
            dataOnTiles[pos] = new CustomData()
            {
                SortingOrder = 0,
                CellPos = pos
            };

            // tile.GetTileData(pos, tileMap);

        }

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
