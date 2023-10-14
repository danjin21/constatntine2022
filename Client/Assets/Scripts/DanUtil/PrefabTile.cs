using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTile : MonoBehaviour
{

    public int padding = 0;


    // Start is called before the first frame update
    void Start()
    {
        Vector3Int destPosInt = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        Vector3Int CellPosInt = Managers.Map.CurrentGrid.WorldToCell(destPosInt);

        gameObject.GetComponent<SpriteRenderer>().sortingOrder = -(int)CellPosInt.y + padding;

        transform.position = new Vector3(transform.position.x, transform.position.y, -100);



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
