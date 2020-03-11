using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetAxis("Fire1") == 1)
        {
            LoadGame();
        }
    }
    public void LoadGame()
    {
    	SceneManager.LoadScene("ControllerTest", LoadSceneMode.Single);
    }
}
