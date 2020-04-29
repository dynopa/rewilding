using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComeUpper : MonoBehaviour
{
    Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
        transform.position += Vector3.down*2f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (targetPos-transform.position)*0.25f;
        if(Vector3.Distance(targetPos,transform.position) < 0.005f){
            Destroy(this);
        }
    }
}
