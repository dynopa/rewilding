using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeChild : MonoBehaviour
{
    public bool shouldChild;
    public Transform newParent;
    public Vector3 endPosition;//local to parent
    public float speed = 1f;
    Vector3 originalPosition;
    Vector3 originalRotation;
    float duration;
    float distance;
    // Update is called once per frame
    void Update()
    {
        if (shouldChild)
        {
            duration += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(originalPosition,newParent.position + endPosition, duration / distance);
            transform.localEulerAngles = Vector3.Lerp(originalRotation, Vector3.zero, duration/distance);
            if (duration / distance > 1.0f)
            {
                Destroy(this);
            }
        }
    }
    public void Trigger()
    {
        if (!shouldChild)
        {
            shouldChild= true;
            originalPosition = transform.position;
            originalRotation = transform.localEulerAngles;
            transform.parent = newParent;
            
            distance = Vector3.Distance(originalPosition, newParent.position + endPosition);
            Debug.Log(distance);
        }
    }
}
