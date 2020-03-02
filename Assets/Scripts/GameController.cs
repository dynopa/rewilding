using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI dayCounter;
    public TextureEditor texEdit;

    public DateTime date;
    int frame = 0;
    string[] months = new string[]{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
    // Start is called before the first frame update
    void Start()
    {
        date = DateTime.Now;
        InitializeServices();
    }
    void Update(){
        dayCounter.text = date.Year +" "+months[date.Month-1];
        if(Input.GetKeyDown(KeyCode.F)){
            SaveLoad.Load();
        }
        
    }
    void InitializeServices(){
        Services.GameController = this;
      

        Services.EventManager = new EventManager();

        Services.PlantManager = new PlantManager();

        Services.PlantManager.texEdit = texEdit;
        Services.PlantManager.Initialize();


    }
}
