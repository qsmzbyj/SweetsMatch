using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///甜品的颜色种类组件
///<summary>
public class SweetColor : MonoBehaviour
{
    //甜品的颜色种类枚举
    public enum SweetColorType
    {
        Blue,
        Green,
        Purple,
        Yellow,
        Pink,
        Red,
        Rainbow,
        count
    }
    //甜品的字典
    public Dictionary<SweetColorType, Sprite> SweetColorDic;
    [System.Serializable]
    public struct ColorPrefab
    {
        public SweetColorType colorType;
        public Sprite colorPrefab;
    }
    public ColorPrefab[] colorPrefabs;

    //颜色种类
    private SweetColorType colorType;
    public SweetColorType ColorType { get => colorType; set => SetColor(value); }

    //渲染器
    private SpriteRenderer sprite;

    //颜色种类的数目
    public int ColorNums
    {
        get
        {
            return colorPrefabs.Length;
        }
    }
    /// <summary>
    /// 初始化甜品的字典
    /// </summary>
    private void Awake()
    {
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        SweetColorDic = new Dictionary<SweetColorType, Sprite>();
        for (int i = 0; i < ColorNums; i++)
        {
            if (!SweetColorDic.ContainsKey(colorPrefabs[i].colorType))
            {
                SweetColorDic.Add(colorPrefabs[i].colorType, colorPrefabs[i].colorPrefab);
            }
        }
    }
    /// <summary>
    /// 设置甜品的颜色种类
    /// </summary>
    /// <param name="newColor">颜色</param>
    public void SetColor(SweetColorType newColor)
    {
        colorType = newColor;
        if (SweetColorDic.ContainsKey(newColor))
        {
            sprite.sprite = SweetColorDic[newColor];
        }
    }
}
