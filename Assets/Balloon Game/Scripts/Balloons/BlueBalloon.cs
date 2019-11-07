using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBalloon : Balloon
{
    [SerializeField]
    private int bounceCount = 0;
    [SerializeField]
    private int maxBounceCount = 3;
    [SerializeField]
    private int bounceForce = 7;
    [SerializeField]
    protected int pointValue = 3;

    public override void TouchBalloon(Vector2 touchPos)
    {
        GameManager.AddPoints(pointValue);
        bounceCount++;
        if (bounceCount >= maxBounceCount)
        {
            Destroy(gameObject);
        }
        else
        {
            Vector2 touchDirection = touchPos - (Vector2)transform.position;
            rigid.AddForce((Vector2.up + touchDirection.normalized) * bounceForce);
        }
    }

}
