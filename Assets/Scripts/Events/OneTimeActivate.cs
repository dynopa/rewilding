using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeActivate : MonoBehaviour
{
    public GameObject objectToActivate;
    // Start is called before the first frame update
    public void Trigger()
    {
        objectToActivate.SetActive(true);
        Destroy(this);
    }
}
