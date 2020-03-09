using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    public byte saveId;
    public TextMeshProUGUI dayCounter;
    public TextureEditor texEdit;

    public DateTime date;
    int frame = 0;
    string[] months = new string[]{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
    // Start is called before the first frame update
    void Start()
    {
        saveId = 1;
        date = DateTime.Now;
        InitializeServices();
    }
    void Update(){
        dayCounter.text = date.Year +" "+months[date.Month-1];
        if(Input.GetKeyDown(KeyCode.F)){
            SaveLoad.Load();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            saveId = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            saveId = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            saveId = 3;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            saveId = 4;
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            saveId = 5;
        }
        if(Input.GetKeyDown(KeyCode.Alpha6)){
            saveId = 6;
        }
        if(Input.GetKeyDown(KeyCode.Alpha7)){
            saveId = 7;
        }
        if(Input.GetKeyDown(KeyCode.Alpha8)){
            saveId = 8;
        }
        if(Input.GetKeyDown(KeyCode.Alpha9)){
            saveId = 9;
        }
        if(Input.GetKeyDown(KeyCode.Alpha0)){
            saveId = 0;
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
