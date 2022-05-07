/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///��Ϸ����
///<summary>
public class GameManager : MonoBehaviour
{

    public GameObject Grid;
    //���ʱ��
    [Range(0, 1)]
    public float filledTime;
    //����
    //private GameManager instance;
    //public GameManager Instance { get => instance; set => instance = value; }

    //����������������
    public int row;
    public int column;

    /// <summary>
    /// ��Ʒ�ĳ�Ա����
    /// </summary>
    #region
    //��Ʒ
    private SweetControl[,] sweets;

    //��Ʒ������ö��
    public enum SweetType
    {
        Empty,
        Normal,
        Barrier,
        Row_Clear,
        Column_Clear,
        RainbowCandy,
        Count //�������
    }
    //��Ʒ�ֵ�
    public Dictionary<SweetType, GameObject> sweetPrefabDic;

    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType type;
        public GameObject sweetPrefab;
    }
    public SweetPrefab[] sweetPrefabs;

    //�������Ʒ��״̬
    private SweetControl pressedSweet;
    private SweetControl enteredSweet;
    #endregion

    *//*private void Awake()
    {
        instance = this;
    }*//*

    private void Start()
    {
        //�ֵ��ʼ��
        sweetPrefabDic = new Dictionary<SweetType, GameObject>();
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            if (!sweetPrefabDic.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDic.Add(sweetPrefabs[i].type, sweetPrefabs[i].sweetPrefab);
            }
        }
        //������������
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                GameObject grid = Instantiate(Grid, CorrectPosition(r, c), Quaternion.identity);
                grid.transform.SetParent(transform);
            }
        }
        //������Ʒ
        sweets = new SweetControl[row, column];
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                CreatNewSweet(r, c, SweetType.Empty);
            }
        }
        //�����ϰ���
        //Destroy(sweets[4, 4].gameObject);
        // CreatNewSweet(4, 4, SweetType.Barrier);


        StartCoroutine(SweetAllFilled());
    }
    /// <summary>
    /// ��������λ��
    /// ������ΪGameManager��x����-������һ��+�ж�Ӧ��y����
    /// ������ΪGameManager��y����+������һ��-�ж�Ӧ��x����
    /// GameManager��ƫ��ֵΪ(0.45,-0.5)
    /// </summary>
    /// <param name="r">��</param>
    /// <param name="c">��</param>
    /// <returns></returns>
    public Vector3 CorrectPosition(int r, int c)
    {
        return new Vector3(transform.position.x + c - column / 2, transform.position.y - r + row / 2);
    }
    /// <summary>
    /// ��������Ʒ
    /// </summary>
    /// <param name="x">����Ʒ��x��</param>
    /// <param name="y">����Ʒ��y��</param>
    /// <param name="type">����Ʒ������</param>
    public SweetControl CreatNewSweet(int x, int y, SweetType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDic[type], CorrectPosition(x, y), Quaternion.identity);
        newSweet.transform.parent = transform;
        sweets[x, y] = newSweet.GetComponent<SweetControl>();
        sweets[x, y].Init(x, y, type, this);
        //sweet[x, y].ColoredComponent.SetColor((SweetColor.SweetColorType)Random.Range(0, 6));
        return sweets[x, y];
    }
    /// <summary>
    /// ���հײ���ȫ������
    /// </summary>
    public IEnumerator SweetAllFilled()
    {
        bool needFill = true;
        while (needFill)
        {
            yield return new WaitForSeconds(filledTime);
            while (FilledSweet())
            {
                yield return new WaitForSeconds(filledTime);
            }
            //������е���ƥ��õ���Ʒ
            needFill = ClearAllMatchedSweets();
        }
    }
    /// <summary>
    /// �ֲ����հ�����
    /// </summary>
    public bool FilledSweet()
    {
        bool isNotFinished = false;
        //�հ��Ϸ�����Ʒ����
        for (int x = row - 2; x >= 0; x--)
        {
            for (int y = 0; y < column; y++)
            {
                SweetControl sweet = sweets[x, y];
                if (sweet.CanMove())
                {
                    SweetControl sweetBelow = sweets[x + 1, y];
                    if (sweetBelow.Type == SweetType.Empty)
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MoveComponent.Move(x + 1, y, filledTime);
                        sweets[x + 1, y] = sweet;
                        CreatNewSweet(x, y, SweetType.Empty);
                        isNotFinished = true;
                    }
                    else //б�����
                    {
                        //���·������·�
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int ydown = y + down;
                                //�������߽�
                                if (ydown >= 0 && ydown < column)
                                {
                                    SweetControl downSweet = sweets[x + 1, ydown];

                                    if (downSweet.Type == SweetType.Empty)
                                    {
                                        bool canVerticalFill = true;

                                        //����Ƿ����ʹ���������
                                        for (int abovex = x; abovex >= 0; abovex--)
                                        {
                                            SweetControl aboveSweet = sweets[abovex, ydown];
                                            if (aboveSweet.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!aboveSweet.CanMove() && aboveSweet.Type != SweetType.Empty)
                                            {
                                                canVerticalFill = false;
                                                break;
                                            }
                                        }
                                        //�Ƿ�б�����
                                        if (!canVerticalFill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MoveComponent.Move(x + 1, ydown, filledTime);
                                            sweets[x + 1, ydown] = sweet;
                                            CreatNewSweet(x, y, SweetType.Empty);
                                            isNotFinished = true;
                                            break;
                                        }
                                        else break;
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        //��0��հ׵����
        for (int i = 0; i < column; i++)
        {
            SweetControl sweet = sweets[0, i];
            if (sweet.Type == SweetType.Empty)
            {
                GameObject newSweet = Instantiate(sweetPrefabDic[SweetType.Normal], CorrectPosition(-1, i), Quaternion.identity);
                newSweet.transform.SetParent(transform);
                Destroy(sweets[0, i].gameObject);

                sweets[0, i] = newSweet.GetComponent<SweetControl>();
                sweets[0, i].Init(-1, i, SweetType.Normal, this);
                sweets[0, i].MoveComponent.Move(0, i, filledTime);
                sweets[0, i].ColoredComponent.SetColor((SweetColor.SweetColorType)Random.Range(0, 6));
                isNotFinished = true;
            }
        }
        return isNotFinished;
    }
    /// <summary>
    /// ����Ƿ�����
    /// </summary>
    /// <param name="sweet1">��һ�ε������Ʒ</param>
    /// <param name="sweet2">׼����������Ʒ</param>
    public bool Adjacent(SweetControl sweet1, SweetControl sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }
    /// <summary>
    /// ������Ʒ
    /// </summary>
    public void ExchangeSweet(SweetControl sweet1, SweetControl sweet2)
    {
        if (Adjacent(sweet1, sweet2))
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            if (Match(sweet1, sweet2.X, sweet2.Y) != null || Match(sweet2, sweet1.X, sweet1.Y) != null)
            {
                int tempx = sweet1.X;
                int tempy = sweet1.Y;

                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, filledTime);
                sweet2.MoveComponent.Move(tempx, tempy, filledTime);

                ClearAllMatchedSweets();
                StartCoroutine(SweetAllFilled());
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet1.X, sweet2.Y] = sweet2;
            }
        }
    }
    /// <summary>
    /// ��������Ʒ
    /// </summary>
    #region
    public void PressedSweet(SweetControl sweet)
    {
        pressedSweet = sweet;
    }
    public void EnteredSweet(SweetControl sweet)
    {
        enteredSweet = sweet;
    }
    public void ReleasedSweet()
    {
        if (Adjacent(pressedSweet, enteredSweet))
        {
            ExchangeSweet(pressedSweet, enteredSweet);
        }
    }
    #endregion
    /// <summary>
    /// ƥ����Ʒ
    /// </summary>
    public List<SweetControl> Match(SweetControl sweet, int newX, int newY)
    {
        if (sweet.CanColor())
        {

            SweetColor.SweetColorType color = sweet.ColoredComponent.ColorType;
            List<SweetControl> matchedRowSweets = new List<SweetControl>();
            List<SweetControl> matchedColumnSweets = new List<SweetControl>();
            List<SweetControl> matchedFinished = new List<SweetControl>();

            //��ƥ��
            matchedRowSweets.Add(sweet);
            for (int i = 0; i <= 1; i++)
            {
                for (int xDistance = 1; xDistance < column; xDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        y = newY - xDistance;
                    }
                    else
                    {
                        y = newY + xDistance;
                    }
                    if (y < 0 || y >= column)
                    {
                        break;
                    }
                    if (sweets[newX, y].CanColor() && sweets[newX, y].ColoredComponent.ColorType == color)
                    {
                        matchedRowSweets.Add(sweets[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //L T ��ƥ��
            //��⵱ǰ�б����б��е�Ԫ�������Ƿ����3
            if (matchedRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchedRowSweets.Count; i++)
                {
                    matchedFinished.Add(matchedRowSweets[i]);
                }
                for (int c = 0; c < matchedRowSweets.Count; c++)
                {
                    // ��ƥ���б�������ƥ��������ÿ��Ԫ���������ν����б���
                    //0Ϊ�� 1Ϊ��
                    for (int i = 0; i <= 1; i++)
                    {
                        for (int yDistance = 1; yDistance < row; yDistance++)
                        {
                            int x;
                            if (i == 0)
                            {
                                x = newX - yDistance;
                            }
                            else
                            {
                                x = newX + yDistance;
                            }
                            if (x < 0 || x >= row)
                            {
                                break;
                            }
                            if (sweets[x, matchedRowSweets[i].Y].CanColor() && sweets[x, matchedRowSweets[i].Y].ColoredComponent.ColorType == color)
                            {
                                matchedColumnSweets.Add(sweets[x, matchedRowSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchedColumnSweets.Count < 2)
                    {
                        matchedColumnSweets.Clear();
                    }
                    else
                    {
                        for (int i = 0; i < matchedColumnSweets.Count; i++)
                        {
                            matchedFinished.Add(matchedColumnSweets[i]);
                        }
                        break;
                    }
                }

            }

            if (matchedFinished.Count >= 3)
            {
                return matchedFinished;
            }

            matchedRowSweets.Clear();
            matchedFinished.Clear();

            //��ƥ��
            matchedColumnSweets.Add(sweet);
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < column; yDistance++)
                {
                    int x;
                    if (i == 0)
                    {
                        x = newX - yDistance;
                    }
                    else
                    {
                        x = newX + yDistance;
                    }
                    if (x < 0 || x >= row)
                    {
                        break;
                    }
                    if (sweets[x, newY].CanColor() && sweets[x, newY].ColoredComponent.ColorType == color)
                    {
                        matchedColumnSweets.Add(sweets[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //TL��ƥ��
            //��⵱ǰ�б����б��е�Ԫ�������Ƿ����3
            if (matchedColumnSweets.Count >= 3)
            {
                for (int i = 0; i < matchedColumnSweets.Count; i++)
                {
                    matchedFinished.Add(matchedColumnSweets[i]);
                }

                int y;
                for (int c = 0; c < matchedColumnSweets.Count; c++)
                {
                    //��ƥ���б�������ƥ��������ÿ��Ԫ���������ν����б���
                    //0Ϊ�� 1Ϊ��
                    for (int i = 0; i <= 1; i++)
                    {
                        for (int xDistance = 1; xDistance < column; xDistance++)
                        {
                            if (i == 0)
                            {
                                y = newY - xDistance;
                            }
                            else
                            {
                                y = newY + xDistance;
                            }
                            if (y < 0 || y >= column)
                            {
                                break;
                            }
                            if (sweets[matchedColumnSweets[i].X, y].CanColor() && sweets[matchedColumnSweets[i].X, y].ColoredComponent.ColorType == color)
                            {
                                matchedRowSweets.Add(sweets[matchedColumnSweets[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchedRowSweets.Count < 2)
                    {
                        matchedRowSweets.Clear();
                    }
                    else
                    {
                        for (int i = 0; i < matchedRowSweets.Count; i++)
                        {
                            matchedFinished.Add(matchedRowSweets[i]);
                        }
                        break;
                    }
                }
            }
            if (matchedFinished.Count >= 3)
            {
                return matchedFinished;
            }
        }
        return null;
    }
    /// <summary>
    /// �����Ʒ
    /// </summary>
    public bool ClearSweet(int x, int y)
    {
        if (sweets[x, y].CanClear() && !sweets[x, y].ClearComponent.IsClearing)
        {
            sweets[x, y].ClearComponent.Clear();
            CreatNewSweet(x, y, SweetType.Empty);

            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// ������п��������Ʒ
    /// </summary>
    private bool ClearAllMatchedSweets()
    {
        bool needRefill = false;
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                if (sweets[x, y].CanClear())
                {
                    List<SweetControl> matchedSweets = Match(sweets[x, y], x, y);
                    if (matchedSweets != null)
                    {
                        for (int i = 0; i < matchedSweets.Count; i++)
                        {
                            if (ClearSweet(matchedSweets[i].X, matchedSweets[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }

}
*/