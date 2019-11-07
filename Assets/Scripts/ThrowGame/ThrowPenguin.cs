using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ThrowPenguin : GameScoreHandler
{

    [SerializeField] private Camera cam;
    [SerializeField] private bool timerStarted = false;
    [SerializeField] private bool gameover = false;
    [SerializeField] private bool playerLost = false;
    [SerializeField]private  GameObject backButton;
    public GameObject howToPlay;
    [SerializeField] private SpriteRenderer upSprite;
    [SerializeField] private SpriteRenderer downSprite;
    [SerializeField] private SpriteRenderer noOctoSprite;
    [SerializeField] private SpriteRenderer octo;
    [SerializeField] private Sprite squishSprite;
    [SerializeField] private float airFrictionForThrow = 500;
    private Feedback _feedback;
    Text timerText;
    Text scoreText;
    Vector2 mouseDelta = Vector2.zero;
    Vector2 oldMousePosition;

    float timerFloat = 5;

    //Vector3 centerOfScreen;
    bool showingScore = false;
    bool launchedPenguin = false;
    bool playedSplat = false;
    public bool sentData = false;
    public bool canPlay = false;
    
    //public float score = 0f;
    float canPlayTimer = 0.5f;
    float scoreTimer = -1;
    float throwPower = 0;

    public override float score  { get; set; }

    // Use this for initialization
    void Awake()
    {
        cam = Camera.main;
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<Text>();
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        backButton.SetActive(false);
        score = 0;
        _feedback = FindObjectOfType<Feedback>();
        if (AppManager.S == null)
            AppManager.S = FindObjectOfType<AppManager>();
    }

    private void Start()
    {
        timerText.text = "";
        sentData = false;
        StartCoroutine(ShowInstruction());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlatformPC())
        {
            if (CheckIsTouching())
            {
                mouseDelta = (Vector2)Input.mousePosition - oldMousePosition;
                oldMousePosition = Input.mousePosition;
            }
        }

        if (!canPlay)
        {
            return;
        }
        else if (canPlayTimer <= 0f)
        {
            if (!gameover)
            {
                GrabPenguin();
                HandleTimer();
                ChooseSprite();
                CalculateThrowPowerAdded();
            }
            else
            {
                if (playerLost && !showingScore)
                {
                    scoreText.text = "Too late! \nYou only got " + ((int)(throwPower / 2)).ToString("F2");
                    RevealQuitButton();

                }
                else
                {
                    if (scoreTimer == -1)
                        scoreTimer = Mathf.Max(2f, (throwPower / 20));
                    if (!launchedPenguin)
                    {
                        Throw();
                    }
                    else
                    {
                        if (scoreTimer > 0)
                            scoreTimer -= Time.deltaTime;
                        else
                        {
                            score = throwPower;                            
                            HighScore.SetHighScore(GameType.Throw, throwPower);
                            //scoreText.text = "Score: " + throwPower.ToString("F2") +"\nHigh Score: " + HighScore.GetHighScore(GameType.Throw).ToString();
                            if (!_feedback.feedbackShown)
                                StartCoroutine(_feedback.ShowFeedBack(throwPower > 9f, 1f, GameType.Throw));
                            octo.sprite = squishSprite;
                            PlaySplatSound();
                            GetComponentInChildren<Animator>().enabled = true;
                            RevealQuitButton();
                            if (!sentData && AppManager.S.gameMode != GameMode.Casual)
                            {
                                int score = (int)(throwPower);
                                if (AppManager.S)
                                {
                                    AppManager.S.UpdateScore(score);
                                    StartCoroutine(AuthenticationManager.S.UploadScore());
                                    sentData = true;
                                }
                                    
                            }
                            
                            //PlayerPrefs.SetFloat(DataManager.S.currentUser.ToString(), throwPower);
                        }
                    }
                }
                //Save score to server leaderboards
            }
        }
    }

    IEnumerator ShowInstruction()
    {
        while (howToPlay.activeSelf)
        {
            if (CheckIsTouching())
            {
                howToPlay.SetActive(false);
                while (true)
                {
                    canPlayTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                    if (canPlayTimer <= 0)
                    {
                        canPlay = true;
                        break;
                    }
                        
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void PlaySplatSound()
    {
        if (playedSplat) return;
        octo.GetComponent<AudioSource>().Play();
        playedSplat = true;
        PlayDustParticle();
    }

    void PlayDustParticle()
    {
        if (GetComponentInChildren<ParticleSystem>())
            GetComponentInChildren<ParticleSystem>().Play();
    }

    void PlaySound()
    {
        if (GetComponent<AudioSource>().isPlaying) return;
        GetComponent<AudioSource>().Play();
    }

    void ChooseSprite()
    {
        if (CheckIsTouching())
        {
            
            Vector3 touchPos = GetPointerPosition();
            Vector3 spritePosition = upSprite.transform.position;
            //print(touchPos);
            if (touchPos.y > spritePosition.y)
            {
                upSprite.enabled = true;
                downSprite.enabled = false;
                //GetComponent<AudioSource>().Play();
            }
            else
            {
                upSprite.enabled = false;
                downSprite.enabled = true;
                //GetComponent<AudioSource>().Play();
            }

            if (touchPos.x < spritePosition.x)
            {
                upSprite.flipX = true;
                downSprite.flipX = true;
                //GetComponent<AudioSource>().Play();
            }
            else
            {
                upSprite.flipX = false;
                downSprite.flipX = false;
                //GetComponent<AudioSource>().Play();
            }
            PlaySound();
        }
    }

    public void LoadMenu()
    {
        StartCoroutine(AppManager.S.BackToGameRoomPanel(AppManager.S.gameMode));
    }

    void ShowScore()
    {
        showingScore = true;
    }

    void RevealQuitButton()
    {
        if (backButton != null)
            backButton.SetActive(true);
    }

    void disableSprites()
    {
        upSprite.enabled = false;
        downSprite.enabled = false;
    }

    void Throw()
    {
        noOctoSprite.enabled = true;
        disableSprites();
        GetComponentInChildren<SpriteRenderer>().enabled = true;

        float tweenDuration = Mathf.Max(2f, (throwPower / 20));
        Ease throwEase = Ease.Linear;
        transform.DOJump(new Vector3(throwPower, transform.position.y, transform.position.z), 3, 1, tweenDuration).SetEase(throwEase);
        transform.GetChild(0).transform.DORotate(new Vector3(0, 0, 15), tweenDuration).SetEase(throwEase);
        cam.transform.DOMoveX(throwPower, tweenDuration).SetEase(throwEase);
        //GameObject.FindGameObjectWithTag("Background").transform.DOLocalMoveX(-5.65f, tweenDuration).SetEase(throwEase);
        launchedPenguin = true;
    }

    void CalculateThrowPowerAdded()
    {
        if (CheckIsTouching())
        {
            if (IsPlatformPC())
            {
                throwPower += mouseDelta.magnitude / airFrictionForThrow;
            }
            else
            {
                throwPower += Input.GetTouch(0).deltaPosition.magnitude / airFrictionForThrow;
            }            
        }
    }

    public static bool IsPlatformPC()
    {
        //input for PC version/editor
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return true;
        }
        //input is android/iOS
        return false;
    }

    void GrabPenguin()
    {        
        if (CheckIsTouching())
        {
            howToPlay.SetActive(false);
            return;
        }
        else if (timerStarted)
        {
            gameover = true;
        }
    }

    public static Vector2 GetPointerPosition()
    {
        Vector2 position = Vector3.zero;
        if (IsPlatformPC())
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            if (Input.touchCount > 0)
            {
                position = Input.GetTouch(0).position;
                return Camera.main.ScreenToWorldPoint(position);
            }
        }
        return Vector2.zero;
    }

    public bool CheckIsTouching()
    {
        if (IsPlatformPC())
        {
            return Input.GetMouseButton(0);
        }
        else
        {
            return Input.touchCount > 0;            
        }
    }

    void HandleTimer()
    {
        if (!howToPlay.activeSelf)
        {
            if (CheckIsTouching() && timerStarted == false && gameover == false)
            {
                timerStarted = true;
            }
            if (Input.GetKeyDown(KeyCode.Space) && timerStarted == false && gameover == false)
            {
                timerStarted = true;
            }

            if (timerStarted && gameover == false)
            {
                timerFloat -= Time.deltaTime;
                if (timerFloat < 0) timerFloat = 0;
                timerText.text = timerFloat.ToString("F2");
            }

            if (timerFloat == 0)
            {
                //timerStarted = false;
                gameover = true;
                playerLost = true;

            }
        }
        
    }
}
