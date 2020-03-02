using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager
{
    public const float maxNeighborDistance = 1.5f;
    public const float maxPylonDistance = 10f;
    public List<Plant> plants;
    public List<Plant> newPlants;
    public List<Vector3> pylonPositions;
    public TextureEditor texEdit;

    public int numPlants{
        get{
            return plants.Count;
        }
    }
    public void Initialize(){
        plants = new List<Plant>();
        newPlants = new List<Plant>();
        Services.EventManager.Register<PlantDestroyed>(OnPlantDestroyed);
        Services.EventManager.Register<PlantJustFed>(OnPlantFed);
        pylonPositions = new List<Vector3>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Pylon")){
            CreateNewPylon(obj.transform.position);
        }
        Debug.Log(pylonPositions.Count);
    }
    public void CreateNewPylon(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, -Vector3.up, out hit))
        {
            texEdit.PaintCircle(hit.textureCoord, maxPylonDistance);
        }
        pylonPositions.Add(pos);
    }

    public bool CreateNewPlant(PlantType type, Vector3 pos){
        bool isCloseEnough = false;
        foreach(Vector3 v in pylonPositions){
            if(Vector3.Distance(v,pos) < maxPylonDistance){
                isCloseEnough = true;
                break;
            }
        }
        if(!isCloseEnough){
            return false;
        }
        foreach(Plant p in plants){
            if(p.type != type){
                continue;
            }
            float distance = Vector3.Distance(pos,p.position);
            if(type == PlantType.Tree){
                if(distance < 4f){
                    return false;
                }
            }else{
                if(distance < 0.8f+((int)type+1)*0.5f){
                    return false;
                }
            }
            
        }
        Plant plant = new Plant(type, pos);
        FindNeighbors(plant);
        newPlants.Add(plant);
        Services.EventManager.Fire(new PlantCreated());
        return true;
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
    //this should only happen if a plant JUST reached a point where it can support others
    void OnPlantFed(AGPEvent e){
        var pjf = (PlantJustFed)e;
        Plant plant = pjf.plant;
        if(plant.CanGiveEnergy == false){
            return;
        }
        foreach(Plant other in plants){
            if(other.CheckNeedThisPlant(plant)){
                other.PlantFullUpdate(plant);
            }
        }
    }
    // Update is called once per frame
    public void Update()
    {
        Debug.Log(plants.Count);
        foreach(Plant plant in plants){
            plant.Update();
        }
        foreach(Plant plant in newPlants){
            plants.Add(plant);
        }
        newPlants.Clear();
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
