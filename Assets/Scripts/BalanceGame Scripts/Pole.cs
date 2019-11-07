using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pole : MonoBehaviour {

    static public Pole S;
    [SerializeField] private float rotSpeed = 5.0f;
    [SerializeField] GameObject backButton;
    public float tiltZ = 0;
    public bool canTilt = true;
    //private bool _isFalling = false;
    private const float GRAVITY_MODIFIER = 1.01f;
    private const float ROTATION_TO_ADD = 0.25f;
    private const float RIGHT_ROT_BOUND = 268f;
    private const float LEFT_ROT_BOUND = 92f;

    void Awake()
    {
        S = this;
    }

    void Update()
    {
        CheckTiltBounds();
        Tilt(transform.rotation.eulerAngles.z);
        
        if (Input.GetKey(KeyCode.J))
        {
            transform.rotation = Quaternion.identity;
            canTilt = true;
            tiltZ = 0;
            BalancePlayer.S._tiltPower = 2;
            
        }
            
    }

	void Tilt(float currentZRot)
    {
        if (canTilt)
        {
            float addedRotation = 0;
            if (currentZRot < 360f && currentZRot > RIGHT_ROT_BOUND)
                addedRotation = -ROTATION_TO_ADD;
            else if (currentZRot > 0 && currentZRot < LEFT_ROT_BOUND)
                addedRotation = ROTATION_TO_ADD;
            else
            {
                if (Input.touchCount > 0)
                {
                    addedRotation = Random.Range(-ROTATION_TO_ADD, ROTATION_TO_ADD);
                    rotSpeed = 5f;
                }
            }
                

            tiltZ += addedRotation;
            rotSpeed *= GRAVITY_MODIFIER;
            float rotZ = tiltZ * Time.deltaTime * rotSpeed;
            transform.DOBlendableRotateBy(new Vector3(0, 0, rotZ), 0.1f, RotateMode.WorldAxisAdd);
        }
        else
            tiltZ = 0f;
    }

    public void CheckTiltBounds()
    {
        //Debug.Log("rotation.z: " + Mathf.Abs(transform.rotation.eulerAngles.z));
        if (canTilt)
        {
            if (transform.rotation.eulerAngles.z < RIGHT_ROT_BOUND && transform.rotation.eulerAngles.z > LEFT_ROT_BOUND)
            {
                canTilt = false;
                Score.S.AllowScoreCount(false);
                if (backButton != null)
                    backButton.SetActive(true);
                DOTween.KillAll();
                StartCoroutine(Panda.S.PlayerParticleEffect());

            }
        }          
    }
}
