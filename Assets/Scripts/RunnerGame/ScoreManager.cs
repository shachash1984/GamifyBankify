using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    [SerializeField] PlayerController player;
    [SerializeField] Text scoreText;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerController>();
        scoreText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score: " + player.transform.position.x.ToString("F2");
	}
}
