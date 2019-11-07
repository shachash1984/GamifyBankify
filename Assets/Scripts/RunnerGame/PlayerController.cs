using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed = 7;
    public float jumpForce = 1000;
    public float jumpFuel;
    public float maxFuel = 1;

    public bool playing = false;
    public bool gameover = false;

    [SerializeField]
    Text score;
    [SerializeField]
    GameObject howToPlay;
    [SerializeField]
    GameObject backButton;

    bool jumped = false;
    Rigidbody rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
        jumpFuel = maxFuel;
	}
	
	// Update is called once per frame
	void Update () {

        if (playing)
        {
            MovePlayer();
            CheckInput();
        }
        else if (Input.touchCount>0 && gameover == false)
        {
            playing = true;
            howToPlay.SetActive(false);
        }

    }

    private void MovePlayer()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    private void CheckInput()
    {        
        if (Input.GetKey(KeyCode.Space) || (Input.touchCount>0 && Input.GetTouch(0).position.y > Screen.height/2))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.DownArrow) || (Input.touchCount > 0 && Input.GetTouch(0).position.y < Screen.height / 2))
        {
            Crouch();
        }
        else
            Crouch(false);


    }

    private void Crouch(bool shouldCrouch = true)
    {
        if (!jumped)
        {
            if (shouldCrouch)
            {
                GetComponent<BoxCollider>().center = Vector3.down * 0.25f;
                GetComponent<BoxCollider>().size = Vector3.one - Vector3.up * 0.50f;

            }
            else
            {
                GetComponent<BoxCollider>().center = Vector3.zero;
                GetComponent<BoxCollider>().size = Vector3.one;
            }
        }
    }
    private void Jump()
    {
        if (jumpFuel > 0)
        {
            rigid.AddForce(Vector3.up * jumpForce * Time.deltaTime);
            jumpFuel -= Time.deltaTime;
        }
        else
        {
            jumped = true;
        }
        if (Input.touchCount == 0 && transform.position.y > 0)
        {
            jumped = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            jumpFuel = maxFuel;
            jumped = false;
        }
        else
        {
            //gameover
            speed = 0;
            gameover = true;
            backButton.SetActive(true);
            TrapSpawner.StopSpawning();
            score.text = "You passed " + transform.position.x.ToString("F2") + " meters!";
        }
    }
}
