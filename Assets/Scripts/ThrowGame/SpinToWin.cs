using UnityEngine;
using System.Collections;

public class SpinToWin : MonoBehaviour
{

    float f_lastX = 0.0f;
    float f_difX = 0.5f;
    //float f_lastY = 0.0f;
    //float f_difY = 0.5f;
    //float f_lastZ = 0.0f;
    //float f_difZ = 0.5f;
    //float f_steps = 0.0f;
    //int i_direction = 1;

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            f_difX = 0.0f;
            //f_difY = 0.0f;
            //f_difZ = 0.0f;
        }
        else if (Input.GetMouseButton(0))
        {
            f_difX = Mathf.Abs(f_lastX - Input.GetAxis("Mouse X"));

            if (f_lastX < Input.GetAxis("Mouse X"))
            {
                //i_direction = -1;
                transform.Rotate(Vector3.up, -f_difX);
            }

            if (f_lastX > Input.GetAxis("Mouse X"))
            {
                //i_direction = 1;
                transform.Rotate(Vector3.up, f_difX);
            }

            f_lastX = -Input.GetAxis("Mouse X");
        }
        else
        {
            if (f_difX > 0.5f) f_difX -= 0.05f;
            if (f_difX < 0.5f) f_difX += 0.05f;

            //transform.Rotate(Vector3.up, f_difX * i_direction);
        }
    }
}