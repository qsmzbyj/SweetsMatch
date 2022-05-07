using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
///<summary>
///��Ϸ����
///<summary>
public class GameManager : MonoBehaviour
{
    //���ͼ��
    public Image mouse;

    //��Ϸʱ��
    public Text TimeText;
    public float gameTime = 60;

    //����
    public Text PlayerScoreText;
    public int playerScore;
    public float currentScore;
    private float addScoreTime;
    public Text FinalScore;

    //���������
    public GameObject gameoverPanel;

    //��������
    public GameObject Grid;

    //��Ϸ״̬
    private bool gameover = false;
    //���ʱ��
    [Range(0, 1)]
    public float filledTime;
    // ����
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; set => _instance = value; }

    //����������������
    public int yRow;
    public int xColumn;

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
        COUNT //�������
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

    private void Awake()
    {
        _instance = this;
    }

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
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                GameObject grid = Instantiate(Grid, CorrectPosition(x, y), Quaternion.identity);
                grid.transform.SetParent(transform);
            }
        }
        //������Ʒ
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
        //���ͼ���滻(λ��)
        mouse.rectTransform.position = Input.mousePosition;

        gameTime -= Time.deltaTime;

        if (gameTime < 0)
        {
            gameTime = 0;
            //������������
            //��Ϸ����
            FindObjectOfType<Canvas>().GetComponent<AudioSource>().volume = 0.05f;
            gameover = true;
            FinalScore.text = string.Format("{0}", playerScore);
            gameoverPanel.SetActive(true);
        }
        //����ʱ
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
    /// �����ϰ���
    /// </summary>
    private void CreateNewBarrier(int x, int y)
    {
        Destroy(sweets[x, y].gameObject);
        CreateNewSweet(x, y, SweetType.Barrier);

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
    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2f - y);
    }
    /// <summary>
    /// ��������Ʒ
    /// </summary>
    /// <param name="x">����Ʒ��x��</param>
    /// <param name="y">����Ʒ��y��</param>
    /// <param name="type">����Ʒ������</param>
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
        bool isNotFinished = false;//�жϱ�������Ƿ����
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                SweetControl sweet = sweets[x, y];//�õ���ǰԪ��λ�õ���Ʒ����

                if (sweet.CanMove())//����޷��ƶ������޷�������� 
                {
                    SweetControl sweetBelow = sweets[x, y + 1];

                    if (sweetBelow.Type == SweetType.Empty)//��ֱ���
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MoveComponent.Move(x, y + 1, filledTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetType.Empty);
                        isNotFinished = true;
                    }
                    else         //б�����
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
                                        bool canfill = true;//�����жϴ�ֱ����Ƿ�����������Ҫ��

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
        //��0��հ׵����
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

            if (Match(sweet1, sweet2.X, sweet2.Y) != null || Match(sweet2, sweet1.X, sweet1.Y) != null|| sweet1.Type == SweetType.RainbowCandy|| sweet2.Type == SweetType.RainbowCandy)
            {
                int tempx = sweet1.X;
                int tempy = sweet1.Y;

                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, filledTime);
                sweet2.MoveComponent.Move(tempx, tempy, filledTime);
                
                //�ʺ��ǵĽ���
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
    // ��������Ʒ
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
    /// ƥ����Ʒ
    /// </summary>
    private List<SweetControl> Match(SweetControl piece, int newX, int newY)
    {
        //�ж��Ƿ�����Ҫƥ�����Ʒ 
        if (!piece.CanColor()) return null;
        var color = piece.ColoredComponent.ColorType;
        var matchRowSweet = new List<SweetControl>();
        var matchColumnSweet = new List<SweetControl>();
        var FinishedMatch = new List<SweetControl>();

        // ��ƥ��
        matchRowSweet.Add(piece);
        //i=0��������i=1��������
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

                // �����߽緵��
                if (x < 0 || x >= xColumn) { break; }

                // �Ƿ�����ͬ��ɫ
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

        //L T��ƥ��
        //���һ�µ�ǰ�б����б��е�Ԫ�������Ƿ����3
        if (matchRowSweet.Count >= 3)
        {
            for (int i = 0; i < matchRowSweet.Count; i++)
            {
                //��ƥ���б�������ƥ��������ÿ��Ԫ���������ν����б���
                // 0�����Ϸ� 1�����·�
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


        //��ƥ������������� �� �б�
        matchRowSweet.Clear();
        matchColumnSweet.Clear();
        matchColumnSweet.Add(piece);

        //��ƥ��

        //i=0�������ϣ�i=1��������
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

                // �����߽�
                if (y < 0 || y >= yRow) { break; }

                // ��Ʒ�Ƿ�����ͬ��ɫ
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

        //L T��ƥ��
        //���һ�µ�ǰ�б����б��е�Ԫ�������Ƿ����3
        if (matchColumnSweet.Count >= 3)
        {
            for (int i = 0; i < matchColumnSweet.Count; i++)
            {
                //��ƥ���б�������ƥ��������ÿ��Ԫ���������ν����б���
                // 0������ 1�����ҷ�
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
    /// �����Ʒ
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
    /// ������п��������Ʒ
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
                        
                        SweetType specialSweetType = SweetType.COUNT;//�Ƿ����������Ʒ
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
                        //5������ �Ͳ����ʺ���
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

                        //����������Ʒ
                        if (specialSweetType != SweetType.COUNT)
                        {
                            Destroy(sweets[specialSweetX, specialSweetY]);
                            SweetControl newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetType);
                            //���� ���� ����������Ʒ��ɫ
                            if (specialSweetType == SweetType.Row_Clear || specialSweetType == SweetType.Column_Clear && newSweet.CanColor() && matchedSweets[0].CanColor())
                            {
                                newSweet.ColoredComponent.SetColor(matchedSweets[0].ColoredComponent.ColorType);
                            }
                            //�ʺ���
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
    /// �������˵�
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// ���¿�ʼ��Ϸ
    /// </summary>
    public void ReloadGameScene()
    {
        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// ����ϰ���
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
    /// �������
    /// </summary>
    public void RowClear(int row)
    {
        for (int x = 0; x < xColumn; x++)
        {
            ClearSweet(x, row);
        }
    }
    /// <summary>
    /// �������
    /// </summary>
    public void ColumnClear(int column)
    {
        for (int y = 0; y < yRow; y++)
        {
            ClearSweet(column, y);
        }
    }
    /// <summary>
    /// �����ɫ�ķ���
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
