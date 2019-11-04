using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody myRigidbody;
    Vector3 inputVector;
    Vector3 startPos;
    public float speedRun;
    public float speedWalk;
    float speed;
    bool canSprint;
    // Use this for initialization
    void Start()
    {
        //startPos = new Vector3(1.45f, 1.24f, 29.45f);
        //transform.position = startPos;
        myRigidbody = GetComponent<Rigidbody>();
        canSprint = true;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        inputVector = transform.right * horizontalInput + transform.forward * verticalInput;

        if (inputVector.magnitude >1f)
        {
            inputVector = Vector3.Normalize(inputVector);
        }

        if (canSprint && Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = speedRun;
            Debug.Log("RUN");
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = speedWalk;
            Debug.Log("WALK");
        }
    }

    private void FixedUpdate()
    {

        if (true)
		{
			myRigidbody.velocity = inputVector * speed + Physics.gravity * 0.4f;
		}

	}
}
