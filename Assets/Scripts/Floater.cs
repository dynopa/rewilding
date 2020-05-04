using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    Vector3 startPos;
    float startTime;
    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.localPosition;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = startPos + Vector3.up*Mathf.Sin(startTime+Time.time*2f)/5f;
    }
}
