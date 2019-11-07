using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Balloon : MonoBehaviour
{
    protected Rigidbody rigid;
    protected BalloonColor color;
    [SerializeField]
    protected float antiGravity = 1;  

    // Use this for initialization
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && Time.timeScale > 0)
            {
                checkTouch(Input.GetTouch(0).position);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && Time.timeScale > 0)
            {
                checkTouch(Input.mousePosition);
            }
        }
    }

    private void FixedUpdate()
    {
        rigid.AddForce((Vector3.up * -2f) / 50);
    }

    private void checkTouch(Vector3 pos)
    {
        //print(Camera.main.ScreenToWorldPoint(pos));
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 touchPos = new Vector2(wp.x, wp.y);

        if (Vector2.Distance(touchPos, transform.position) < 1)
        {
            //print(gameObject.transform.name + "popped");
            TouchBalloon(pos);
            //GetComponent<AudioSource>().Play();            
        }
    }

    public virtual void TouchBalloon(Vector2 touchPos) { }

    public BalloonColor GetColor { get { return color; } }        
}
