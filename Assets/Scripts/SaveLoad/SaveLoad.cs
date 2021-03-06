﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SaveLoad
{
    public static void Save(){
        Save save = new Save();
        save.plants = new List<PlantData>();
        save.id = Services.GameController.saveId;
        save.year = Services.GameController.date.Year;
        save.month = Services.GameController.date.Month;
        foreach(OldPlant plant in Services.PlantManager.plants){
            save.plants.Add(plant.Save());
        }
        string json = JsonUtility.ToJson(save);
        //Debug.Log(json);
        string path = null;
        path = "Assets/Resources/Save"+save.id+".json";
        System.IO.File.WriteAllText(path,json);
        Debug.Log(Services.PlantManager.plants.Count);
    }
    public static void Load()
    {
        string path = "Assets/Resources/Save"+Services.GameController.saveId+".json";
        string json = System.IO.File.ReadAllText(path);
        Save save = JsonUtility.FromJson(json,typeof(Save)) as Save;
        Services.GameController.date = new DateTime(save.year,save.month,1);
        foreach(OldPlant plant in Services.PlantManager.plants){
            plant.Destroy();
        }
        Services.PlantManager.plants.Clear();
        foreach(PlantData data in save.plants){
            OldPlant plant = new OldPlant((OldPlantType)data.plantType, data.position);
            plant.LoadPlant(data);
            Services.PlantManager.FindNeighbors(plant);
            Services.PlantManager.plants.Add(plant);
        }
        for(int i =0; i < 3;i++){
            if(Services.GameController.player.canAccessPlant[i+1] == false){
                //still trying to unlock this level
                if(Services.PlantManager.typeCount[i] >= Services.GameController.unlockLevels[i]){
                    Services.GameController.player.canAccessPlant[i+1] = true;
                }
            }
        }
    }
}
[System.Serializable]
public class Save
{
    public byte id;
    public int year;
    public int month;
    public List<PlantData> plants = new List<PlantData>();
}
