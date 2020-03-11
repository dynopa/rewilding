using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager
{
    public const float maxNeighborDistance = 1.5f;
    public const float maxPylonDistance = 10f;
    public List<Plant> plants;
    public List<Plant> newPlants;
    public List<Plant> deadPlants;
    public List<Vector3> pylonPositions;
    public TextureEditor texEdit;
    bool firstTreePlanted;
    public bool firstTreeGrown;
    bool tooManyPlants;

    public int numPlants{
        get{
            return plants.Count;
        }
    }
    public void Initialize(){
        plants = new List<Plant>();
        newPlants = new List<Plant>();
        deadPlants = new List<Plant>();
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
        if(firstTreeGrown ==false){
            firstTreeGrown = true;
            Services.EventManager.Fire(new FirstTreeGrown());
        }
        RaycastHit hit;
        if (Physics.Raycast(pos, -Vector3.up, out hit))
        {
            texEdit.PaintCircle(hit.textureCoord, maxPylonDistance);
        }
        pylonPositions.Add(pos);
    }
    public bool CloseToPylon(Vector3 pos){
        foreach(Vector3 v in pylonPositions){
            if(Vector3.Distance(v,pos) < maxPylonDistance){
                return true;
            }
        }
        return false;
    }
    public bool CreateNewPlant(PlantType type, Vector3 pos, bool playerPlaced = false){
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
        if(!playerPlaced){
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
        }
        
        Plant plant = new Plant(type, pos);
        if(type == PlantType.Tree && firstTreePlanted == false){
            firstTreePlanted = true;
            Services.EventManager.Fire(new FirstTreePlanted());
        }
        FindNeighbors(plant);
        if(playerPlaced){
            plants.Add(plant);
        }else{
            newPlants.Add(plant);
        }
        Services.EventManager.Fire(new PlantCreated(plant));
        return true;
    }
    public void FindNeighbors(Plant p1){
        foreach(Plant p2 in plants){
            byte p1Level = (byte)p1.type;
            byte p2Level = (byte)p2.type;
            if(p1Level == p2Level || Mathf.Abs(p1Level-p2Level) == 1){

            }else{
                continue;
            }
            float distance = Vector3.Distance(p1.position,p2.position);
            if(distance > maxNeighborDistance){
                continue;
            }
            if(!p1.neighbors.Contains(p2)){//add this plant to its neighbor
                    p1.neighbors.Add(p2);
                    p1.NewPlantUpdate(p2);
                }
            if(!p2.neighbors.Contains(p1)){//add neighbor to this plant
                    p2.neighbors.Add(p1);
                    p2.NewPlantUpdate(p1);
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
        deadPlants.Add(plant);
        
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
        Services.GameController.date = Services.GameController.date.AddMonths(1);
        if(Services.GameController.date.Month == 4){
            Services.EventManager.Fire(new Day2());
        }
        //Debug.Log(plants.Count);
        foreach(Plant plant in plants){
            plant.Update();
        }
        //this deals with new plants grown from other plants
        foreach(Plant plant in newPlants){
            plants.Add(plant);
        }
        foreach(Plant plant in deadPlants){
            plants.Remove(plant);
        }
        if(tooManyPlants == false){
            if(plants.Count >= 40){
                tooManyPlants = true;
                Services.EventManager.Fire(new TooManyPlants());
            }
        }
        newPlants.Clear();
        deadPlants.Clear();
        SaveLoad.Save();
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
