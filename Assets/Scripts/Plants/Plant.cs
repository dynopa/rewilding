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
    public Dictionary<PlantType,int> needs;//needs for the plant by type abstract
    float needsMetPercent;//whether its needs are met or not
    float growthPercent;//how far along this plant is to full-grown

    int energyTotal = 1;//how much enery can this plant give
    public int energyGiven = 0;
    public bool CanGiveEnergy{
        get{
            return true;//energyGiven < energyTotal && needsMetPercent >= needsThreshold;
        }
    }
    public GameObject gameObject;
    Vector3 minSize = new Vector3(0.25f,0.25f,0.25f);
    Vector3 maxSize = Vector3.one;

    public bool withering;

    public Plant(PlantType type,Vector3 pos){
        this.type = type;
        position = pos;
        needsMetPercent = 0;
        growthPercent = 0;
        neighbors = new List<Plant>();
        needs = new Dictionary<PlantType, int>();
        switch(type){
            case PlantType.Grass:
                needs.Add(PlantType.Spread,2);
                break;
            case PlantType.Shrub:
                /*needs.Add(PlantType.Spread,3);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);*/
                needs.Add(PlantType.Grass,2);
                break;
            case PlantType.Tree:
                /*needs.Add(PlantType.Spread,4);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);
                needs.Add(PlantType.Grass,3);
                needsActual.Add(PlantType.Grass,0);
                needsMet.Add(PlantType.Grass,false);*/
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
            if(!withering){
                if(needsMetPercent < 0.5f){
                    withering = true;
                }
            }else{
                if(needsMetPercent > 0f){
                    withering = false;
                }else{
                    Destroy();
                }
            }
            //growthPercent+=0.05f*needsMetPercent*(1.0f/level)*4;
            growthPercent+=(1*needsMetPercent)*(1.0f/level);
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
        }else{
            int numMyLevel = 1;
            int numLowerLevel = 0;
            foreach(Plant neighbor in neighbors){
                if(neighbor.type == type){
                    numMyLevel++;
                }else{
                    numLowerLevel++;
                }
            }
            if(numLowerLevel >= numMyLevel*2){
                needsMetPercent = (float)numMyLevel/(float)numLowerLevel;
            }else if(numLowerLevel >= numMyLevel){
                needsMetPercent = 0.5f;
            }else{
                needsMetPercent = 0;
            }
        }
        
        if(met == false){
            if(needsMetPercent >= needsThreshold){
                Services.EventManager.Fire(new PlantJustFed(this));
            }
        }
    }
    public bool CheckNeedThisPlant(Plant plant){
        return needs.ContainsKey(plant.type) || plant.type == type;
    }

    //this is called when a new plant is added to your neighbors
    public void NewPlantUpdate(Plant newPlant){
        //you need it!
        CheckNeeds();
    }
    //when a plant you are neighbors with is updated to be able to need fulfilling
    public void PlantFullUpdate(Plant newPlant){
        CheckNeeds();
    }
    //this is called when a plant is removed from your neighbors
    public void RemovePlantUpdate(Plant newPlant){
        neighbors.Remove(newPlant);
        CheckNeeds();
    }
    public PlantData Save(){
        PlantData data = new PlantData();
        data.position = position;
        data.plantType = (int)type;
        data.grown = grown;
        data.growthPercent = growthPercent;
        data.withering = withering;
        return data;
    }
    public void LoadPlant(PlantData data){
        position = data.position;
        type = (PlantType)data.plantType;
        grown = data.grown;
        growthPercent = data.growthPercent;
        withering = data.withering;
        if(growthPercent >= 1.0f){
            growthPercent = 1.0f;
            grown = true;
        }
        if(grown){
            if(type == PlantType.Tree){
                Services.PlantManager.pylonPositions.Add(position);
            }
        }
        gameObject.transform.localScale = Vector3.Lerp(minSize,maxSize,growthPercent);
    }
}
[System.Serializable]
public class PlantData
{
    public Vector3 position;
    public int plantType;
    //int stage;
    public bool grown;
    public float growthPercent;
    public bool withering;
}