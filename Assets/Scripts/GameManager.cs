using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoSingletion<GameManager>
{
    public int Height;
    public int Width;
    public int ColorNum = 4;

    public float AddLineInterval = 5f;
    public float DownTime = 1f;
    public float AbandonPieceDownTime = 0.03f;
    public float DieEffectDelay = 0.1f;

    public int Frequency22 = 3;
    public int Frequency33 = 10;
    public int Frequency44 = 12;
    public int WinSize = 5;

    void Awake()
    {
    }

    void Start()
    {
        initializeGameStatesCanvas();
        initializeScoreCanvas();
        ShowGridLines();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameState == GameStates.Playing)
                Pause();
        }
    }

    #region GridLine
    [SerializeField] private GameObject GridCanvas;

    public void ShowGridLines()
    {
        float lineWidth = 0.05f;
        GridCanvas.SetActive(true);
        for (int i = 0; i <= Width; i++)
        {
            GridLine newLine = GameObjectPoolManager.Instance.Pool_GridLinePool.AllocateGameObject<GridLine>(GridCanvas.transform);
            newLine.Initialize(GridLine.Orient.Vertical, new Vector2(i, 0), lineWidth, Height);
        }
        for (int j = 0; j <= Height; j++)
        {
            GridLine newLine = GameObjectPoolManager.Instance.Pool_GridLinePool.AllocateGameObject<GridLine>(GridCanvas.transform);
            newLine.Initialize(GridLine.Orient.Horizontal, new Vector2(0, j), lineWidth, Width);
        }
    }

    #endregion

    #region GameStates And Canvas

    public GameObject BeginCanvas;
    public Image NeedIntroPanel;
    public Animator NeedIntroAnim;
    public GameObject GameOverCanvas;
    public GameObject LevelSelectCanvas;
    public GameObject GamePauseCanvas;

    public enum GameStates
    {
        BeforeStart,
        Playing,
        GameOver,
        Pause
    }

    internal GameStates GameState = GameStates.BeforeStart;

    private void initializeGameStatesCanvas()
    {
        WinConditionText.enabled = false;
        ConditionText.enabled = false;
        ScoreCanvas.SetActive(false);
        LevelSelectCanvas.SetActive(false);
        BeginCanvas.SetActive(true);
        NeedIntroPanel.gameObject.SetActive(false);
        GameOverCanvas.SetActive(false);
        GameWinCanvas.SetActive(false);
        GamePauseCanvas.SetActive(false);
    }

    private bool isNeedIntro = true;
    public void StartButtonClick()
    {
        if (isNeedIntro)
        {
            NeedIntroPanel.gameObject.SetActive(true);
            NeedIntroAnim.SetTrigger("Jump");
        }
        else
        {
            BeginCanvas.SetActive(false);
            LevelSelectCanvas.SetActive(true);
        }
    }

    public void ReceiveIntroClick()
    {
        BeginCanvas.SetActive(false);
        Introduction();
    }

    public void CloseBeginCanvasClick()
    {
        isNeedIntro = false;
        NeedIntroPanel.gameObject.SetActive(false);
        BeginCanvas.SetActive(false);
        LevelSelectCanvas.SetActive(true);
    }

    public void GameOver()
    {
        GameState = GameStates.GameOver;
        GameOverCanvas.SetActive(true);
        TipsManager.Instance.RefreshText();
        Time.timeScale = 0;
    }

    public void GameWin()
    {
        StartCoroutine(Co_GameWin(0.5f));
    }

    IEnumerator Co_GameWin(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameWin(Score, FallTotalScore, BreakTotalScore, new int[] { currentRow, currentCol });
        yield return null;
    }


    [SerializeField] private GameObject GameWinCanvas;
    [SerializeField] private Text Win_TotalScoreText;
    [SerializeField] private Text Win_FallScoreText;
    [SerializeField] private Text Win_BreakScoreText;
    [SerializeField] private Text Win_MaxSizeText;

    public void GameWin(int totalscore, int fallscore, int breakscore, int[] maxsize)
    {
        GameWinCanvas.SetActive(true);
        TipsManager.Instance.RefreshText();
        GameState = GameStates.GameOver;
        Win_TotalScoreText.text = "Total Score: " + totalscore;
        Win_FallScoreText.text = "Fall Score: " + fallscore;
        Win_BreakScoreText.text = "Break Score: " + breakscore;
        Win_MaxSizeText.text = "Max Block Size: " + maxsize[0] + "x" + maxsize[1];
    }

    public enum HardLevel
    {
        Easy = 0,
        Hard = 1,
        Nightmare = 2
    }

    [SerializeField] private Text WinConditionText;
    [SerializeField] private Text ConditionText;

    private string conditionText
    {
        get
        {
            return "Build a <b><color=\"#FFFF11\">" + WinSize + "x" + WinSize + "</color></b> Block or larger to eliminate. Eliminate all colors !";
        }
    }
    public void OnClickEasyGame()
    {
        SetHardLevel(HardLevel.Easy);
        WinConditionText.enabled = true;
        ConditionText.enabled = true;
        ConditionText.text = conditionText;
        TargetSizeText.text = "Target: " + WinSize + "x" + WinSize;
        NewGame(HardLevel.Easy);
    }
    public void OnClickHardGame()
    {
        SetHardLevel(HardLevel.Hard);
        WinConditionText.enabled = true;
        ConditionText.enabled = true;
        ConditionText.text = conditionText;
        TargetSizeText.text = "Target: " + WinSize + "x" + WinSize;
        NewGame(HardLevel.Hard);
    }
    public void OnClickNightmareGame()
    {
        SetHardLevel(HardLevel.Nightmare);
        WinConditionText.enabled = true;
        ConditionText.enabled = true;
        ConditionText.text = conditionText;
        TargetSizeText.text = "Target: " + WinSize + "x" + WinSize;
        NewGame(HardLevel.Nightmare);
    }

    private void NewGame(HardLevel hardlevel)
    {
        GameState = GameStates.Playing;
        Time.timeScale = 1f;
        initializeScoreCanvas();
        LevelSelectCanvas.SetActive(false);
        ScoreCanvas.SetActive(true);
        NewLineGenerator.Initialize();
        BlocksManager.Instance.ResetGame();
        switch (hardlevel)
        {
            case HardLevel.Easy:
                BlocksManager.Instance.AddLinesOfBlock(3); break;
            case HardLevel.Hard:
                BlocksManager.Instance.AddLinesOfBlock(4); break;
            case HardLevel.Nightmare:
                BlocksManager.Instance.AddLinesOfBlock(5); break;
        }
    }

    private void SetHardLevel(HardLevel hardLevel)
    {
        switch (hardLevel)
        {
            case HardLevel.Easy:
                {
                    AddLineInterval = 20;
                    ColorNum = 2;
                    Frequency22 = 3;
                    Frequency33 = 10;
                    Frequency44 = 12;
                    WinSize = 7;
                    break;
                }
            case HardLevel.Hard:
                {
                    AddLineInterval = 20;
                    ColorNum = 3;
                    Frequency22 = 2;
                    Frequency33 = 8;
                    Frequency44 = 12;
                    WinSize = 6;
                    break;
                }
            case HardLevel.Nightmare:
                {
                    AddLineInterval = 20;
                    ColorNum = 4;
                    Frequency22 = 2;
                    Frequency33 = 8;
                    Frequency44 = 12;
                    WinSize = 5;
                    break;
                }
        }
    }

    public void ClearGame()
    {
        Time.timeScale = 0;
        BlocksManager.Instance.ClearAllBlocks();
    }

    public void Replay()
    {
        GamePauseCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        GameWinCanvas.SetActive(false);
        LevelSelectCanvas.SetActive(true);
        ClearGame();
    }

    private void Pause()
    {
        GameState = GameStates.Pause;
        Time.timeScale = 0;
        GamePauseCanvas.SetActive(true);
        TipsManager.Instance.RefreshText();
    }

    public void Introduction()
    {
        if (GameState == GameStates.Playing)
        {
            Pause();
        }
        GamePauseCanvas.SetActive(false);
        LevelSelectCanvas.SetActive(false);
        isNeedIntro = false;
        IntroductionManager.Instance.StartIntroduction();
    }

    public void EndIntroduction()
    {
        if (GameState == GameStates.Pause)
        {
            GamePauseCanvas.SetActive(true);
            TipsManager.Instance.RefreshText();
        }
        else if (GameState == GameStates.BeforeStart)
        {
            LevelSelectCanvas.SetActive(true);
        }
    }

    public void Resume()
    {
        GameState = GameStates.Playing;
        Time.timeScale = 1f;
        GamePauseCanvas.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Points Show

    public GameObject ScoreCanvas;

    public Text ScoreText;
    public Animator ScoreTextAnim;
    public Text FallTotalScoreText;
    public Animator FallTotalScoreTextAnim;
    public Text BreakTotalScoreText;
    public Animator BreakTotalScoreTextAnim;
    public Text SizeText;
    public Text TargetSizeText;
    public Animator SizeTextAnim;

    public GameObject FallScoreCanvas;
    public GameObject BreakScoreCanvas;
    public GameObject CongratulationCanvas;

    private int score = 0;

    private int Score
    {
        get { return score; }
        set
        {
            score = value;
            ScoreText.text = value.ToString();
            ScoreTextAnim.SetTrigger("Jump");
        }
    }

    private int fallTotalScore = 0;

    private int FallTotalScore
    {
        get { return fallTotalScore; }
        set
        {
            fallTotalScore = value;
            FallTotalScoreTextAnim.SetTrigger("Jump");
            FallTotalScoreText.text = value.ToString();
        }
    }

    private int breakScore = 0;

    private int BreakTotalScore
    {
        get { return breakScore; }
        set
        {
            breakScore = value;
            BreakTotalScoreTextAnim.SetTrigger("Jump");
            BreakTotalScoreText.text = value.ToString();
        }
    }

    //临时跳动分数
    public Text FallScoreText;
    public Animator FallScoreTextAnim;
    public Text BreakScoreText;
    public Animator BreakScoreTextAnim;

    public Text CongratulationText;
    public Animator CongratulationAnim;


    private void initializeScoreCanvas()
    {
        ScoreText.text = "0";
        SizeText.text = "1x1";
        Score = 0;
        FallTotalScore = 0;
        BreakTotalScore = 0;

        FallScoreCanvas.SetActive(false);
        BreakScoreCanvas.SetActive(false);
    }

    public void GetFallScore(int point)
    {
        FallScoreCanvas.SetActive(true);
        FallScoreText.text = "Fall! +" + point.ToString();
        FallScoreTextAnim.SetTrigger("Jump");

        FallTotalScore += point;
        Score += point;
    }

    public void GetBreakScore(int point)
    {
        BreakScoreCanvas.SetActive(true);
        BreakScoreText.text = "Break! +" + point.ToString();
        BreakScoreTextAnim.SetTrigger("Jump");

        BreakTotalScore += point;
        Score += point;
    }

    public void CongratulationTextFly()
    {
        CongratulationCanvas.SetActive(true);
        CongratulationText.text = "You have eliminate ONE color !";
        CongratulationAnim.speed = 0.5f;
        CongratulationAnim.SetTrigger("Jump");
    }

    int currentRow = 1;
    int currentCol = 1;

    public void SetMaxBlockSize(int[] size)
    {
        if (size[0] == currentRow && size[1] == currentCol) return;
        currentRow = size[0];
        currentCol = size[1];
        SizeText.text = size[0] + "x" + size[1];
        SizeTextAnim.SetTrigger("Jump");
    }

    #endregion
}