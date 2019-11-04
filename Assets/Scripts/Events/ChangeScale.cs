using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScale : MonoBehaviour
{
    public bool shouldChange;
    public Vector3 endScale;
    public float speed = 1f;
    Vector3 originalScale;
    float duration;
    // Update is called once per frame
    private void Awake()
    {
        originalScale = transform.localScale;
        endScale = new Vector3(endScale.x * originalScale.x, endScale.y * originalScale.y, endScale.z * originalScale.z);

    }
    void Update()
    {
        if (shouldChange)
        {
            duration += Time.deltaTime * 0.25f * speed;
            transform.localScale = Vector3.Lerp(originalScale, endScale, duration);
            if (duration > 1.0f)
            {
                Destroy(this);
            }
        }
    }
    public void Trigger()
    {
        if (!shouldChange)
        {
            shouldChange = true;
        }
    }
}
