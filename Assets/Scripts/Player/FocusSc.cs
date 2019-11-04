using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.forward, out hit, 5) && hit.transform.tag == "Creature")
            {
                Creature creature = hit.transform.gameObject.GetComponent<Creature>();
            }
        }
    }
}
