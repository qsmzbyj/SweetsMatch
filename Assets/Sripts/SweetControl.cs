using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///自定义的sweet
///<summary>
public class SweetControl : MonoBehaviour
{
    /// <summary>
    /// 清除组件
    /// </summary>
    private ClearSweets clearComponent;
    public ClearSweets ClearComponent { get => clearComponent;  }

    /// <summary>
    /// 移动组件
    /// </summary>
    public SweetMovement MoveComponent { get => moveComponent;}
    private SweetMovement moveComponent;
    /// <summary>
    /// 颜色种类组件
    /// </summary>
    public SweetColor ColoredComponent { get => coloredComponent; set => coloredComponent = value; }

    private SweetColor coloredComponent;
    /// <summary>
    /// sweet的x轴与y轴
    /// </summary>
    public int X
    {
        get
        {
            return x;
        }
        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }
    private int x;
    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }
    private int y;

    /// <summary>
    /// 甜品的类型
    /// </summary>
    public GameManager.SweetType Type { get => type;  }
    private GameManager.SweetType type;


    [HideInInspector]
    public GameManager gameManager;

    /// <summary>
    /// 加载组件
    /// </summary>
    private void Awake()
    {
        moveComponent = GetComponent<SweetMovement>();
        coloredComponent = GetComponent<SweetColor>();
        clearComponent = GetComponent<ClearSweets>();
    }

    /// <summary>
    /// 初始化sweet
    /// </summary>
    /// <param name="_x">x轴</param>
    /// <param name="_y">y轴</param>
    /// <param name="_type">甜品种类</param>
    /// <param name="_gameManager">游戏管理</param>
    public void Init(int _x,int _y,GameManager.SweetType _type,GameManager _gameManager)
    {
        x = _x;
        y = _y;
        type = _type;
        gameManager = _gameManager;
    }

    /// <summary>
    /// 判断当前sweet是否能移动
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        return moveComponent != null;
    }
    /// <summary>
    /// 判断是否有颜色种类
    /// </summary>
    public bool CanColor()
    {
        return coloredComponent != null;
    }
    /// <summary>
    /// 判断是否能清除
    /// </summary>
    public bool CanClear()
    {
        return clearComponent != null;
    }

    //鼠标进行相邻甜品的交换
    #region
    private void OnMouseDown()
    {
        gameManager.PressedSweet(this);
    }
    private void OnMouseEnter()
    {
        gameManager.EnteredSweet(this);
    }
    private void OnMouseUp()
    {
        gameManager.ReleasedSweet();
    }
    #endregion

}
