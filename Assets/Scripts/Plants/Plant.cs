using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlantType
{
    Spread,Grass,Shrub,Tree,sSpecial
}
public class Plant
{
    public const float needsThreshold = 0.25f;
    public const int babyLimit = 2;
    public bool grown;
    public int numBabies;
    public Vector3 position;
    public PlantType type;
    public List<Plant> neighbors;//other plants THAT COULD support this plant near this plant
    List<Plant> bonds;//plants that this plant is already bonded with
    public Dictionary<PlantType,int> needs;//needs for the plant by type abstract
    Dictionary<PlantType, int> needsActual;
    Dictionary<PlantType, bool> needsMet;
    float needsMetPercent;//whether its needs are met or not
    float growthPercent;//how far along this plant is to full-grown

    int energyTotal = 1;//how much enery can this plant give
    public int energyGiven = 0;
    public bool CanGiveEnergy{
        get{
            return energyGiven < energyTotal && needsMetPercent >= needsThreshold;
        }
    }
    public GameObject gameObject;
    Vector3 minSize = new Vector3(0.25f,0.25f,0.25f);
    Vector3 maxSize = Vector3.one;


    public Plant(PlantType type,Vector3 pos){
        this.type = type;
        position = pos;
        needsMetPercent = 0;
        growthPercent = 0;
        neighbors = new List<Plant>();
        bonds = new List<Plant>();
        needs = new Dictionary<PlantType, int>();
        needsActual = new Dictionary<PlantType, int>();
        needsMet = new Dictionary<PlantType, bool>();
        switch(type){
            case PlantType.Grass:
                needs.Add(PlantType.Spread,2);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);
                break;
            case PlantType.Shrub:
                /*needs.Add(PlantType.Spread,3);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);*/
                needs.Add(PlantType.Grass,2);
                needsActual.Add(PlantType.Grass,0);
                needsMet.Add(PlantType.Grass,false);
                break;
            case PlantType.Tree:
                /*needs.Add(PlantType.Spread,4);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);
                needs.Add(PlantType.Grass,3);
                needsActual.Add(PlantType.Grass,0);
                needsMet.Add(PlantType.Grass,false);*/
                needs.Add(PlantType.Shrub,2);
                needsActual.Add(PlantType.Shrub,0);
                needsMet.Add(PlantType.Shrub,false);
                break;
        }
        gameObject = new GameObject(type.ToString());
        gameObject.transform.position = position;
        gameObject.tag = "Plant";
        BoxCollider c = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
        c.size = new Vector3(0.5f,0.5f,0.5f);
        c.isTrigger =true;
        gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(0,360),0);
       
        GameObject.Instantiate(Resources.Load(type.ToString()),gameObject.transform);
        gameObject.transform.localScale = minSize;
    }

    public void Update()//called each night to grow the plant
    {
        if(grown && numBabies >= babyLimit){
            return;
        }
        if(!grown){
            float level = (int)type+1;
            CheckNeeds();
            growthPercent+=0.05f*needsMetPercent*(1.0f/level)*4;
            if(growthPercent >= 1.0f){
                growthPercent = 1.0f;
                grown = true;
            }
            if(grown){
                Services.EventManager.Fire(new PlantGrown(this));
                if(type == PlantType.Tree){
                    Services.PlantManager.pylonPositions.Add(position);
                }
            }
            gameObject.transform.localScale = Vector3.Lerp(minSize,maxSize,growthPercent);
            return;
        }
        /*if(needsMetPercent> needsThreshold){
            growthPercent+=0.1f*needsMetPercent;
        }*/
        
        if(grown){
            //grow more plants!
            if(Random.value < 0.5f){
                if(HaveBaby()){
                    numBabies++;
                }
            }
            
        }
    }
    public void Destroy(){
        //make a death event
        Services.EventManager.Fire(new PlantDestroyed(this));
        GameObject.Destroy(gameObject);
    }
    public bool HaveBaby(){
        float level = (int)type+1;
        Vector3 newPosition = position+Random.insideUnitSphere*Random.Range(2f,4f)*(level);
        newPosition.y = position.y+5f;
        RaycastHit hit;
        Ray ray = new Ray(newPosition,Vector3.down);
        if (Physics.Raycast(ray, out hit)){
            if(hit.collider.CompareTag("Ground") == false){
                return false;
            }
            newPosition.y = hit.point.y;
        }else{
            newPosition.y = position.y;
        }
        return Services.PlantManager.CreateNewPlant(type,newPosition);
    }
    //check how your needs are being met
    public void CheckNeeds(){
        bool met = needsMetPercent>=needsThreshold;
        needsMetPercent = 0;
        if(type == PlantType.Spread){
            needsMetPercent = 1;
        }
        foreach(PlantType typePlant in needs.Keys){
            needsMet[typePlant] = needsActual[typePlant] >= needs[typePlant];
            needsMetPercent = (float)needsActual[typePlant]/(float)needs[typePlant];
        }
        if(met == false){
            if(needsMetPercent >= needsThreshold){
                Services.EventManager.Fire(new PlantJustFed(this));
            }
        }
    }
    public bool CheckNeedThisPlant(Plant plant){
        if(!needsMet.ContainsKey(plant.type)){
            return false;
        }
        return !needsMet[plant.type];
    }

    //this is called when a new plant is added to your neighbors
    public void NewPlantUpdate(Plant newPlant){
        //you need it!
        if(needsMet[newPlant.type] != true){
            if(newPlant.CanGiveEnergy){
                CreateBond(newPlant);
            }
        }
        CheckNeeds();
    }
    //when a plant you are neighbors with is updated to be able to need fulfilling
    public void PlantFullUpdate(Plant newPlant){
        if(needsMet[newPlant.type] != true){
            if(newPlant.CanGiveEnergy){
                CreateBond(newPlant);
            }
        }
        CheckNeeds();
    }
    void CreateBond(Plant plant){
        needsActual[plant.type]++;
        plant.energyGiven++;
        bonds.Add(plant);
    }
    void RemoveBond(Plant plant){
        bonds.Remove(plant);
        plant.energyGiven--;
        needsActual[plant.type]--;
    }
    //this is called when a plant is removed from your neighbors
    public void RemovePlantUpdate(Plant newPlant){
        if(bonds.Contains(newPlant)){//you're using this plant's energy!!
            RemoveBond(newPlant);
        }else{
            neighbors.Remove(newPlant);
        }
        CheckNeeds();
        CheckNeighborsForNeeds();
    }
    //check neighbors you already have for more energy
    void CheckNeighborsForNeeds(){
        foreach(PlantType typePlant in needs.Keys){
            if(needsMet[typePlant]){
                continue;//you're already good!
            }
            foreach(Plant neighbor in neighbors){
                if(neighbor.type != typePlant){
                    continue;
                }
                if(bonds.Contains(neighbor)){
                    continue;//you're already using this joke
                }
                if(neighbor.CanGiveEnergy == false){
                    continue;//already given all its energy away
                }
                CreateBond(neighbor);
                if(needsActual[typePlant] >= needs[typePlant]){
                    break;
                }
            }
        }
    }
}