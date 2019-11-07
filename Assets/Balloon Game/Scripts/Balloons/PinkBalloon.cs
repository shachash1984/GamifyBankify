using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkBalloon : Balloon
{
    [SerializeField]
    protected int pointValue = 0;
    [SerializeField]
    private int bounceForce = 7;

    public override void TouchBalloon(Vector2 touchPos)
    {
        Vector2 touchDirection = touchPos - (Vector2)transform.position;
        rigid.AddForce((Vector2.up + touchDirection.normalized) * bounceForce);         
    }
}
