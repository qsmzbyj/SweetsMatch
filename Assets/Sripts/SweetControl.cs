using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///�Զ����sweet
///<summary>
public class SweetControl : MonoBehaviour
{
    /// <summary>
    /// ������
    /// </summary>
    private ClearSweets clearComponent;
    public ClearSweets ClearComponent { get => clearComponent;  }

    /// <summary>
    /// �ƶ����
    /// </summary>
    public SweetMovement MoveComponent { get => moveComponent;}
    private SweetMovement moveComponent;
    /// <summary>
    /// ��ɫ�������
    /// </summary>
    public SweetColor ColoredComponent { get => coloredComponent; set => coloredComponent = value; }

    private SweetColor coloredComponent;
    /// <summary>
    /// sweet��x����y��
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
    /// ��Ʒ������
    /// </summary>
    public GameManager.SweetType Type { get => type;  }
    private GameManager.SweetType type;


    [HideInInspector]
    public GameManager gameManager;

    /// <summary>
    /// �������
    /// </summary>
    private void Awake()
    {
        moveComponent = GetComponent<SweetMovement>();
        coloredComponent = GetComponent<SweetColor>();
        clearComponent = GetComponent<ClearSweets>();
    }

    /// <summary>
    /// ��ʼ��sweet
    /// </summary>
    /// <param name="_x">x��</param>
    /// <param name="_y">y��</param>
    /// <param name="_type">��Ʒ����</param>
    /// <param name="_gameManager">��Ϸ����</param>
    public void Init(int _x,int _y,GameManager.SweetType _type,GameManager _gameManager)
    {
        x = _x;
        y = _y;
        type = _type;
        gameManager = _gameManager;
    }

    /// <summary>
    /// �жϵ�ǰsweet�Ƿ����ƶ�
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        return moveComponent != null;
    }
    /// <summary>
    /// �ж��Ƿ�����ɫ����
    /// </summary>
    public bool CanColor()
    {
        return coloredComponent != null;
    }
    /// <summary>
    /// �ж��Ƿ������
    /// </summary>
    public bool CanClear()
    {
        return clearComponent != null;
    }

    //������������Ʒ�Ľ���
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
