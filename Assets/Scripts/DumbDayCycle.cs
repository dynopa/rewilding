using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbDayCycle : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(transform.localEulerAngles.x > 30f && transform.localEulerAngles.x < 150f){
            transform.Rotate(Vector3.right*0.25f,Space.Self);
        }else{
            transform.Rotate(Vector3.right*3,Space.Self);
        }
    }
}
