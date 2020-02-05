using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlantType
{
    Spread,Grass,Shrub,Tree,sSpecial
}
public class Plant
{
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
            return energyGiven < energyTotal;
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
                needs.Add(PlantType.Spread,4);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);
                needs.Add(PlantType.Grass,2);
                needsActual.Add(PlantType.Grass,0);
                needsMet.Add(PlantType.Grass,false);
                break;
            case PlantType.Tree:
                needs.Add(PlantType.Spread,8);
                needsActual.Add(PlantType.Spread,0);
                needsMet.Add(PlantType.Spread,false);
                needs.Add(PlantType.Grass,4);
                needsActual.Add(PlantType.Grass,0);
                needsMet.Add(PlantType.Grass,false);
                needs.Add(PlantType.Shrub,4);
                needsActual.Add(PlantType.Shrub,0);
                needsMet.Add(PlantType.Shrub,false);
                break;
        }
        gameObject = new GameObject(type.ToString());
        gameObject.transform.position = position;
        gameObject.tag = "Plant";
        BoxCollider c = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
        c.size = new Vector3(0.5f,0.5f,0.5f);
        gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(0,360),0);
        GameObject.Instantiate(Resources.Load(type.ToString()),gameObject.transform);
        gameObject.transform.localScale = minSize;
    }

    public void Update()//called each night to grow the plant
    {
        CheckNeeds();
        if(needsMetPercent> 0.5f){
            growthPercent+=0.1f;
        }
        gameObject.transform.localScale = Vector3.Lerp(minSize,maxSize,growthPercent);
    }
    public void Destroy(){
        //make a death event
        Services.EventManager.Fire(new PlantDestroyed(this));
        GameObject.Destroy(gameObject);
    }
    //check how your needs are being met
    public void CheckNeeds(){
        needsMetPercent = 0;
        if(type == PlantType.Spread){
            needsMetPercent = 1;
        }
        foreach(PlantType typePlant in needs.Keys){
            needsMet[typePlant] = needsActual[typePlant] >= needs[typePlant];
            needsMetPercent = needsActual[typePlant]/needs[typePlant];
        }
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