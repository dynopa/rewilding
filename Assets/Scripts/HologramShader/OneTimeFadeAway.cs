using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeFadeAway : MonoBehaviour
{
    public bool shouldFade;
    public float speed = 1;
    float duration;
    float originalPower;
    float distance;
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        originalPower = mr.material.GetFloat("_RimPower");
        distance = 5f - originalPower;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFade)
        {
            duration += Time.deltaTime * speed;
            mr.material.SetFloat("_RimPower", Mathf.Lerp(originalPower, 5f, duration/distance));
            if (duration/distance > 1.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }
    public void Trigger()
    {
        if (!shouldFade)
        {
            shouldFade = true;
        }
    }
}
