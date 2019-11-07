using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WhackManager : GameScoreHandler {


    public static WhackManager instance;
    public float timer = 15;
    public bool sentData = false;

    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text timerText;
    [SerializeField]
    GameObject howToPlay;
    [SerializeField]
    GameObject backButton;

    public bool gameStarted = false;
    public bool gameover = false;
    private Feedback _feedback;

    public override float score { get; set; }
    

    // Use this for initialization
    void Start () {        
        if (instance == null)
        {
            instance = this;
        }
        sentData = false;
        if (AppManager.S == null)
            AppManager.S = FindObjectOfType<AppManager>();
        _feedback = FindObjectOfType<Feedback>();
        score = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space))
            UpdateScore();
        CheckStart();
        if (!gameStarted) return;
        CheckTimer();
        if (gameover == false)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F2");
        }
        else
        {
            if (!sentData && AppManager.S.gameMode != GameMode.Casual)
            {
                AppManager.S.UpdateScore((int)score);
                StartCoroutine(AuthenticationManager.S.UploadScore());
                sentData = true;
                if (!_feedback.feedbackShown)
                    StartCoroutine(_feedback.ShowFeedBack(score > 20f, 1f, GameType.Whack));
                HighScore.SetHighScore(GameType.Whack, score);
            }
            else if(AppManager.S.gameMode == GameMode.Casual)
            {
                if (!_feedback.feedbackShown)
                    StartCoroutine(_feedback.ShowFeedBack(score > 20f, 1f, GameType.Whack));
                HighScore.SetHighScore(GameType.Whack, score);
            }
        }
    }

    void CheckTimer()
    {
        if (timer<=0.01f)
        {
            timerText.text = "0.00";
            gameover = true;
            //PlayerPrefs.SetFloat(DataManager.S.currentUser.ToString(), score);
            backButton.SetActive(true);
        }
    }

    void CheckStart()
    {
        if (Input.touchCount>0 || Input.GetMouseButtonDown(0))
        {
            gameStarted = true;
            howToPlay.SetActive(false);
        }
    }

    public void LoadMenu()
    {
        StartCoroutine(AppManager.S.BackToGameRoomPanel(AppManager.S.gameMode));
    }

    public void UpdateScore(bool add = true)
    {
        if (add)
            score += 2;
        else
            score -= 1;
        scoreText.text = "Score: " + score.ToString("F2");
    }

}
