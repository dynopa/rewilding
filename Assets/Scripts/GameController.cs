using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InitializeServices();
    }
    void Update(){
        Services.PlantManager.Update();
    }
    void InitializeServices(){
        Services.GameController = this;


        Services.EventManager = new EventManager();

        Services.PlantManager = new PlantManager();
        Services.PlantManager.Initialize();

        
    }
}
