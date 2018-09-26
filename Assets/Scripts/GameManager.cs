using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingletion<GameManager>
{
    public int Height;
    public int Width;
    public int ColorNum = 4;

    public bool IsPieceAbandon = false;

    public float downTime = 1f;
    public float pieceDownTime = 0.1f;
    public float abandonPieceDownTime = 0.03f;
    public float dieEffectDelay = 0.1f;

    void Awake()
    {
    }

    void Start()
    {
        initializeGameStatesCanvas();
        initializeScoreCanvas();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameState == GameStates.Playing)
                Pause();
        }
    }

    #region GameStates And Canvas

    public GameObject GameOverCanvas;
    public GameObject GameStartCanvas;
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
        ScoreCanvas.SetActive(false);
        GameStartCanvas.SetActive(true);
        GameOverCanvas.SetActive(false);
        GamePauseCanvas.SetActive(false);
    }

    public void GameOver()
    {
        GameState = GameStates.GameOver;
        GameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void NewGame()
    {
        GameState = GameStates.Playing;
        Time.timeScale = 1f;
        initializeScoreCanvas();
        GameStartCanvas.SetActive(false);
        ScoreCanvas.SetActive(true);
        BlocksManager.Instance.StartGame();
    }

    public void ClearGame()
    {
        Time.timeScale = 0;
        BlocksManager.Instance.StartGame();
    }

    public void Replay()
    {
        GamePauseCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);

        ClearGame();
        NewGame();
    }

    private void Pause()
    {
        Time.timeScale = 0;
        GamePauseCanvas.SetActive(true);
    }

    public void Resume()
    {
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
    public Text EliminateTotalScoreText;
    public Animator EliminateTotalScoreTextAnim;
    public Text FallTotalScoreText;
    public Animator FallTotalScoreTextAnim;
    public Text BreakTotalScoreText;
    public Animator BreakTotalScoreTextAnim;
    public Text SizeText;
    public Animator SizeTextAnim;

    public GameObject EliminateScoreCanvas;
    public GameObject FallScoreCanvas;
    public GameObject BreakScoreCanvas;

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

    private int eliminateTotalScore = 0;

    private int EliminateTotalScore
    {
        get
        {
            return eliminateTotalScore;
        }
        set
        {
            eliminateTotalScore = value;
            EliminateTotalScoreTextAnim.SetTrigger("Jump");
            EliminateTotalScoreText.text = value.ToString();
        }
    }
    private int fallTotalScore = 0;
    private int FallTotalScore
    {
        get
        {
            return fallTotalScore;
        }
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
        get
        {
            return breakScore;
        }
        set
        {
            breakScore = value;
            BreakTotalScoreTextAnim.SetTrigger("Jump");
            BreakTotalScoreText.text = value.ToString();
        }
    }

    //临时跳动分数
    public Text EliminateScoreText;
    public Animator EliminateScoreTextAnim;
    public Text FallScoreText;
    public Animator FallScoreTextAnim;
    public Text BreakScoreText;
    public Animator BreakScoreTextAnim;

    private void initializeScoreCanvas()
    {
        ScoreText.text = "0";
        SizeText.text = "0x0";
        Score = 0;
        EliminateTotalScore = 0;
        FallTotalScore = 0;
        BreakTotalScore = 0;

        EliminateScoreCanvas.SetActive(false);
        FallScoreCanvas.SetActive(false);
        BreakScoreCanvas.SetActive(false);
    }

    public void GetEliminateScore(int point)
    {
        EliminateScoreCanvas.SetActive(true);
        EliminateScoreText.text = "Eliminate! +" + point.ToString();
        EliminateScoreTextAnim.SetTrigger("Jump");

        EliminateTotalScore += point;
        Score += point;
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

