using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    PlayerController player;

    public float cameraDistance = 2;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {     
        if (player.playing)
            transform.position = new Vector3(player.transform.position.x + cameraDistance, transform.position.y, transform.position.z);
        
	}
}
