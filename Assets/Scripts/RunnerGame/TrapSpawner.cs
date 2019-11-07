using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour {

    [SerializeField]
    GameObject spike;
    [SerializeField]
    GameObject lowBat;
    [SerializeField]
    GameObject highBat;

    public float spawnTimer;
    public float maxSpawnTimer = 3;
    public float accelerationTimer = 1;

    static bool playing = false;

    // Use this for initialization
    void Start () {
        spawnTimer = maxSpawnTimer;
	}
	
	// Update is called once per frame
	void Update () {
        if (playing)
        { 
        spawnTimer -= Time.deltaTime;
        accelerationTimer -= Time.deltaTime;
        if (accelerationTimer<=0)
        {
            accelerationTimer = 1;
            if (maxSpawnTimer>1)
                maxSpawnTimer -= 0.1f;
        }
            if (spawnTimer <= 0)
            {
                spawnTimer = maxSpawnTimer;
                int maxRandom = 2;
                if (maxSpawnTimer < 1.5f) maxRandom++;
                switch (Random.Range(0, maxRandom))
                {
                    case 0:
                        Instantiate(spike, transform.position, transform.rotation);
                        break;
                    case 1:
                        Instantiate(lowBat, transform.position + Vector3.up, transform.rotation);
                        break;
                    case 2:
                        Instantiate(highBat, transform.position + Vector3.up * 3, transform.rotation);
                        break;
                }
            }

        }
        else
        {
            if (Input.touchCount > 0 && playing == false)
            {
                playing = true;
            }
        }
	}

    public static void StopSpawning()
    {
        playing = false;
    }
}
