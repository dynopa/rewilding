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

    public void CreateNewPlant(PlantType type, Vector3 pos){
        Plant plant = new Plant(type, pos);
        FindNeighbors(plant);

    }
    public void DestroyPlant(Plant plant){
        //remove its connections to other plants
        //when I have the EventManager working this will be grand
        plants.Remove(plant);
        plant.Destroy();
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
