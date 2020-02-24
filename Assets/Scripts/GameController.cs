using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI dayCounter;
    DateTime date;
    int frame = 0;
    string[] months = new string[]{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
    // Start is called before the first frame update
    void Awake()
    {
        date = DateTime.Now;
        InitializeServices();
    }
    void Update(){
        frame++;
        if(Input.GetMouseButtonDown(2)){
            Services.PlantManager.Update();
            dayCounter.text = date.Year +" "+months[date.Month-1];
            date = date.AddDays(7);
        }
        if(frame%4==0){
            
        }
        
    }
    void InitializeServices(){
        Services.GameController = this;


        Services.EventManager = new EventManager();

        Services.PlantManager = new PlantManager();
        Services.PlantManager.Initialize();

        
    }
}
