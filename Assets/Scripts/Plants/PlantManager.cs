using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager
{
    public List<Plant> plants;
    public int[] typeCount = new int[4];//cumulative
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
            CreateNewPylon(obj.transform.position,true);
        }
    }
    public void CreateNewPylon(Vector3 pos, bool dome)
    {
        if(firstTreeGrown ==false){
            firstTreeGrown = true;
            Services.EventManager.Fire(new FirstTreeGrown());
        }
        RaycastHit hit;
        if (Physics.Raycast(pos, -Vector3.up, out hit))
        {
            texEdit.PaintCircle(hit.textureCoord, dome ? Services.GameController.domePylonRadius : Services.GameController.pylonRadius);
        }
        pylonPositions.Add(pos);
    }
    public void CreateNewPylon(Vector3 pos){
        CreateNewPylon(pos, false);
    }
    public bool CloseToPylon(Vector3 pos){
        for(int i = 0; i < pylonPositions.Count;i++){
            Vector3 v  = pylonPositions[i];
            float distance = i==0 ? Services.GameController.domePylonRadius : Services.GameController.pylonRadius;
            if(Vector3.Distance(v,pos) < distance){
                return true;
            }
        }
        return false;
    }
    public Plant CreateNewPlant(PlantType type, Vector3 pos, bool playerPlaced = false){
        bool isCloseEnough = CloseToPylon(pos);
        if(!isCloseEnough){
            Debug.Log("Plant is not in pylon radius");
            return null;
        }
        if(!playerPlaced){
            foreach(Plant p in plants){
                float distance = Vector3.Distance(pos,p.position);
                float maxAllowedDistance = 0f;
                if(p.type != type){
                    //they're different
                    maxAllowedDistance = Services.GameController.plantInfo[(int)PlantInfo.collideDistanceForOthers,(int)type];
                }else{
                    maxAllowedDistance = Services.GameController.plantInfo[(int)PlantInfo.collideDistanceForSame,(int)type];
                }
                //this is for all the same
                if(distance < maxAllowedDistance){
                    if((int)p.type < (int)type-1){
                        //its below it, so just destroy it instead
                        p.Destroy();
                    }else{
                        Debug.Log("Plant is too close to other plants");
                        return null;
                    }
                    
                }
            }
            foreach(Plant p in newPlants){
                float distance = Vector3.Distance(pos,p.position);
                float maxAllowedDistance = 0f;
                if(p.type != type){
                    //they're different
                    maxAllowedDistance = Services.GameController.plantInfo[(int)PlantInfo.collideDistanceForOthers,(int)type];
                }else{
                    maxAllowedDistance = Services.GameController.plantInfo[(int)PlantInfo.collideDistanceForSame,(int)type];
                }
                //this is for all the same
                if(distance < maxAllowedDistance){
                    if((int)p.type < (int)type-1){
                        //its below it, so just destroy it instead
                        p.Destroy();
                    }else{
                        Debug.Log("Plant is too close to other plants");
                        return null;
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
        
        return plant;
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
            if(distance > Services.GameController.plantInfo[(int)PlantInfo.maxNeighborDistance,p1Level]){
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
            if(other.babies.Contains(plant)){
                other.babies.Remove(plant);
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
        foreach(Plant plant in deadPlants){
            plants.Remove(plant);
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
        for(int i =0; i < 3;i++){
            if(Services.GameController.player.canAccessPlant[i+1] == false){
                //still trying to unlock this level
                if(typeCount[i] >= Services.GameController.unlockLevels[i]){
                    Services.GameController.player.canAccessPlant[i+1] = true;
                }
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
    public bool DestroyPlantFromLocation(Vector3 v){
        //returns whether or not it actually deleted a plant
        foreach(Plant plant in plants){
            if(plant.dead){continue;}
            if(Vector3.Distance(plant.position,v) <Services.GameController.deleteDistance){
                plant.Destroy();
                return true;
            }
        }
        return false;
    }
    public void CreateNarrativeMoment(){
        if(Services.GameController.makeNarrativeEvents==false){
            return;
        }
        if (GameObject.FindGameObjectsWithTag("NarrObj").Length > 0)
        {

        }
        else
        {
            //random plant
            int plantNum = Random.Range(0, plants.Count);

            GameObject narrObj = GameObject.Instantiate(Resources.Load("NarrativeMoment"), plants[plantNum].gameObject.transform) as GameObject;
            //narrObj.transform.position = new Vector3(plants[plantNum].position.x, 0f, plants[plantNum].position.z);
            narrObj.transform.localPosition = new Vector3(0, 0f, 0);
        }
    }
}
