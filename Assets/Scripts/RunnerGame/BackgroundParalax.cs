using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParalax : MonoBehaviour {

    //[SerializeField]
    //GameObject backgroundImage1;
    //[SerializeField]
    //GameObject backgroundImage2;
    //[SerializeField]
    //PlayerController player;
    /*[SerializeField]
    float parralaxSpeed;

    public float parralaxCoefficient = 2;

    //bool moveNumber1 = true;

    public float backgroundXsize = 17;
    public Vector3 lastBackgroundPosition;

	// Use this for initialization
	void Start () {
        parralaxSpeed = player.speed / parralaxCoefficient;        

    }
	
	// Update is called once per frame
	void Update () {
        if (player.playing)
        {
            if (player.gameover == false)
            {
                if (Vector2.Distance(player.transform.position, transform.position) > backgroundXsize - Camera.main.orthographicSize)
                {
                    transform.Translate(Vector3.right * backgroundXsize);
                }

                backgroundImage1.transform.Translate(Vector3.left * Time.deltaTime * parralaxSpeed);
                backgroundImage2.transform.Translate(Vector3.left * Time.deltaTime * parralaxSpeed);
                if (Vector2.Distance(backgroundImage1.transform.position, player.transform.position) > backgroundXsize
                    && backgroundImage1.transform.position.x < player.transform.position.x)
                {
                    backgroundImage1.transform.Translate(Vector3.right * backgroundXsize * 2);
                }
                if (Vector2.Distance(backgroundImage2.transform.position, player.transform.position) > backgroundXsize
                    && backgroundImage2.transform.position.x < player.transform.position.x)
                {
                    backgroundImage2.transform.Translate(Vector3.right * backgroundXsize * 2);
                }
            }
        }
    }*/
}
