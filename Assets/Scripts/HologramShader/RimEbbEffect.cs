using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimEbbEffect : MonoBehaviour
{
    public float power;
    public float offset;
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        if(offset != 0)
        {
            offset = Random.Range(0, 1000);
        }
        mr = GetComponent<MeshRenderer>();
        //Debug.Log(mr.material.GetFloat("_RimPower"));
    }

    // Update is called once per frame
    void Update()
    {
        power = Mathf.Lerp(0.5f,1f,Mathf.PerlinNoise(Time.time,offset));
        mr.material.SetFloat("_RimPower", power);
    }
}
