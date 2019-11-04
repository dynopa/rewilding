using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float mouseSensitivity = 150f;
    float verticalLookAngle = 0f;

    private void Start()
    {
        FadeIn(1f);
    }
    // Update is called once per frame
    void Update () {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        //transform.Rotate(-mouseY, 0f, 0);
        if (transform.parent != null)
        {
            transform.parent.Rotate(0f, mouseX, 0f);
        }
        //looking up/down code
        verticalLookAngle -= mouseY;
        verticalLookAngle = Mathf.Clamp(verticalLookAngle, -89f, 89f);

        transform.localEulerAngles = new Vector3
            (
                verticalLookAngle, 
                transform.localEulerAngles.y, 
                0
            );

	}

    public void FadeIn(float fadeTime)
    {

    }
}
