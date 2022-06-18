using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpBar : MonoBehaviour
{

    [SerializeField]
    Transform _mpBar = null;

    public void SetMpBar (float ratio)
    {
        // 0과 1사이에 두는거임
        ratio = Mathf.Clamp(ratio, 0, 1);
        _mpBar.localScale = new Vector3(ratio, 1, 1);
        
    }
}
