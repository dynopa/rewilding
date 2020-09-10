using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum lookObject
{
    none, ground, plant, door, narrObj
}

public class PlayerState : MonoBehaviour
{
    private Camera cam;


    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        Services.PlayerState = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public lookObject CastCheck()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance > 2.5f)
            {
                return lookObject.none;
            }
            else if (hit.transform.name == "Button")
            {
                return lookObject.door;
            }
            else if (hit.transform.CompareTag("NarrObj"))
            {
                return lookObject.narrObj;
            }
            else if (hit.transform.CompareTag("Ground"))
            {
                return lookObject.ground;
            }
            else if (hit.transform.CompareTag("Plant"))
            {
                return lookObject.plant;
            }
            else
            {
                return lookObject.none;
            }
        }
        else
        {
            return lookObject.none;
        }
    }
}
