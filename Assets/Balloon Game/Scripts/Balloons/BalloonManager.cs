using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    [SerializeField]
    GameObject greenBalloonPrefab;
    [SerializeField]
    GameObject blueBalloonPrefab;
    [SerializeField]
    GameObject redBalloonPrefab;
    [SerializeField]
    GameObject pinkBalloonPrefab;

    private int pinkBalloonCount = 0;
    [SerializeField]
    private int maxPinkBalloonCount = 0;

    public float pinkBalloonTimer = 0;
    [SerializeField]
    private float pinkBalloonSpawnTime = 5;

    public static BalloonManager instance;

    [SerializeField]
    int spawnPixelOffset = 20;

    [SerializeField]
    int greenChance = 60;
    [SerializeField]
    int blueChance = 20;
    [SerializeField]
    float directionForce = 3;

    private void Start()
    {
        if (instance==null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (pinkBalloonCount < maxPinkBalloonCount)
        {
            pinkBalloonTimer += Time.deltaTime;
            if (pinkBalloonTimer>=pinkBalloonSpawnTime)
            {
                pinkBalloonCount++;
                pinkBalloonTimer = 0;
                SpawnBalloon(true);
            }
        }
    }

    public static void SpawnBalloon(bool spawnPink = false)
    {
        GameObject newBalloon = null;
        Vector3 spawnPosition = instance.GetRandomSpawnPoint();                
        float randomDirection = Random.Range(-1f, -0.6f);
        if (Random.Range(0,2) == 0)
        {
            randomDirection = Mathf.Abs(randomDirection);
        }
        if (spawnPink)
        {
            newBalloon = Instantiate(instance.pinkBalloonPrefab, spawnPosition, new Quaternion());
        }
        else
        {
            switch (instance.ChooseRandomColor())
            {
                case BalloonColor.Green:
                    newBalloon = Instantiate(instance.greenBalloonPrefab, spawnPosition, new Quaternion());                    
                    break;
                case BalloonColor.Blue:
                    newBalloon = Instantiate(instance.blueBalloonPrefab, spawnPosition, new Quaternion());
                    break;
                case BalloonColor.Red:
                    newBalloon = Instantiate(instance.redBalloonPrefab, spawnPosition, new Quaternion());
                    break;
            }
        }
        newBalloon.GetComponent<Rigidbody>().AddForce(Vector3.right * randomDirection * instance.directionForce);    
    }
        
    private BalloonColor ChooseRandomColor()
    {
        int spawnChance = Random.Range(0, 100);
        if (spawnChance < greenChance)
        {
            return BalloonColor.Green;
        }
        if (spawnChance < greenChance + blueChance)
        {
            return BalloonColor.Blue;
        }
        return BalloonColor.Red;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int spawnPointX = Random.Range(spawnPixelOffset, Screen.width- spawnPixelOffset);
        int spawnPointY = Screen.height + spawnPixelOffset;
        Vector3 spawnPoint = new Vector3(spawnPointX, spawnPointY,0);
        spawnPoint = Camera.main.ScreenToWorldPoint(spawnPoint);
        spawnPoint.z = 0;
        //print(spawnPoint);
        return spawnPoint;
    }
}

public enum BalloonColor
{
    Green,
    Blue,
    Pink,
    Red
}
