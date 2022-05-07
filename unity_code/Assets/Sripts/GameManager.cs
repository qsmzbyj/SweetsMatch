using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
///<summary>
///游戏管理
///<summary>
public class GameManager : MonoBehaviour
{
    //鼠标图标
    public Image mouse;

    //游戏时间
    public Text TimeText;
    public float gameTime = 60;

    //分数
    public Text PlayerScoreText;
    public int playerScore;
    public float currentScore;
    private float addScoreTime;
    public Text FinalScore;

    //结束的面板
    public GameObject gameoverPanel;

    //背景方块
    public GameObject Grid;

    //游戏状态
    private bool gameover = false;
    //填充时间
    [Range(0, 1)]
    public float filledTime;
    // 单例
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; set => _instance = value; }

    //场景的行数与列数
    public int yRow;
    public int xColumn;

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
        COUNT //标记类型
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

    private void Awake()
    {
        _instance = this;
    }

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
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                GameObject grid = Instantiate(Grid, CorrectPosition(x, y), Quaternion.identity);
                grid.transform.SetParent(transform);
            }
        }
        //创建甜品
        sweets = new SweetControl[xColumn, yRow];
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                CreateNewSweet(x, y, SweetType.Empty);
            }
        }

        CreateNewBarrier(4, 4);
        CreateNewBarrier(4, 3);
        CreateNewBarrier(1, 1);
        CreateNewBarrier(1, 6);
        CreateNewBarrier(7, 6);
        CreateNewBarrier(7, 1);

        StartCoroutine(SweetAllFilled());
    }
    private void Update()
    {
        //鼠标图标替换(位置)
        mouse.rectTransform.position = Input.mousePosition;

        gameTime -= Time.deltaTime;

        if (gameTime < 0)
        {
            gameTime = 0;
            //弹出结束场景
            //游戏结束
            FindObjectOfType<Canvas>().GetComponent<AudioSource>().volume = 0.05f;
            gameover = true;
            FinalScore.text = string.Format("{0}", playerScore);
            gameoverPanel.SetActive(true);
        }
        //倒计时
        TimeText.text = string.Format("{0:0}", gameTime);

        if (addScoreTime <= 0.05f)
        {
            addScoreTime += Time.deltaTime;
        }
        else
        {
            if (currentScore < playerScore)
            {
                currentScore++;
                PlayerScoreText.text = string.Format("{0}", currentScore);
                addScoreTime = 0;
            }
        }
    }

    /// <summary>
    /// 创建障碍物
    /// </summary>
    private void CreateNewBarrier(int x, int y)
    {
        Destroy(sweets[x, y].gameObject);
        CreateNewSweet(x, y, SweetType.Barrier);

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
    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2f - y);
    }
    /// <summary>
    /// 创建新甜品
    /// </summary>
    /// <param name="x">新甜品的x轴</param>
    /// <param name="y">新甜品的y轴</param>
    /// <param name="type">新甜品的类型</param>
    public SweetControl CreateNewSweet(int x, int y, SweetType type)
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
        bool isNotFinished = false;//判断本次填充是否完成
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                SweetControl sweet = sweets[x, y];//得到当前元素位置的甜品对象

                if (sweet.CanMove())//如果无法移动，则无法往下填充 
                {
                    SweetControl sweetBelow = sweets[x, y + 1];

                    if (sweetBelow.Type == SweetType.Empty)//垂直填充
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MoveComponent.Move(x, y + 1, filledTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetType.Empty);
                        isNotFinished = true;
                    }
                    else         //斜向填充
                    {
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = x + down;

                                if (downX >= 0 && downX < xColumn)
                                {
                                    SweetControl downSweet = sweets[downX, y + 1];

                                    if (downSweet.Type == SweetType.Empty)
                                    {
                                        bool canfill = true;//用来判断垂直填充是否可以满足填充要求

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            SweetControl sweetAbove = sweets[downX, aboveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetType.Empty)
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }

                                        if (!canfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MoveComponent.Move(downX, y + 1, filledTime);
                                            sweets[downX, y + 1] = sweet;
                                            CreateNewSweet(x, y, SweetType.Empty);
                                            isNotFinished = true;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

            }
        }
        //第0层空白的填充
        for (int x = 0; x < xColumn; x++)
        {
            SweetControl sweet = sweets[x, 0];

            if (sweet.Type == SweetType.Empty)
            {
                Destroy(sweets[x, 0].gameObject);
                GameObject newSweet = Instantiate(sweetPrefabDic[SweetType.Normal], CorrectPosition(x, -1), Quaternion.identity);
                newSweet.transform.parent = transform;

                sweets[x, 0] = newSweet.GetComponent<SweetControl>();
                sweets[x, 0].Init(x, -1, SweetType.Normal, this);
                sweets[x, 0].MoveComponent.Move(x, 0, filledTime);
                sweets[x, 0].ColoredComponent.SetColor((SweetColor.SweetColorType)Random.Range(0, sweets[x, 0].ColoredComponent.ColorNums));
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

            if (Match(sweet1, sweet2.X, sweet2.Y) != null || Match(sweet2, sweet1.X, sweet1.Y) != null|| sweet1.Type == SweetType.RainbowCandy|| sweet2.Type == SweetType.RainbowCandy)
            {
                int tempx = sweet1.X;
                int tempy = sweet1.Y;

                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, filledTime);
                sweet2.MoveComponent.Move(tempx, tempy, filledTime);
                
                //彩虹糖的交换
                if (sweet1.Type == SweetType.RainbowCandy && sweet1.CanClear() && sweet2.CanClear())
                {
                    RainbowClear rainbowColor = sweet1.GetComponent<RainbowClear>();

                    if (rainbowColor != null)
                    {
                        rainbowColor.ClearColor = sweet2.ColoredComponent.ColorType;
                    }

                    ClearSweet(sweet1.X, sweet1.Y);
                
                }
                if (sweet2.Type == SweetType.RainbowCandy && sweet2.CanClear() && sweet1.CanClear())
                {
                    RainbowClear rainbowColor = sweet2.GetComponent<RainbowClear>();

                    if (rainbowColor != null)
                    {
                        rainbowColor.ClearColor = sweet1.ColoredComponent.ColorType;
                    }

                    ClearSweet(sweet2.X, sweet2.Y);

                }

                ClearAllMatchedSweets();
                StartCoroutine(SweetAllFilled());

                pressedSweet = null;
                enteredSweet = null;
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet1.X, sweet2.Y] = sweet2;
            }
        }
    }
    // 鼠标操作甜品
    #region
    public void PressedSweet(SweetControl sweet)
    {
        if (gameover)
        {
            return;
        }
        pressedSweet = sweet;
    }
    public void EnteredSweet(SweetControl sweet)
    {
        if (gameover)
        {
            return;
        }
        enteredSweet = sweet;
    }
    public void ReleasedSweet()
    {
        if (gameover)
        {
            return;
        }
        if (pressedSweet != null && enteredSweet != null && !pressedSweet.ClearComponent.IsClearing && !enteredSweet.ClearComponent.IsClearing)
        {
            if (Adjacent(pressedSweet, enteredSweet))
            {
                ExchangeSweet(pressedSweet, enteredSweet);
            }
        }
    }
    #endregion

    /// <summary>
    /// 匹配甜品
    /// </summary>
    private List<SweetControl> Match(SweetControl piece, int newX, int newY)
    {
        //判断是否是需要匹配的甜品 
        if (!piece.CanColor()) return null;
        var color = piece.ColoredComponent.ColorType;
        var matchRowSweet = new List<SweetControl>();
        var matchColumnSweet = new List<SweetControl>();
        var FinishedMatch = new List<SweetControl>();

        // 行匹配
        matchRowSweet.Add(piece);
        //i=0代表往左，i=1代表往右
        for (int i = 0; i <= 1; i++)
        {
            for (int xOffset = 1; xOffset < xColumn; xOffset++)
            {
                int x;

                if (i == 0)
                {
                    x = newX - xOffset;
                }
                else
                {
                    x = newX + xOffset;
                }

                // 超出边界返回
                if (x < 0 || x >= xColumn) { break; }

                // 是否是相同颜色
                if (sweets[x, newY].CanColor() && sweets[x, newY].ColoredComponent.ColorType == color)
                {
                    matchRowSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }
        }

        if (matchRowSweet.Count >= 3)
        {
            for (int i = 0; i < matchRowSweet.Count; i++)
            {
                FinishedMatch.Add(matchRowSweet[i]);
            }
        }

        //L T型匹配
        //检查一下当前行遍历列表中的元素数量是否大于3
        if (matchRowSweet.Count >= 3)
        {
            for (int i = 0; i < matchRowSweet.Count; i++)
            {
                //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                // 0代表上方 1代表下方
                for (int dir = 0; dir <= 1; dir++)
                {
                    for (int yOffset = 1; yOffset < yRow; yOffset++)
                    {
                        int y;

                        if (dir == 0)
                        {
                            y = newY - yOffset;
                        }
                        else
                        {
                            y = newY + yOffset;
                        }

                        if (y < 0 || y >= yRow)
                        {
                            break;
                        }

                        if (sweets[matchRowSweet[i].X, y].CanColor() && sweets[matchRowSweet[i].X, y].ColoredComponent.ColorType == color)
                        {
                            matchColumnSweet.Add(sweets[matchRowSweet[i].X, y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (matchColumnSweet.Count < 2)
                {
                    matchColumnSweet.Clear();
                }
                else
                {
                    for (int j = 0; j < matchColumnSweet.Count; j++)
                    {
                        FinishedMatch.Add(matchColumnSweet[j]);
                    }
                    break;
                }
            }
        }

        if (FinishedMatch.Count >= 3)
        {
            return FinishedMatch;
        }


        //行匹配结束后，重置行 列 列表
        matchRowSweet.Clear();
        matchColumnSweet.Clear();
        matchColumnSweet.Add(piece);

        //列匹配

        //i=0代表往上，i=1代表往下
        for (int i = 0; i <= 1; i++)
        {
            for (int yOffset = 1; yOffset < xColumn; yOffset++)
            {
                int y;

                if (i == 0)
                {
                    y = newY - yOffset;
                }
                else
                {
                    y = newY + yOffset;
                }

                // 超出边界
                if (y < 0 || y >= yRow) { break; }

                // 甜品是否是相同颜色
                if (sweets[newX, y].CanColor() && sweets[newX, y].ColoredComponent.ColorType == color)
                {
                    matchColumnSweet.Add(sweets[newX, y]);
                }
                else
                {
                    break;
                }
            }
        }

        if (matchColumnSweet.Count >= 3)
        {
            for (int i = 0; i < matchColumnSweet.Count; i++)
            {
                FinishedMatch.Add(matchColumnSweet[i]);
            }
        }

        //L T型匹配
        //检查一下当前行遍历列表中的元素数量是否大于3
        if (matchColumnSweet.Count >= 3)
        {
            for (int i = 0; i < matchColumnSweet.Count; i++)
            {
                //列匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                // 0代表左方 1代表右方
                for (int dir = 0; dir <= 1; dir++)
                {
                    for (int xOffset = 1; xOffset < yRow; xOffset++)
                    {
                        int x;

                        if (dir == 0)
                        {
                            x = newX - xOffset;
                        }
                        else
                        {
                            x = newX + xOffset;
                        }

                        if (x < 0 || x >= xColumn)
                        {
                            break;
                        }

                        if (sweets[x, matchColumnSweet[i].Y].CanColor() && sweets[x, matchColumnSweet[i].Y].ColoredComponent.ColorType == color)
                        {
                            matchRowSweet.Add(sweets[x, matchColumnSweet[i].Y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (matchRowSweet.Count < 2)
                {
                    matchRowSweet.Clear();
                }
                else
                {
                    for (int j = 0; j < matchRowSweet.Count; j++)
                    {
                        FinishedMatch.Add(matchRowSweet[j]);
                    }
                    break;
                }
            }
        }

        if (FinishedMatch.Count >= 3)
        {
            return FinishedMatch;
        }

        return null;
    }
    /// <summary>
    /// 清除甜品
    /// </summary>
    private bool ClearSweet(int x, int y)
    {
        if (sweets[x, y].CanClear() && !sweets[x, y].ClearComponent.IsClearing)
        {
            sweets[x, y].ClearComponent.Clear();
            CreateNewSweet(x, y, SweetType.Empty);
            ClearBarrier(x, y);
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
        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xColumn; x++)
            {
                if (sweets[x, y].CanClear())
                {
                    List<SweetControl> matchedSweets = Match(sweets[x, y], x, y);
                    if (matchedSweets != null)
                    {
                        
                        SweetType specialSweetType = SweetType.COUNT;//是否产生特殊甜品
                        SweetControl randomSweet = matchedSweets[Random.Range(0, matchedSweets.Count)];
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;

                        if (matchedSweets.Count == 4)
                        {
                            if (pressedSweet == null || enteredSweet == null)
                            {
                                specialSweetType = (SweetType)Random.Range((int)SweetType.Row_Clear, (int)SweetType.Column_Clear+1);
                            }
                            else if (pressedSweet.X == enteredSweet.X)
                            {
                                specialSweetType = SweetType.Row_Clear;
                            }
                            else
                            {
                                specialSweetType = SweetType.Column_Clear;
                            }
                        }
                        //5个消除 就产生彩虹糖
                        else if (matchedSweets.Count>=5)
                        {
                            specialSweetType = SweetType.RainbowCandy;
                        }

                        for (int i = 0; i < matchedSweets.Count; i++)
                        {
                            if (ClearSweet(matchedSweets[i].X, matchedSweets[i].Y))
                            {
                                needRefill = true;
                            }
                            if (matchedSweets[i] != pressedSweet && matchedSweets[i] != enteredSweet) continue;

                            specialSweetX = matchedSweets[i].X;
                            specialSweetY = matchedSweets[i].Y;
                        }

                        //生成特殊甜品
                        if (specialSweetType != SweetType.COUNT)
                        {
                            Destroy(sweets[specialSweetX, specialSweetY]);
                            SweetControl newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetType);
                            //整行 整列 的消除的甜品颜色
                            if (specialSweetType == SweetType.Row_Clear || specialSweetType == SweetType.Column_Clear && newSweet.CanColor() && matchedSweets[0].CanColor())
                            {
                                newSweet.ColoredComponent.SetColor(matchedSweets[0].ColoredComponent.ColorType);
                            }
                            //彩虹糖
                            else if(specialSweetType==SweetType.RainbowCandy&&newSweet.CanColor())
                            {
                                newSweet.ColoredComponent.SetColor(SweetColor.SweetColorType.Rainbow);
                            }
                        }

                    }
                }
            }
        }
        return needRefill;
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void ReloadGameScene()
    {
        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// 清除障碍物
    /// </summary>
    private void ClearBarrier(int x, int y)
    {
        for (int friendX = x - 1; friendX <= x + 1; friendX++)
        {
            if (friendX != x && friendX >= 0 && friendX < xColumn)
            {
                if (sweets[friendX, y].CanClear() && sweets[friendX, y].Type == SweetType.Barrier)
                {
                    sweets[friendX, y].ClearComponent.Clear();
                    CreateNewSweet(friendX, y, SweetType.Empty);
                }
            }
        }
        for (int friendy = y - 1; friendy <= y + 1; friendy++)
        {
            if (friendy != y && friendy >= 0 && friendy < yRow)
            {
                if (sweets[x, friendy].CanClear() && sweets[x, friendy].Type == SweetType.Barrier)
                {
                    sweets[x, friendy].ClearComponent.Clear();
                    CreateNewSweet(x, friendy, SweetType.Empty);
                }
            }
        }
    }
    /// <summary>
    /// 清除整行
    /// </summary>
    public void RowClear(int row)
    {
        for (int x = 0; x < xColumn; x++)
        {
            ClearSweet(x, row);
        }
    }
    /// <summary>
    /// 清除整列
    /// </summary>
    public void ColumnClear(int column)
    {
        for (int y = 0; y < yRow; y++)
        {
            ClearSweet(column, y);
        }
    }
    /// <summary>
    /// 清除颜色的方法
    /// </summary>
    public void ClearColor(SweetColor.SweetColorType colorType)
    {
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                if ((sweets[x, y].CanColor() && sweets[x, y].ColoredComponent.ColorType == colorType )|| (colorType==SweetColor.SweetColorType.Rainbow))
                {
                    ClearSweet(x, y);
                }
            }
        }
    }
}
