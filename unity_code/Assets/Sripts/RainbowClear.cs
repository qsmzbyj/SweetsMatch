using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///²ÊºçÌÇ
///<summary>
public class RainbowClear : ClearSweets
{
    private SweetColor.SweetColorType clearColor;

    public SweetColor.SweetColorType ClearColor { get => clearColor; set => clearColor = value; }

    public override void Clear()
    {
        base.Clear();
        sweet.gameManager.ClearColor(clearColor);
    }

}
