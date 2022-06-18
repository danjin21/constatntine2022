using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DanUIText : Text
{
    [SerializeField] private bool m_DisableWordWrap;

    public override string text
    {
        get => base.text;
        set
        {
            if(m_DisableWordWrap)
            {
                string nsbp = value.Replace(' ', '\u00A0');
                base.text = nsbp;
                return;
            }
            base.text = value;
        }
    }

}


