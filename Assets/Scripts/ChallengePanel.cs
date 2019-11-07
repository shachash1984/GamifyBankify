using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ChallengePanel : MonoBehaviour {

    public Text groupText;
    public Text gameText;
    public Text sumText;
    public Button _statsButton;
    public Button _playButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    public Challenge challenge;

}
