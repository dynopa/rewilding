using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager
{
    public const float maxNeighborDistance = 5f;
    public List<Plant> plants;
    public int numPlants{
        get{
            return plants.Count;
        }
    }
    public void Initialize(){
        plants = new List<Plant>();
        Services.EventManager.Register<PlantDestroyed>(OnPlantDestroyed);
    }
    public void CreateNewPlant(PlantType type, Vector3 pos){
        Plant plant = new Plant(type, pos);
        FindNeighbors(plant);
        plants.Add(plant);
    }
    public void FindNeighbors(Plant p1){
        foreach(Plant p2 in plants){
            bool p1Needsp2 = p1.needs.ContainsKey(p2.type);
            bool p2Needsp1 = p2.needs.ContainsKey(p1.type);
            if(!p1Needsp2 && !p2Needsp1){
                //i don't need you so why am i even looking?
                continue;
            }
            float distance = Vector3.Distance(p1.position,p2.position);
            if(distance > maxNeighborDistance){
                continue;
            }
            if(p1Needsp2){
                if(!p1.neighbors.Contains(p2)){//add this plant to its neighbor
                    p1.neighbors.Add(p2);
                    p1.NewPlantUpdate(p2);
                }
            }
            if(p2Needsp1){
                if(!p2.neighbors.Contains(p1)){//add neighbor to this plant
                    p2.neighbors.Add(p1);
                    p2.NewPlantUpdate(p1);
                }
            }
            
        }
    }
    void OnPlantDestroyed(AGPEvent e){
        //var event = (PlantDestroyed) e;
        var pd = (PlantDestroyed)e;
        Plant plant = pd.plant;
        //remove plants from other people's lists
        foreach (Plant other in plants)
        {
            if(other.neighbors.Contains(plant)){
                other.RemovePlantUpdate(plant);
            }
        }
        plants.Remove(plant);
        
    }
    // Update is called once per frame
    public void Update()
    {
        Debug.Log(plants.Count);
        foreach(Plant plant in plants){
            plant.Update();
        }
    }
    public void DestroyPlantFromGameObject(GameObject g){
        foreach (Plant plant in plants)
        {
            if(plant.gameObject == g){
                plant.Destroy();
                break;
            }
        }
    }
}
