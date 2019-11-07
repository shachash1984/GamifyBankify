using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScore : MonoBehaviour {

    public static HighScore instance;

    private void Start()
    {
        if (instance==null)
        {
            instance = this;
        }
        /*else
        {
            Destroy(gameObject);
        }*/
    }

    public static void SetHighScore(GameType type, float score)
    {
        if (score> GetHighScore(type))
            PlayerPrefs.SetFloat(type.ToString() + "HighScore", score);
    }

    public static float GetHighScore(GameType type)
    {
        return PlayerPrefs.GetFloat(type.ToString() + "HighScore");
    }
}


