using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public enum PlantInfo{
    growthRate,
    babyChance,
    needsMetToHaveBaby,
    maxNeighborDistance,
    collideDistanceForOthers,
    collideDistanceForSame,
    dependentRatio,
    babiesAllowed,
    babiesPerDay,
    plantCost,
    minBabyDistance,
    maxBabyDistance
}
public class GameController : MonoBehaviour
{
    public byte saveId;
    public TextMeshProUGUI dayCounter;
    public TextureEditor texEdit;
    public TextAsset levers;
    public float[,] plantInfo;
    public DateTime date;
    int frame = 0;
    bool freshStart = false;
    bool fired = false;
    string[] months = new string[]{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
    // Start is called before the first frame update
    [HideInInspector]
    public float pylonRadius;
    [HideInInspector]
    public float domePylonRadius;
    public bool makeNarrativeEvents;
    public PlayerController player;
    void Awake()
    {
        plantInfo = new float[12,4];
        ReadLevers();
        saveId = 1;
        date = DateTime.Now;
        date = date.AddYears(150);
        InitializeServices();
        //
    }
    void ReadLevers(){
        string leversString = levers.text;
        string[] leverLines = leversString.Split('\n');
        int whichPlant = -1;
        int whichVariable = 0;
        bool pylonStuff = true;
        for(int i = 0; i < leverLines.Length;i++){
            string line = leverLines[i];
            if(line.Contains("=")){//new variable
            string info = line.Split('=')[1].Trim();
                if(pylonStuff){
                    switch(whichVariable){
                        case 0:
                            pylonRadius = float.Parse(info);
                            break;
                        case 1:
                            domePylonRadius = float.Parse(info);
                            break;
                        case 2:
                            player.seedGainPerDay = (int)float.Parse(info);
                            break;
                        case 3:
                            player.seedPerDay = (int)float.Parse(info);
                            break;
                        case 4:
                            player.seedsLeft = (int)float.Parse(info);
                            break;
                    }
                    whichVariable++;
                    continue;
                }
                plantInfo[whichVariable,whichPlant] = float.Parse(info);
                whichVariable++;
                continue;
            }
            if(line.Contains("-")){
                if(pylonStuff){
                    pylonStuff = false;
                }
                whichVariable = 0;
                whichPlant++;
                continue;
            }
        }
    }
    void Update(){
        if(!fired)
        {
            fired = true;
            Services.EventManager.Fire(new GameStart());
        }
        if((Time.time > 30 || Services.PlantManager.plants.Count >= 10) && freshStart == false){
            Services.EventManager.Fire(new After30Seconds());
            freshStart = true;
        }
        dayCounter.text = date.Year +" "+months[date.Month-1];
        if(Input.GetKeyDown(KeyCode.F)){
            SaveLoad.Load();
        }
        if(Input.GetKeyDown(KeyCode.G)){
            SaveLoad.Save();
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
