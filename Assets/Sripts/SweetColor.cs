using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///��Ʒ����ɫ�������
///<summary>
public class SweetColor : MonoBehaviour
{
    //��Ʒ����ɫ����ö��
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
    //��Ʒ���ֵ�
    public Dictionary<SweetColorType, Sprite> SweetColorDic;
    [System.Serializable]
    public struct ColorPrefab
    {
        public SweetColorType colorType;
        public Sprite colorPrefab;
    }
    public ColorPrefab[] colorPrefabs;

    //��ɫ����
    private SweetColorType colorType;
    public SweetColorType ColorType { get => colorType; set => SetColor(value); }

    //��Ⱦ��
    private SpriteRenderer sprite;

    //��ɫ�������Ŀ
    public int ColorNums
    {
        get
        {
            return colorPrefabs.Length;
        }
    }
    /// <summary>
    /// ��ʼ����Ʒ���ֵ�
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
    /// ������Ʒ����ɫ����
    /// </summary>
    /// <param name="newColor">��ɫ</param>
    public void SetColor(SweetColorType newColor)
    {
        colorType = newColor;
        if (SweetColorDic.ContainsKey(newColor))
        {
            sprite.sprite = SweetColorDic[newColor];
        }
    }
}
