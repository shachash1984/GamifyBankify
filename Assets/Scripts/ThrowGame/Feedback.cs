using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Feedback : MonoBehaviour {

    [SerializeField] protected Sprite _greatSprite;
    [SerializeField] protected Sprite _awesomeSprite;
    [SerializeField] protected ParticleSystem _feedBackEffect;
    protected Image _feedBackImage;
    protected Button _closeFeedBackButton;
    protected CanvasGroup _feedBackPanel;
    protected Text _feedBackText;
    protected float _score;
    protected GameScoreHandler game;
    public bool feedbackShown = false;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        _feedBackPanel = GetComponent<CanvasGroup>();
        ToggleUIItem(_feedBackPanel, false, true);
        _feedBackImage = transform.GetChild(0).GetComponent<Image>();
        _feedBackText = _feedBackImage.transform.GetChild(0).GetComponent<Text>();
        game = FindObjectOfType<GameScoreHandler>();
        _feedBackEffect.gameObject.SetActive(false);
        feedbackShown = false;
    }

    public virtual IEnumerator ShowFeedBack(bool awesome, float delay, GameType gt)
    {
        feedbackShown = true;
        _closeFeedBackButton = transform.GetChild(1).GetComponent<Button>();
        _closeFeedBackButton.onClick.RemoveAllListeners();
        _closeFeedBackButton.onClick.AddListener(() =>
        {
            ToggleUIItem(_feedBackPanel, false);
            Debug.Log("ToggleUIItem(_feedBackPanel, false)");
        });
        yield return new WaitForSeconds(delay);
        if (awesome)
            SetFeedBackImage(_awesomeSprite);
        else
            SetFeedBackImage(_greatSprite);
        SetFeedBackText(awesome, gt);
        ToggleUIItem(_feedBackPanel, true);
        _feedBackEffect.gameObject.SetActive(true);

    }

    protected void ToggleUIItem(CanvasGroup cv, bool show, bool immediate = false)
    {
        if (immediate)
        {
            if (show)
            {
                cv.alpha = 1f;
                cv.blocksRaycasts = true;
            }
            else
            {
                cv.alpha = 0f;
                cv.blocksRaycasts = false;
            }
        }
        else
        {
            if (show)
            {
                cv.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
                cv.blocksRaycasts = true;
            }
            else
            {
                cv.DOFade(0, 0.5f).SetEase(Ease.OutQuad);
                cv.blocksRaycasts = false;
            }
        }
    }

    protected void SetFeedBackImage(Sprite s)
    {
        _feedBackImage.sprite = s;
    }

    protected void SetFeedBackText(bool awesome, GameType gt)
    {
        _score = game.score;
        if (awesome)
        {
            _feedBackText.text = string.Format("{0} points!\nHigh Score: {1}", _score.ToString("F2"), HighScore.GetHighScore(gt).ToString("F2"));
        }
        else
        {
            _feedBackText.text = string.Format("{0} points!\nHigh Score: {1}", _score.ToString("F2"), HighScore.GetHighScore(gt).ToString("F2"));
        }
    }
}
