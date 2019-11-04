using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeMove : MonoBehaviour
{
    public bool shouldMove;
    public Vector3 endDirection;
    public float speed = 1f;
    Vector3 originalPosition;
    float duration;
    float distance;
    // Update is called once per frame
    private void Awake()
    {
        originalPosition = transform.position;
        distance = endDirection.magnitude;
    }
    void Update()
    {
        if (shouldMove)
        {
            duration += Time.deltaTime*speed;
            transform.position = Vector3.Lerp(originalPosition, originalPosition+endDirection, duration/distance);
            if(duration/distance > 1.0f)
            {
                Destroy(this);
            }
        }
    }
    public void Trigger()
    {
        if (!shouldMove)
        {
            shouldMove = true;
            originalPosition = transform.position;
        }
    }
}
