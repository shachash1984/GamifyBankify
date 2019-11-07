using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : GameScoreHandler {

    public static GameManager instance;
    public override float score { get; set; }
    [SerializeField]
    Text scoreText;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject instructionPanel;
    static public bool sentData = false;
    public int missedBalloonPointReduction = -2;
    [SerializeField]
    private int maxHP = 3;
    public bool gameOver = false;
    public bool isInstructionPanelActive = false;
    public int currentHP;

    private float balloonSpawnTimer = 0;
    public float spawnCooldown = 2.5f;
    private Feedback _feedback;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        gameOver = false;
        balloonSpawnTimer = 0;
        currentHP = maxHP;
        sentData = false;
        ToggleInstructions(true);
        ToggleBackButton(false);
        if (AppManager.S == null)
            AppManager.S = FindObjectOfType<AppManager>();
        _feedback = FindObjectOfType<Feedback>();
    }
	
	// Update is called once per frame
	void Update () {
        if (isInstructionPanelActive)
            return;
        balloonSpawnTimer += Time.deltaTime;
        if (balloonSpawnTimer >= spawnCooldown && !gameOver)
        {
            balloonSpawnTimer = 0;
            BalloonManager.SpawnBalloon();
        }
	}

    public static void AddPoints(int points)
    {
        instance.score += points;
        instance.RefreshPointText();
    }

    private void RefreshPointText()
    {
        scoreText.text = "Points: " + instance.score;
    }

    public  void CheckGameOver()
    {
        if (instance.currentHP<=0)
        {
            gameOver = true;
            ToggleBackButton(true);

            //pop up game over menu

            //db code
            if (!sentData && AppManager.S.gameMode != GameMode.Casual)
            {
                AppManager.S.UpdateScore((int)score);
                StartCoroutine(AuthenticationManager.S.UploadScore());
                sentData = true;
                if (!_feedback.feedbackShown)
                    StartCoroutine(_feedback.ShowFeedBack(score > 15f, 1f, GameType.Baloon));
                HighScore.SetHighScore(GameType.Baloon, score);
            }
            else if(AppManager.S.gameMode == GameMode.Casual)
            {
                if (!_feedback.feedbackShown)
                    StartCoroutine(_feedback.ShowFeedBack(score > 15f, 1f, GameType.Baloon));
                HighScore.SetHighScore(GameType.Baloon, score);
            }
        }
    }

    public void ToggleBackButton(bool on)
    {
        backButton.SetActive(on);
    }
    
    public void ToggleInstructions(bool on)
    {
        instructionPanel.SetActive(on);
        isInstructionPanelActive = on;
    }

    public void LoadMenu()
    {
        StartCoroutine(AppManager.S.BackToGameRoomPanel(AppManager.S.gameMode));
    }
}
