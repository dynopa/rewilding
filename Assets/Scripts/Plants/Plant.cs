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
    public int babyLimit = 2;
    public bool grown;
    public byte stage=1;
    public int numBabies;
    public Vector3 position;
    public PlantType type;
    public List<Plant> neighbors;//other plants THAT COULD support this plant near this plant
    public Dictionary<PlantType,int> needs;//needs for the plant by type abstract
    float needsMetPercent;//whether its needs are met or not
    float growthPercent;//how far along this plant is to full-grown
    public bool shouldPlay = true;

    int energyTotal = 1;//how much enery can this plant give
    public int energyGiven = 0;

    //FMOD EVENT ASSIGNMENT
    public FMOD.Studio.EventInstance plantFood;
    public float sizeRandom;





    public bool CanGiveEnergy{
        get{
            return true;//energyGiven < energyTotal && needsMetPercent >= needsThreshold;
        }
    }
    public int GetState{
        get{
            if(withering){
                return 3;
            }
            if(needsMetPercent < 0.5f){
                return 2;
            }
            if(needsMetPercent > 0.5f){
                return 0;
            }
            return 1;
        }
    }
    public GameObject gameObject;
    MeshRenderer plantDisplay;
    Vector3 minSize = new Vector3(0.25f,0.25f,0.25f);
    Vector3 maxSize = Vector3.one;

    public bool withering;
    public int babiesPerDay;
    public float ratioNeeded;
    public List<Plant> babies = new List<Plant>();

    public Plant(PlantType type,Vector3 pos){
        stage = 1;
        this.type = type;
        ratioNeeded = Services.GameController.plantInfo[(int)PlantInfo.dependentRatio,(int)type];
        babiesPerDay = (int)Services.GameController.plantInfo[(int)PlantInfo.babiesPerDay,(int)type];
        babyLimit = (int)Services.GameController.plantInfo[(int)PlantInfo.babiesAllowed,(int)type];
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
                needs.Add(PlantType.Shrub,2);
                break;
        }
        gameObject = new GameObject(type.ToString());
        gameObject.transform.position = position;
        gameObject.tag = "Plant";
        BoxCollider c = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
        c.size = new Vector3(0.5f,0.5f,0.5f);
        c.isTrigger = true;
        if(type == PlantType.Tree){
            c.isTrigger = false;
            c.size = new Vector3(c.size.x,c.size.y*10f,c.size.z);
        }
        gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(0,360),0);
       
        GameObject.Instantiate(Resources.Load(type.ToString()+"_"+stage),gameObject.transform);
        gameObject.transform.GetChild(0).gameObject.AddComponent(typeof(ComeUpper));
        sizeRandom = Random.Range(0.8f,1.2f);
        gameObject.transform.localScale = minSize*sizeRandom;
        plantDisplay = gameObject.GetComponentInChildren<MeshRenderer>();
    }

    public void Update()//called each night to grow the plant
    {
        if(grown && babies.Count >= babyLimit){
            return;
        }
        if(!grown){
            float level = (int)type+1;
            CheckNeeds();
            
            if(!withering){
                if(needsMetPercent < 0.5f){
                    //make sure it will play bum note
                    shouldPlay = true;
                    withering = true;
                   
                }
            }else{
                if(needsMetPercent > 0f){
                    withering = false;
                    Services.EventManager.Fire(new PlantJustFed(this));
                   // Debug.Log("Hello");
                    //Services.EventManager.Fire
                }
                else{
                    Destroy();
                }
            }
            //growthPercent+=0.05f*needsMetPercent*(1.0f/level)*4;
            if(ReferenceEquals(plantDisplay,null)){
                if(withering){
                    plantDisplay.material.color = Color.black;
                }else{
                    if(needsMetPercent < 0.5f){
                        //about to wither
                        plantDisplay.material.color = Color.white;
                    }else if(needsMetPercent < 1.0f){
                        //half!
                        plantDisplay.material.color = Color.gray;
                    }else{
                        //good
                        plantDisplay.material.color = Color.white;
                    }
                }
            }
            growthPercent+=(Services.GameController.plantInfo[(int)PlantInfo.growthRate,(int)type]*needsMetPercent);
            Debug.Log(growthPercent);
            if(growthPercent >= stage){
                growthPercent = 0;
                stage++;
                if(stage > level){
                    grown = true;
                    growthPercent = 1.0f;
                }
                if(stage != 2){
                    //switch out the model
                    GameObject.Destroy(gameObject.transform.GetChild(0).gameObject);
                    GameObject.Instantiate(Resources.Load(type.ToString()+"_"+(stage-1)),gameObject.transform);
                    plantDisplay = gameObject.GetComponentInChildren<MeshRenderer>();
                }
                if(level == 3){
                    Debug.Log(stage+","+grown);
                }
                
            }
            if(grown){
                Services.EventManager.Fire(new PlantGrown(this));
                if(type == PlantType.Tree){
                    Services.PlantManager.CreateNewPylon(position);
                }
            }
            gameObject.transform.localScale = Vector3.Lerp(minSize,maxSize,growthPercent)*sizeRandom;
            return;
        }
        /*if(needsMetPercent> needsThreshold){
            growthPercent+=0.1f*needsMetPercent;
        }*/
        
        if(grown){
            CheckNeeds();
            //grow more plants!  && needsMetPercent >= Services.GameController.needsMetToHaveBaby[(int)type])
            if(Random.value <= Services.GameController.plantInfo[(int)PlantInfo.babyChance,(int)type]){
                for(int i = 0; i < babiesPerDay; i++){
                    if(HaveBaby()){
                        numBabies++;
                    }
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
        Vector3 newPosition = position+Random.insideUnitSphere*Random.Range(Services.GameController.plantInfo[(int)PlantInfo.minBabyDistance,(int)type],Services.GameController.plantInfo[(int)PlantInfo.maxBabyDistance,(int)type])*(level);
        newPosition.y = position.y+5f;
        RaycastHit hit;
        Ray ray = new Ray(newPosition,Vector3.down);
        if (Physics.Raycast(ray, out hit)){
            if(hit.collider.CompareTag("Ground") == false && hit.collider.CompareTag("Plant") == false){
                return false;
            }
            if(hit.collider.CompareTag("Ground")){
                newPosition.y = hit.point.y;
            }else{
                newPosition.y = position.y;
            }
            
            
        }else{
            newPosition.y = position.y;
        }
        Plant baby = Services.PlantManager.CreateNewPlant(type,newPosition);
        if(ReferenceEquals(baby,null)){
            return false;
        }else{
            babies.Add(baby);
            return true;
        }
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
            if(numLowerLevel >= numMyLevel*ratioNeeded){
                //Debug.Log("Hello");
                needsMetPercent = 1.0f;
            }else if(numLowerLevel >= numMyLevel*ratioNeeded/2f){
                needsMetPercent = 0.5f;
            }else{
                needsMetPercent = 0;
            }
        }
        
        if(met == false){
            if(needsMetPercent >= needsThreshold){
                Services.EventManager.Fire(new PlantJustFed(this));
                //Debug.Log("H");
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
        data.stage = stage;
        data.grown = grown;
        data.growthPercent = growthPercent;
        data.withering = withering;
        return data;
    }
    public void LoadPlant(PlantData data){
        position = data.position;
        type = (PlantType)data.plantType;
        stage = data.stage;
        if(stage != 1){
            GameObject.Destroy(gameObject.transform.GetChild(0).gameObject);
            GameObject.Instantiate(Resources.Load(type.ToString()+"_"+(stage-1)),gameObject.transform);
            plantDisplay = gameObject.GetComponentInChildren<MeshRenderer>();
        }
        grown = data.grown;
        growthPercent = data.growthPercent;
        withering = data.withering;
        if(growthPercent >= 1.0f){
            growthPercent = 1.0f;
            grown = true;
        }
        if(grown){
            if(type == PlantType.Tree){
                Services.PlantManager.CreateNewPylon(position);
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
    public byte stage;
    public bool grown;
    public float growthPercent;
    public bool withering;
}