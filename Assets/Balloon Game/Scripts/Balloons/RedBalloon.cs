using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBalloon : Balloon
{
    [SerializeField]
    protected int pointValue = -5;

    // Use this for initialization
    void Start () {
		
	}

    public override void TouchBalloon(Vector2 touchPos)
    {
        GameManager.AddPoints(pointValue);
        Destroy(gameObject);
    }
}
