using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BalancePlayer : MonoBehaviour {

    static public BalancePlayer S;
    [SerializeField] private GameObject _pole;
    public float _tiltPower = 2f;
    private const float TILT_POWER_MULT = 1.03f;
    private Button _leftButton;
    private Button _rightButton;
    private Score score;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(gameObject);

        score = FindObjectOfType<Score>();
        _leftButton = score.transform.GetChild(1).GetComponent<Button>();
        _rightButton = score.transform.GetChild(0).GetComponent<Button>();
        _leftButton.gameObject.SetActive(true);
        _rightButton.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Score.OnGameOver += DisableGameButtons;
    }

    private void OnDisable()
    {
        Score.OnGameOver -= DisableGameButtons;
    }


    public void TiltPole(bool tiltRight)
    {
        GetComponent<AudioSource>().Play();
        _tiltPower *= TILT_POWER_MULT;
        if (Pole.S.canTilt)
        {
            DOTween.Kill(_pole.transform);
            if (tiltRight)
                Pole.S.tiltZ -= _tiltPower;
            else
                Pole.S.tiltZ += _tiltPower;
        }        
    }

    public void DisableGameButtons()
    {
        _leftButton.gameObject.SetActive(false);
        _rightButton.gameObject.SetActive(false);
    }
}
