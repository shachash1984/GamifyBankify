using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;
using System;

public enum GameMode { Create, Accept, Casual, View}
public enum GameType {None = 0, Throw, Balance, Whack, Baloon}

[Serializable]
public struct Message
{
    public string headline;
    public string message;
}
public class AppManager : MonoBehaviour {

    static public AppManager S;
    public User user;
    public Challenge challenge;
    public GameMode gameMode;
    public bool isCasualPlay = false;
    public string lastPIN = "lastPIN";
    private Canvas _canvas;
    [SerializeField] private CanvasGroup _buttonsPanel;
    [SerializeField] private CanvasGroup _gamesPanel;
    private Button _throwGameButton;
    private Button _balanceGameButton;
    private Button _whackGameButton;
    private Button _baloonGameButton;
    private Button _backButton;
    [SerializeField] private Button _justPlayButton;
    //private CanvasGroup _logosPanel;
    private InputField _inputNameCreate;
    private InputField _inputNameAccept;
    private InputField _inputPIN;
    private Button _OKButtonCreate;
    private Button _OKButtonAccept;
    private CanvasGroup _issueChallengePanel;
    private CanvasGroup _acceptChallengePanel;
    private CanvasGroup _gameRoomPanel;
    private Button[] _gameButtons;
    private Text _pinCodeText;
    private CanvasGroup _messagePanel;
    private Text _messageHeadLine;
    private Text _messageText;
    private Button _startGameButton;
    private Button _issueChallengeButton;
    private Button _acceptChallengeButton;
    private Button _dismissMessageButton;
    private Button _scoreBoardButton;
    private Button _shareButton;
    //private Button _backToBankifyButton;
    public Message[] messages;
    private Dictionary<string, string> messageMap;
    private Coroutine _updateScoresCoroutine;

    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        DontDestroyOnLoad(this);
        Init();
    }

    private void OnDestroy()
    {
        Debug.Log("AppManager destroyed");
        StopAllCoroutines();
    }

    private void Update()
    {
        #if UNITY_ANDROID
             if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        #endif
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
                PlayerPrefs.DeleteAll();
        #endif
    }

    public static AppManager GetInstance()
    {
        return S;
    }

    public void Init()
    {
        //assign all the UI Elements
        AssignUIElements();

        //Close all panels and open the main panel
        AppStartSequence();

        //Register Main buttons
        RegisterIssueChallengeButton();
        RegisterAcceptChallengeButton();
        RegisterJustPlayButton();
        RegisterScoreBoardButton();

        //make sure the user and challenge are not null
        if (user == null)
            user = new User();
        if (challenge == null)
            challenge = new Challenge();

        //init the messages dictionary
        InitMessageMap();

    }

    public IEnumerator InitGameRoomPanel(GameMode gm)
    {
        if (gm == GameMode.Create)
        {
            ToggleUIItem(_gameRoomPanel, true);
            _pinCodeText.text = string.Format("PIN: {0}", challenge.PIN);
            yield return StartCoroutine(AuthenticationManager.S.CreateChallenge(challenge));
            _startGameButton.gameObject.SetActive(true);
            _startGameButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.AddListener(() => PlayGame(challenge.gameIndex));
            _shareButton.onClick.RemoveAllListeners();
            _shareButton.onClick.AddListener(() => SharePIN());
            AssignBackButton(new GameObject[] { _gamesPanel.gameObject }, new GameObject[] { _gameRoomPanel.gameObject }, () =>
            {
                _backButton.onClick.RemoveAllListeners();
                AssignBackButton(new GameObject[] { _buttonsPanel.gameObject }, new GameObject[] { _gamesPanel.gameObject, _backButton.gameObject }, () => 
                {
                    ClearChallenge();
                    ClearUser();
                });
            });
            _updateScoresCoroutine = StartCoroutine(UpdateScoreBoardLoop());

        }
        else if (gm == GameMode.Accept)
        {
            ToggleUIItem(_gameRoomPanel, true);
            _pinCodeText.text = string.Format("PIN: {0}", challenge.PIN);
            yield return StartCoroutine(AuthenticationManager.S.LoginToChallenge());
            while (challenge.gameIndex == 0)
            {
                Debug.Log("Waiting for gameIndex...");
                yield return new WaitForSeconds(2f);
            }
            _startGameButton.gameObject.SetActive(true);
            _startGameButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.AddListener(() => PlayGame(challenge.gameIndex));
            _shareButton.onClick.RemoveAllListeners();
            _shareButton.onClick.AddListener(() => SharePIN());
            AssignBackButton(new GameObject[] { _gamesPanel.gameObject }, new GameObject[] { _gameRoomPanel.gameObject, _backButton.gameObject }, ()=>
            {
               // ClearChallenge();
               // ClearUser();
            });
            _updateScoresCoroutine = StartCoroutine(UpdateScoreBoardLoop());

        }
        else if(gm == GameMode.View)
        {
            ToggleUIItem(_gameRoomPanel, true);
            ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), true);
            _pinCodeText.text = string.Format("PIN: {0}", challenge.PIN);
            yield return StartCoroutine(AuthenticationManager.S.GetCurrentChallengeUsers());
            _startGameButton.gameObject.SetActive(false);
            _shareButton.onClick.RemoveAllListeners();
            _shareButton.onClick.AddListener(() => SharePIN());
            AssignBackButton(new GameObject[] { _buttonsPanel.gameObject }, new GameObject[] { _gameRoomPanel.gameObject, _backButton.gameObject }, ()=>
            {
                
            });
            _backButton.onClick.AddListener(() =>
            {
                ResetAppManager();
            });
            if (AuthenticationManager.S == null)
            {
                GetComponent<AuthenticationManager>().enabled = false;
                yield return null;
                GetComponent<AuthenticationManager>().enabled = true;
            }
            _updateScoresCoroutine = StartCoroutine(UpdateScoreBoardLoop());
        }
    }

    public void AssignUIElements()
    {
        //assigning the main canvas
        _canvas = FindObjectOfType<Canvas>();

        //assiging the games panel and game buttons
        _gamesPanel = _canvas.transform.GetChild(1).GetComponent<CanvasGroup>();
        _throwGameButton = _gamesPanel.transform.GetChild(1).GetComponent<Button>();
        _balanceGameButton = _gamesPanel.transform.GetChild(4).GetComponent<Button>();
        _whackGameButton = _gamesPanel.transform.GetChild(5).GetComponent<Button>();
        _baloonGameButton = _gamesPanel.transform.GetChild(3).GetComponent<Button>();

        //assigning the main panel and main buttons
        _buttonsPanel = _canvas.transform.GetChild(2).GetComponent<CanvasGroup>();
        _issueChallengeButton = _buttonsPanel.transform.GetChild(1).GetComponent<Button>();
        _acceptChallengeButton = _buttonsPanel.transform.GetChild(2).GetComponent<Button>();
        _justPlayButton = _buttonsPanel.transform.GetChild(3).GetComponent<Button>();
        _scoreBoardButton = _buttonsPanel.transform.GetChild(4).GetComponent<Button>();

        //_backToBankifyButton = _buttonsPanel.transform.GetChild(3).GetComponent<Button>();

        //assigning the logos panel
        //_logosPanel = _canvas.transform.GetChild(3).GetComponent<CanvasGroup>();


        //assigning the Issue Challenge Panel
        _issueChallengePanel = _canvas.transform.GetChild(4).GetComponent<CanvasGroup>();
        _inputNameCreate = _issueChallengePanel.transform.GetChild(1).GetComponent<InputField>();
        _OKButtonCreate = _issueChallengePanel.transform.GetChild(2).GetComponent<Button>();

        //assigning the accept Challenge Button
        _acceptChallengePanel = _canvas.transform.GetChild(5).GetComponent<CanvasGroup>();
        _inputNameAccept = _acceptChallengePanel.transform.GetChild(1).GetComponent<InputField>();
        _inputPIN = _acceptChallengePanel.transform.GetChild(2).GetComponent<InputField>();
        _OKButtonAccept = _acceptChallengePanel.transform.GetChild(3).GetComponent<Button>();


        //assigning the game room panel
        _gameRoomPanel = _canvas.transform.GetChild(6).GetComponent<CanvasGroup>();
        _pinCodeText = _gameRoomPanel.transform.GetChild(3).GetComponent<Text>();
        _startGameButton = _gameRoomPanel.transform.GetChild(4).GetComponent<Button>();
        _shareButton = _gameRoomPanel.transform.GetChild(6).GetComponent<Button>();

        //assigning the message panel
        _messagePanel = _canvas.transform.GetChild(7).GetComponent<CanvasGroup>();
        _messageHeadLine = _messagePanel.transform.GetChild(0).GetComponent<Text>();
        _messageText = _messagePanel.transform.GetChild(1).GetComponent<Text>();
        _dismissMessageButton = _messagePanel.transform.GetChild(2).GetComponent<Button>();

        //assigning backButton
        _backButton = _canvas.transform.GetChild(8).GetComponent<Button>();
        isCasualPlay = false;
    }

    public void AppStartSequence()
    {
        ToggleUIItem(_buttonsPanel, true, true);
        ToggleUIItem(_issueChallengePanel, false, true);
        ToggleUIItem(_acceptChallengePanel, false, true);
        ToggleUIItem(_gamesPanel, false, true);
        ToggleUIItem(_gameRoomPanel, false, true);
        ToggleUIItem(_messagePanel, false, true);
        _backButton.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void RegisterGameButtons(GameMode gm)
    {
        switch (gm)
        {
            case GameMode.Create:
                _throwGameButton.onClick.RemoveAllListeners();
                _throwGameButton.onClick.AddListener(() =>
                {
                    SetGameType(1);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Create));

                });
                _balanceGameButton.onClick.RemoveAllListeners();
                _balanceGameButton.onClick.AddListener(() =>
                {
                    SetGameType(2);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Create));

                });
                _whackGameButton.onClick.RemoveAllListeners();
                _whackGameButton.onClick.AddListener(() =>
                {
                    SetGameType(3);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Create));

                });
                _baloonGameButton.onClick.RemoveAllListeners();
                _baloonGameButton.onClick.AddListener(() =>
                {
                    SetGameType(4);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Create));

                });
                break;
            case GameMode.Accept:
                _throwGameButton.onClick.RemoveAllListeners();
                _throwGameButton.onClick.AddListener(() =>
                {
                    SetGameType(1);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Accept));

                });
                _balanceGameButton.onClick.RemoveAllListeners();
                _balanceGameButton.onClick.AddListener(() =>
                {
                    SetGameType(2);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Accept));

                });
                _whackGameButton.onClick.RemoveAllListeners();
                _whackGameButton.onClick.AddListener(() =>
                {
                    SetGameType(3);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Accept));

                });
                _baloonGameButton.onClick.RemoveAllListeners();
                _baloonGameButton.onClick.AddListener(() =>
                {
                    SetGameType(4);
                    AssignPINCode(false);
                    StartCoroutine(InitGameRoomPanel(GameMode.Accept));

                });
                break;
            case GameMode.Casual:
                _throwGameButton.onClick.RemoveAllListeners();
                _throwGameButton.onClick.AddListener(() =>
                {
                    SetGameType(1);
                    PlayGame(1);

                });
                _balanceGameButton.onClick.RemoveAllListeners();
                _balanceGameButton.onClick.AddListener(() =>
                {
                    SetGameType(2);
                    PlayGame(2);

                });
                _whackGameButton.onClick.RemoveAllListeners();
                _whackGameButton.onClick.AddListener(() =>
                {
                    SetGameType(3);
                    PlayGame(3);

                });
                _baloonGameButton.onClick.RemoveAllListeners();
                _baloonGameButton.onClick.AddListener(() =>
                {
                    SetGameType(4);
                    PlayGame(4);
                });
                break;
            default:
                break;
        }
    }

    public void RegisterIssueChallengeButton()
    {
        _issueChallengeButton.onClick.RemoveAllListeners();
        _issueChallengeButton.onClick.AddListener(() =>
        {
            CreateChallenge();
            //Register Game Buttons
            RegisterGameButtons(GameMode.Create);
        });

    }

    public void RegisterAcceptChallengeButton()
    {
        _acceptChallengeButton.onClick.RemoveAllListeners();
        _acceptChallengeButton.onClick.AddListener(() =>
        {
            AcceptChallenge();
            //Register Game Buttons
            RegisterGameButtons(GameMode.Accept);
        });

    }

    public void InitMessageMap()
    {
        messageMap = new Dictionary<string, string>();
        for (int i = 0; i < messages.Length; i++)
        {
            messageMap.Add(messages[i].headline, messages[i].message);
        }
    }

    public IEnumerator BackToGameRoomPanel(GameMode gm)
    {
        yield return null;
        if (gm == GameMode.Casual)
        {

            StartCoroutine(ReturnToMenu());
        }
        else
        {
            StartCoroutine(ReturnToGameRoom());
        }

    }

    IEnumerator ReturnToGameRoom()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            SceneManager.LoadScene(0);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 0);
        yield return null;
        Init();
        ToggleUIItem(_gameRoomPanel, true, true);
        _pinCodeText.text = string.Format("PIN: {0}", challenge.PIN);
        _startGameButton.gameObject.SetActive(false);
        ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), true, true);
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() =>
        {
            DisplayMessage("Leave Challenge", "Are you sure\nyou want to leave\nthis challenge?");
            _dismissMessageButton.onClick.RemoveAllListeners();
            _dismissMessageButton.onClick.AddListener(() =>
            {
                ScoreBoardManager.S.ResetScoreBoard();
                ResetAppManager();
                ToggleUIItem(_buttonsPanel, true);
                ToggleUIItem(_gameRoomPanel, false);
                ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), false);
                DismissMessage();
                Init();
            });
            //assign option buttons ( Leave / Cancel)
            //display option buttons
        });
        _shareButton.onClick.RemoveAllListeners();
        _shareButton.onClick.AddListener(() => SharePIN());
        _issueChallengeButton.onClick.RemoveAllListeners();
        _issueChallengeButton.onClick.AddListener(() => CreateChallenge());
        ToggleUIItem(_buttonsPanel, false, true);
        ScoreBoardManager.S.Init();
        if (AuthenticationManager.S == null)
        {
            GetComponent<AuthenticationManager>().enabled = false;
            yield return null;
            GetComponent<AuthenticationManager>().enabled = true;
        }

        _updateScoresCoroutine = StartCoroutine(UpdateScoreBoardLoop());

    }

    IEnumerator ReturnToMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            SceneManager.LoadScene(0);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 0);
        yield return null;

        Init();
    }

    public void InitNameInputPanel()
    {
        _inputNameCreate.text = "";
        _inputNameAccept.text = "";
        _OKButtonCreate.onClick.RemoveAllListeners();
        _OKButtonCreate.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(_inputNameCreate.text))
                CheckIsNameValid(_inputNameCreate.text);
            else
            {
                DisplayMessage("Empty Name");
                AssignDismissMessageButton(1);
            }
        });
        
    }

    public void InitParticipantNameInputPanel()
    {
        _inputNameAccept.text = "";
        _pinCodeText.text = "";
        _OKButtonAccept.onClick.RemoveAllListeners();
        _OKButtonAccept.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(_inputNameAccept.text) || !string.IsNullOrEmpty(_inputPIN.text))
                CheckIfNameAndPINAreValid(_inputNameAccept.text, _inputPIN.text);
            else
            {
                if (string.IsNullOrEmpty(_inputPIN.text))
                    DisplayMessage("Empty PIN");
                else if (string.IsNullOrEmpty(_inputNameAccept.text))
                    DisplayMessage("Empty Name");
            }
        });
    }

    public void CheckIsNameValid(string nam)
    {
        if(nam.Contains(" "))
            DisplayMessage("Name Error");
        else
        {
            AssignUserName(nam);
            if (_issueChallengePanel.gameObject.activeSelf)
            {
                ToggleUIItem(_issueChallengePanel, false);
                ToggleUIItem(_gamesPanel, true);
            }
        }
    }

    public void CheckIfNameAndPINAreValid(string nam, string pin)
    {
        if (nam.Contains(" "))
            DisplayMessage("Name Error");
        else if (pin.Contains(" "))
            DisplayMessage("PIN Error", "PIN cannot contain spaces");
        else
        {
            EnterPIN(pin);
            AssignUserName(nam);
            ToggleUIItem(_acceptChallengePanel, false);
            StartCoroutine(InitGameRoomPanel(GameMode.Accept));
        }
    }

    public void AssignUserName(string nameEntered)
    {
        if (user == null)
            user = new User();
        user.name = nameEntered;
        if (challenge == null)
            challenge = new Challenge();
        if (challenge.userContainer == null)
            challenge.userContainer = new UserContainer();
        challenge.userContainer.users.Add(user);
    }

    public void EnterPIN(string pin)
    {
        this.challenge.PIN = pin.ToUpper();
    }

    public void CreateChallenge()
    {
        gameMode = GameMode.Create;
        ToggleUIItem(_buttonsPanel, false);
        ToggleUIItem(_issueChallengePanel, true);
        ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), true);
        InitNameInputPanel();
        ScoreBoardManager.S.Init();
        AssignBackButton(new GameObject[] { _buttonsPanel.gameObject }, new GameObject[] { _issueChallengePanel.gameObject, _backButton.gameObject, _gamesPanel.gameObject }, ()=>
        {
            //ClearChallenge();
            //ClearUser();
        });
        isCasualPlay = false;
    }

    public void AcceptChallenge()
    {
        gameMode = GameMode.Accept;
        ToggleUIItem(_buttonsPanel, false);
        ToggleUIItem(_acceptChallengePanel, true);
        ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), true);
        InitParticipantNameInputPanel();
        ScoreBoardManager.S.Init();
        AssignBackButton(new GameObject[] { _buttonsPanel.gameObject }, new GameObject[] { _acceptChallengePanel.gameObject, _backButton.gameObject }, ()=>
        {
            ClearChallenge();
            ClearUser();
        });
        isCasualPlay = false;
    }

    public void PlayGame(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ToggleUIItem(CanvasGroup cv, bool show, bool immediate = false)
    {
        if (immediate)
        {
            if (show)
            {
                cv.gameObject.SetActive(true);
                cv.alpha = 1f;
                cv.blocksRaycasts = true;
            }
            else
            {
                cv.alpha = 0f;
                cv.blocksRaycasts = false;
                cv.gameObject.SetActive(false);
            }
        }
        else
        {
            if (show)
            {
                cv.gameObject.SetActive(true);
                cv.DOFade(1, 0.75f).SetEase(Ease.OutQuad);
                cv.blocksRaycasts = true;
            }
            else
            {
                cv.DOFade(0, 0.75f).SetEase(Ease.OutQuad);
                cv.blocksRaycasts = false;
                cv.gameObject.SetActive(false);
            }
        }
        
    }

    public void AssignBackButton(GameObject[] itemsToOpen, GameObject[] itemsToClose, UnityEngine.Events.UnityAction DoAction)
    {
        _backButton.onClick.RemoveAllListeners();
        foreach (GameObject item in itemsToClose)
        {
            _backButton.onClick.AddListener(() => ToggleUIItem(item.GetComponent<CanvasGroup>(), false));
        }

        foreach (GameObject item in itemsToOpen)
        {
            _backButton.onClick.AddListener(() =>
            {
                ToggleUIItem(item.GetComponent<CanvasGroup>(), true);
                user = null;

            });
        }
        _backButton.onClick.AddListener(DoAction);
        
    }

    public void SetGameType(int gameIndex)
    {
        challenge.gameIndex = gameIndex;
    }

    private bool AssignPINCode(bool lastChallenge)
    {
        if (!lastChallenge)
        {
            this.challenge.PIN = CreatePassword(6);
            return true;
        }
        else
        {
            if (PlayerPrefs.HasKey(lastPIN))
            {
                this.challenge.PIN = PlayerPrefs.GetString(lastPIN);
                return true;
            }
            else
            {
                DisplayMessage("Challenge not found");
                return false;
            }
        }
    }

    public string CreatePassword(int length)
    {
        const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            res.Append(valid[UnityEngine.Random.Range(0, valid.Length)]);
        }
        PlayerPrefs.SetString(lastPIN, res.ToString());
        return res.ToString();
    }

    public void DisplayMessage(string msgKey, string msg = "", bool enforce = false)
    {
        if (!enforce)
        {
            if (messageMap.ContainsKey(msgKey))
            {
                _messageHeadLine.text = msgKey;
                _messageText.text = messageMap[msgKey];
            }
            else
            {
                messageMap.Add(msgKey, msg);
                _messageHeadLine.text = msgKey;
                _messageText.text = msg;
            }
            ToggleUIItem(_messagePanel, true);
        }
        else
        {
            messageMap.Add(msgKey, msg);
            _messageHeadLine.text = msgKey;
            _messageText.text = msg;
            ToggleUIItem(_messagePanel, true);
        }
        _dismissMessageButton.onClick.RemoveAllListeners();
        _dismissMessageButton.onClick.AddListener(() =>
        {
            DismissMessage();
        });
    }

    public void DismissMessage()
    {
        ToggleUIItem(_messagePanel, false);
    }

    public void OpenKeyboard()
    {
        TouchScreenKeyboard.Open("");
    }

    public void UpdateScore(int newScore)
    {
        this.user.score = newScore;
        this.challenge.userContainer.users[this.challenge.userContainer.users.IndexOf(this.challenge.userContainer.users.Find(x => x.name == this.user.name))].score = newScore;
    }

    public void LaunchBankifyApp()
    {
        bool fail = false;
        string bundleId = "com.bankify.bankifyapp"; // your target bundle id
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            fail = true;
        }

        if (fail)
        { //open app in store
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.bankify.bankifyapp");
        }
        else //open the app
            ca.Call("startActivity", launchIntent);

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();

    }

    public IEnumerator UpdateScoreBoardLoop()
    {

        yield return null;
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 0);
        yield return new WaitForSeconds(1);
        if (SceneManager.GetActiveScene().buildIndex != 0)
            StopCoroutine(_updateScoresCoroutine);

        while (SceneManager.GetActiveScene().buildIndex == 0 && challenge != null || gameMode == GameMode.View)
        {
            if (AuthenticationManager.S == null)
            {
                GetComponent<AuthenticationManager>().enabled = false;
                yield return null;
                GetComponent<AuthenticationManager>().enabled = true;
            }
            yield return StartCoroutine(AuthenticationManager.S.GetCurrentChallengeUsers());
            yield return new WaitForSeconds(10f);
        }
    }

    public void JustPlay()
    {
        gameMode = GameMode.Casual;
        isCasualPlay = true;
        ToggleUIItem(_buttonsPanel, false);
        ToggleUIItem(_gamesPanel, true, true);
        ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), true);
        AssignBackButton(new GameObject[] { _buttonsPanel.gameObject }, new GameObject[] { _gamesPanel.gameObject, _backButton.gameObject }, () => { });
    }

    public void RegisterJustPlayButton()
    {
        
        _justPlayButton.onClick.RemoveAllListeners();
        _justPlayButton.onClick.AddListener(() => 
        {
            JustPlay();
            RegisterGameButtons(GameMode.Casual);
        });
    }

    public void ViewScoreBoard()
    {
        if (AssignPINCode(true))
        {
            gameMode = GameMode.View;
            ToggleUIItem(_buttonsPanel, false);
            StartCoroutine(InitGameRoomPanel(gameMode));
            ScoreBoardManager.S.Init();
            isCasualPlay = true;
        }
    }

    public void RegisterScoreBoardButton()
    {

        _scoreBoardButton.onClick.RemoveAllListeners();
        _scoreBoardButton.onClick.AddListener(() =>
        {
            ViewScoreBoard();
        });
        _dismissMessageButton.onClick.RemoveAllListeners();
        _dismissMessageButton.onClick.AddListener(() =>
        {
            ToggleUIItem(_buttonsPanel, true);
        });
    }

    public void ReAssignPIN()
    {
        string[] pinText = _pinCodeText.text.Split(':');
        challenge.PIN = pinText[1].Trim();
    }

    public void ResetAppManager()
    {
        user = null;
        challenge.gameIndex = 0;
        challenge.userContainer = null;
        challenge.PIN = "";
        challenge = null;
        StopAllCoroutines();
        if (AuthenticationManager.S != null)
            AuthenticationManager.S.StopAllCoroutines();
    }

    public void SharePIN()
    {
        var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0 , 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToJPG();
        string path = Application.persistentDataPath + "/ScreenShot.jpg";
        File.WriteAllBytes(path, bytes);
        Destroy(tex);

        NativeShare share = new NativeShare();
        share.SetSubject("Gamify PIN Code");
        share.SetTitle("Gamify PIN Code");
        share.SetText("Come join our challenge at GamifyBankify!\nThe PIN Code is: " + challenge.PIN);
        share.AddFile(path);
        share.Share();
    }

    public void ClearChallenge()
    {
        challenge = new Challenge();
    }

    public void ClearUser()
    {
        user = new User();
    }

    public void AssignDismissMessageButton(int index)
    {
        if(index == 0)
        {
            _dismissMessageButton.onClick.RemoveAllListeners();
            _dismissMessageButton.onClick.AddListener(() =>
            {
                ToggleUIItem(_gameRoomPanel, false);
                ToggleUIItem(_backButton.GetComponent<CanvasGroup>(), false);
                ToggleUIItem(_buttonsPanel, true);
            });
        }
        else if(index == 1)
        {
            _dismissMessageButton.onClick.RemoveAllListeners();
            _dismissMessageButton.onClick.AddListener(() =>
            {
                ToggleUIItem(_buttonsPanel, false, true);
            });
        }
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }
}
