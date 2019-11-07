using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBalloon : Balloon
{
    [SerializeField]
    protected int pointValue = 5;

    public override void TouchBalloon(Vector2 touchPos)
    {
        GameManager.AddPoints(pointValue);
        Destroy(gameObject);
    }

}
