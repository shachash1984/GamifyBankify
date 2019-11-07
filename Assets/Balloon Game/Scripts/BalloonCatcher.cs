using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonCatcher : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.instance.isInstructionPanelActive)
            return;

        if (collision.gameObject.GetComponent<Balloon>())
        {
            if (collision.gameObject.GetComponent<PinkBalloon>())
            {
                GameManager.instance.currentHP--;
                GameManager.instance.CheckGameOver();
            }
            else
            {
                if (collision.gameObject.GetComponent<RedBalloon>() == null)
                {
                    GameManager.AddPoints(GameManager.instance.missedBalloonPointReduction);
                }
            }
            Destroy(collision.gameObject);
        }
    }
}
