using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitHedgehog : MonoBehaviour {

    bool hittable = false;
    [SerializeField]
    ParticleSystem smackStars;
    public Animator redFlickPanel;

    private void Reset()
    {
        redFlickPanel = FindObjectOfType<RedFlicker>().GetComponent<Animator>();
    }

    private void Awake()
    {
        if (smackStars == null)
            smackStars = GetComponentInChildren<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                checkTouch(Input.GetTouch(0).position);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            checkTouch(Input.mousePosition);
        }
#endif

    }

    private void checkTouch(Vector3 pos)
    {
        //print(Camera.main.ScreenToWorldPoint(pos));
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 touchPos = new Vector2(wp.x, wp.y);

        if (Vector2.Distance(touchPos,transform.position)<1)
        {
            if (hittable)
            {
                //print(gameObject.transform.parent.name);
                BecomeUnhittable();
                bool correctHit = tag == "Hedgehog" || tag == "Apple";
                WhackManager.instance.UpdateScore(correctHit);
                GetComponent<AudioSource>().Play();
                if (correctHit)
                    smackStars.Play();
                else
                {
                    redFlickPanel.Play("FlickRed");
                }
                    
            }
        }
    }
    private void BecomeHittable()
    {
        hittable = true;
    }
    private void BecomeUnhittable()
    {
        hittable = false;
    }

    public void TriggerHittable()
    {
        Invoke("BecomeHittable",0.5f);
    }
    public void TriggerUnhittable()
    {
        Invoke("BecomeUnhittable", 0.25f);
    }
}
