using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CursorManager : MonoBehaviour
{
    public RectTransform transform_cursor;
    public RectTransform transform_icon;
    public Text text_mouse;
    Vector2 mousePos;





    private void Start()
    {
        Init_Cursor();
    }
    private void Update()
    {
        Update_MousePosition();
    }

    private void Init_Cursor()
    {
        Cursor.visible = false;
        transform_cursor.pivot = Vector2.up;

        if (transform_cursor.GetComponent<Graphic>())
            transform_cursor.GetComponent<Graphic>().raycastTarget = false;
        if (transform_icon.GetComponent<Graphic>())
            transform_icon.GetComponent<Graphic>().raycastTarget = false;
    }

    //CodeFinder 코드파인더
    //From https://codefinder.janndk.com/ 
    private void Update_MousePosition()
    {
        mousePos = Input.mousePosition;
        if ((Vector2)transform_cursor.position != mousePos)
        {
            transform_cursor.position = mousePos;
            float w = transform_icon.rect.width;
            float h = transform_icon.rect.height;
            transform_icon.position = transform_cursor.position + (new Vector3(-9, +6, 0));
        }

      
    }
}
