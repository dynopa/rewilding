using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMover : MonoBehaviour
{
    public Vector2 pos1;
    public Vector2 pos2;
    private Vector2 startPos;
    private float lerpStartTime;
    public float lerpLength;
    private RectTransform rTransform;
    private float timePassed;

    // Start is called before the first frame update
    void Start()
    {
        rTransform = gameObject.GetComponent<RectTransform>();
        startPos = rTransform.anchoredPosition;
        pos1 = startPos;

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime/2;
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - lerpStartTime) * 2f;
        float squatProg = distCovered / lerpLength;
        rTransform.anchoredPosition = Vector2.Lerp(pos1, pos2, Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time / lerpLength, 1)));
        
        if (squatProg > 1)
        {
            
        }
    }
}
