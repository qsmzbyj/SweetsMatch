/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///游戏管理
///<summary>
public class GameManager : MonoBehaviour
{

    public GameObject Grid;
    //填充时间
    [Range(0, 1)]
    public float filledTime;
    //单例
    //private GameManager instance;
    //public GameManager Instance { get => instance; set => instance = value; }

    //场景的行数与列数
    public int row;
    public int column;

    /// <summary>
    /// 甜品的成员变量
    /// </summary>
    #region
    //甜品
    private SweetControl[,] sweets;

    //甜品的种类枚举
    public enum SweetType
    {
        Empty,
        Normal,
        Barrier,
        Row_Clear,
        Column_Clear,
        RainbowCandy,
        Count //标记类型
    }
    //甜品字典
    public Dictionary<SweetType, GameObject> sweetPrefabDic;

    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType type;
        public GameObject sweetPrefab;
    }
    public SweetPrefab[] sweetPrefabs;

    //鼠标点击甜品的状态
    private SweetControl pressedSweet;
    private SweetControl enteredSweet;
    #endregion

    *//*private void Awake()
    {
        instance = this;
    }*//*

    private void Start()
    {
        //字典初始化
        sweetPrefabDic = new Dictionary<SweetType, GameObject>();
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            if (!sweetPrefabDic.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDic.Add(sweetPrefabs[i].type, sweetPrefabs[i].sweetPrefab);
            }
        }
        //创建背景方块
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                GameObject grid = Instantiate(Grid, CorrectPosition(r, c), Quaternion.identity);
                grid.transform.SetParent(transform);
            }
        }
        //创建甜品
        sweets = new SweetControl[row, column];
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                CreatNewSweet(r, c, SweetType.Empty);
            }
        }
        //创建障碍物
        //Destroy(sweets[4, 4].gameObject);
        // CreatNewSweet(4, 4, SweetType.Barrier);


        StartCoroutine(SweetAllFilled());
    }
    /// <summary>
    /// 纠正方块位置
    /// 横坐标为GameManager的x坐标-列数的一半+列对应的y坐标
    /// 纵坐标为GameManager的y坐标+行数的一半-行对应的x坐标
    /// GameManager的偏差值为(0.45,-0.5)
    /// </summary>
    /// <param name="r">行</param>
    /// <param name="c">列</param>
    /// <returns></returns>
    public Vector3 CorrectPosition(int r, int c)
    {
        return new Vector3(transform.position.x + c - column / 2, transform.position.y - r + row / 2);
    }
    /// <summary>
    /// 创建新甜品
    /// </summary>
    /// <param name="x">新甜品的x轴</param>
    /// <param name="y">新甜品的y轴</param>
    /// <param name="type">新甜品的类型</param>
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
    /// 将空白部分全部填满
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
            //清除所有的已匹配好的甜品
            needFill = ClearAllMatchedSweets();
        }
    }
    /// <summary>
    /// 分布填充空白区域
    /// </summary>
    public bool FilledSweet()
    {
        bool isNotFinished = false;
        //空白上方的甜品下移
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
                    else //斜向填充
                    {
                        //左下方与右下方
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int ydown = y + down;
                                //不超出边界
                                if (ydown >= 0 && ydown < column)
                                {
                                    SweetControl downSweet = sweets[x + 1, ydown];

                                    if (downSweet.Type == SweetType.Empty)
                                    {
                                        bool canVerticalFill = true;

                                        //检测是否可以使用竖向填充
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
                                        //是否斜向填充
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
        //第0层空白的填充
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
    /// 检查是否相邻
    /// </summary>
    /// <param name="sweet1">第一次点击的甜品</param>
    /// <param name="sweet2">准备交换的甜品</param>
    public bool Adjacent(SweetControl sweet1, SweetControl sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }
    /// <summary>
    /// 交换甜品
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
    /// 鼠标操作甜品
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
    /// 匹配甜品
    /// </summary>
    public List<SweetControl> Match(SweetControl sweet, int newX, int newY)
    {
        if (sweet.CanColor())
        {

            SweetColor.SweetColorType color = sweet.ColoredComponent.ColorType;
            List<SweetControl> matchedRowSweets = new List<SweetControl>();
            List<SweetControl> matchedColumnSweets = new List<SweetControl>();
            List<SweetControl> matchedFinished = new List<SweetControl>();

            //行匹配
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
            //L T 型匹配
            //检测当前行遍历列表中的元素数量是否大于3
            if (matchedRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchedRowSweets.Count; i++)
                {
                    matchedFinished.Add(matchedRowSweets[i]);
                }
                for (int c = 0; c < matchedRowSweets.Count; c++)
                {
                    // 行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    //0为上 1为下
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

            //列匹配
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
            //TL型匹配
            //检测当前列遍历列表中的元素数量是否大于3
            if (matchedColumnSweets.Count >= 3)
            {
                for (int i = 0; i < matchedColumnSweets.Count; i++)
                {
                    matchedFinished.Add(matchedColumnSweets[i]);
                }

                int y;
                for (int c = 0; c < matchedColumnSweets.Count; c++)
                {
                    //列匹配列表中满足匹配条件的每个元素左右依次进行列遍历
                    //0为左 1为右
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
    /// 清除甜品
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
    /// 清除所有可清除的甜品
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