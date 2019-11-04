using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimColorInfo : MonoBehaviour
{
    public int value;
    public List<Color> colors;
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        colors = new List<Color>()
        {
            Color.red,Color.green,Color.blue
        };
    }

    // Update is called once per frame
    void Update()
    {
        mr.material.SetColor("_Color", colors[value]);
        mr.material.SetColor("_RimColor",colors[value]);
    }
    public void SetValue(int v)
    {
        value = v;
    }
}
