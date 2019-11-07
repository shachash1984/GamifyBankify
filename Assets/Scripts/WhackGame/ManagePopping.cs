using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagePopping : MonoBehaviour {

    float popUpTimer;
    bool friendlyBlob;       
    
    [SerializeField]
    HitHedgehog hedgehog;
    [SerializeField]
    HitHedgehog blob;

    // Use this for initialization
    void Start () {            
        RandomizeNextHedgehog();
    }
	
	// Update is called once per frame
	void Update () {
        if (WhackManager.instance.gameover || WhackManager.instance.gameStarted == false) return;
        
        popUpTimer -= Time.deltaTime;
        CheckTimer();
        

    }

    void RandomizeNextHedgehog()
    {
        popUpTimer = Random.Range(1f, 2f);
        friendlyBlob = Random.value >= 0.5f;
    }

    void CheckTimer()
    {
        if (popUpTimer<=0)
        {
            PopUp();
            RandomizeNextHedgehog();
        }
    }
    

    void PopUp()
    {
        if (friendlyBlob)
        {
            blob.GetComponent<Animator>().Play("BlobPopUp");
            blob.TriggerHittable();
            Invoke("BlobPopDown", Random.Range(1f, 2f));
        }
        else
        {            
            if (hedgehog.tag == "Hedgehog")
            {
                hedgehog.GetComponent<Animator>().Play("PopUp");
                hedgehog.TriggerHittable();
                Invoke("PopDown", Random.Range(1f, 2f));
            }
            if (hedgehog.tag == "Apple")
            {
                hedgehog.GetComponent<Animator>().Play("PopUpApple");
                hedgehog.TriggerHittable();
                Invoke("PopDownApple", Random.Range(1f, 2f));
            }
        }                   
    }

    void BlobPopDown()
    {
        blob.GetComponent<Animator>().Play("BlobPopDown");
        blob.TriggerUnhittable();
        RandomizeNextHedgehog();
    }
    void PopDown()
    {
        hedgehog.GetComponent<Animator>().Play("PopDown");
        hedgehog.TriggerUnhittable();
        RandomizeNextHedgehog();
    }
    void PopDownApple()
    {
        hedgehog.GetComponent<Animator>().Play("PopDownApple");
        hedgehog.TriggerUnhittable();
        RandomizeNextHedgehog();
    }


}
