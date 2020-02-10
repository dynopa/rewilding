using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI dayCounter;
    DateTime date;
    // Start is called before the first frame update
    void Awake()
    {
        date = DateTime.Now;
        InitializeServices();
    }
    void Update(){
        Services.PlantManager.Update();
        dayCounter.text = date.ToShortDateString();
        date = date.AddDays(1.0f);
    }
    void InitializeServices(){
        Services.GameController = this;


        Services.EventManager = new EventManager();

        Services.PlantManager = new PlantManager();
        Services.PlantManager.Initialize();

        
    }
}
