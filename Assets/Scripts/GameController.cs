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
    collideDistanceForSame
}
public class GameController : MonoBehaviour
{
    public byte saveId;
    public TextMeshProUGUI dayCounter;
    public TextureEditor texEdit;
    [NamedArray(typeof(PlantInfo))] public float[] grassData;
    [NamedArray(typeof(PlantInfo))] public float[] bushData;
    [NamedArray(typeof(PlantInfo))] public float[] flowerData;
    [NamedArray(typeof(PlantInfo))] public float[] treeData;
    [HideInInspector]
    public float[] growthRate;
    [HideInInspector]
    public float[] chanceOfBaby;
    [HideInInspector]
    public float[] needsMetToHaveBaby;
    [HideInInspector]
    public float[] distanceForOthers;
    [HideInInspector]
    public float[] distanceForSame;
    public float pylonRadius;
    [HideInInspector]
    public float[] maxNeighborDistance;
    public DateTime date;
    int frame = 0;
    bool freshStart = false;
    bool fired = false;
    string[] months = new string[]{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
    // Start is called before the first frame update
    void Awake()
    {
        growthRate = new float[]{grassData[0],bushData[0],flowerData[0],treeData[0]};
        chanceOfBaby = new float[]{grassData[1],bushData[1],flowerData[1],treeData[1]};
        needsMetToHaveBaby = new float[]{grassData[2],bushData[2],flowerData[2],treeData[2]};
        distanceForOthers = new float[]{grassData[3],bushData[3],flowerData[3],treeData[3]};
        distanceForSame = new float[]{grassData[4],bushData[4],flowerData[4],treeData[4]};
        maxNeighborDistance = new float[]{grassData[5],bushData[5],flowerData[5],treeData[5]};
        saveId = 1;
        date = DateTime.Now;
        InitializeServices();
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
