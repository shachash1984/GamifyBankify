using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class Score : GameScoreHandler {

    static public Score S;
    //private int _score;
    [SerializeField] private Text _scoreText;
    private bool _countScore = false;
    private bool gameover = false;
    public bool sentData = false;
    [SerializeField] private GameObject howToPlay;
    private Feedback _feedback;
    public static event Action OnGameOver;

    public override float score { get; set; }
    
    void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(gameObject);

        sentData = false;
        _feedback = FindObjectOfType<Feedback>();
        if (AppManager.S == null)
            AppManager.S = FindObjectOfType<AppManager>();
    }

    void Update()
    {
       
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && gameover == false)
        {
            _countScore = true;
            
            if (howToPlay!=null)
            {
                if (howToPlay.activeInHierarchy)
                {
                    howToPlay.SetActive(false);
                    BalancePlayer.S.TiltPole(true);
                }
            }
                
            //GetComponent<AudioSource>().Play();
        }
        else if (gameover && !sentData && AppManager.S.gameMode != GameMode.Casual)
        {
            AppManager.S.UpdateScore((int)score);
            StartCoroutine(AuthenticationManager.S.UploadScore());
            sentData = true;
            if (!_feedback.feedbackShown)
                StartCoroutine(_feedback.ShowFeedBack(score > 150f, 1f, GameType.Balance));
            if (OnGameOver != null)
                OnGameOver();
            HighScore.SetHighScore(GameType.Balance, score);
        }
        else if(gameover && AppManager.S.gameMode == GameMode.Casual)
        {
            if (!_feedback.feedbackShown)
                StartCoroutine(_feedback.ShowFeedBack(score > 150f, 1f, GameType.Balance));
            if (OnGameOver != null)
                OnGameOver();
            HighScore.SetHighScore(GameType.Balance, score);
        }
        if (_countScore)
            SetScoreText();
    }

    public void SetScoreText()
    {
        AddScore(1);
        _scoreText.text = string.Format("Score: {0}", score.ToString("F2"));
    }

    void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }

    public void LoadMenu()
    {
        StartCoroutine(AppManager.S.BackToGameRoomPanel(AppManager.S.gameMode));
    }

    public void AllowScoreCount(bool count)
    {
        //GetComponent<AudioSource>().Pause();
        _countScore = count;
        gameover = !count;
    }
}
