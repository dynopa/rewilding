using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeRotate : MonoBehaviour
{
    public bool shouldRotate;
    public Vector3 endRotation;
    public float speed = 1f;
    Vector3 originalRotation;
    float duration;
    private void Awake()
    {
        originalRotation = transform.localEulerAngles;
    }
    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {
            duration += Time.deltaTime * speed;
            transform.localEulerAngles = Vector3.Lerp(originalRotation, originalRotation + endRotation, duration/endRotation.magnitude);
            if (duration/endRotation.magnitude > 1.0f)
            {
                Destroy(this);
            }
        }
    }
    public void Trigger()
    {
        if (!shouldRotate)
        {
            shouldRotate = true;
            originalRotation = transform.localEulerAngles;
        }
    }
}
