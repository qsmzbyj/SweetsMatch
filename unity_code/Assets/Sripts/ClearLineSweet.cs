using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///<summary>
public class ClearLineSweet : ClearSweets
{
    public bool isRow;

    public override void Clear()
    {
        base.Clear();

        if (isRow)
        {
            sweet.gameManager.RowClear(sweet.Y);
        }
        else
        {
            sweet.gameManager.ColumnClear(sweet.X);
        }

    }
    
}
