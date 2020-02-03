using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlantType
{
    None,Spread,Grass,Shrub,Tree,Special,Delete
}

public class Plant : MonoBehaviour
{
    public string desc;
    public PlantType type;
    public List<string> prefabs = new List<string>() {"None", "Spread", "Grass", "Shrub", "Tree", "Special" };
    public List<Plant> neighbors = new List<Plant>();
    public List<LineRenderer> lines = new List<LineRenderer>();//shows the plants this plant supports
    public List<Plant> supportedBy = new List<Plant>();
    public List<Plant> dependents = new List<Plant>();
    public float[] needs = new float[] { 0, 0, 0, 0, 0, 0 };//how much of each thing to get to 100%
    float[] needsMet = new float[] { 0, 0, 0, 0, 0, 0 };//how much does it have rn
    public float supportNum = 1;//how many can this single thing support?
    public bool met = false;
    public float gradientSupport = 0; //1 means fully supported
    MeshRenderer mr;
    Material good;
    public Material bad;

    GameObject child;
    public float size = 1;
    int age;
    public float growthStage;
    public bool used; //if this is true, its used up all its resources to support others
    float tempSupported; //used to check how much they're supporting in a day
    public bool[] filled = new bool[] { false, false, false, false, false, false };//true means its needs are met!

    // Start is called before the first frame update
    public void Age()
    {
        age++;
        size += ((1 - size) * 0.1f)*gradientSupport;
        if(size> 0.95f)
        {
            size = 1f;
            growthStage = 1;
        }
        
    }
    void Awake()
    {
        filled = new bool[] { false, false, false, false, false, false };
    }

    // Update is called once per frame
    void Update()
    {
        gradientSupport = CheckNeeds();
        met = gradientSupport >= 0.5f;
        if (mr)
        {
            if(good == null)
            {
                good = mr.material;
            }
            mr.material = met ? good : bad;
            child.transform.localScale = new Vector3(1, 1, 1);//met ? new Vector3(1, 1, 1)*size : new Vector3(0.5f, 0.5f, 0.5f)*size;
        }
        foreach(LineRenderer lr in lines)
        {
            if (lr.enabled)
            {
                if (PlantNeighborManager.instance.showLines)
                {
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;
                }
                else
                {
                    lr.startWidth = 0;
                    lr.endWidth = 0;
                }
            }
        }
    }
    public void SetType(PlantType newType)
    {
        gameObject.name = newType.ToString();
        type = newType;
        if (type != PlantType.None && !child)
        {
            string modelName = prefabs[(int)type];
            child = Instantiate(Resources.Load(modelName), transform) as GameObject;
            size = child.transform.localScale.x*0.3f;
            age = 20;
            growthStage = 0;
            mr = child.GetComponent<MeshRenderer>();
            supportNum = 1;
            switch (type)
            {
                case PlantType.Spread:
                    supportNum = 1;
                    break;
                case PlantType.Grass:
                    needs = new float[6];
                    needs[1] = 2;
                    break;
                case PlantType.Shrub:
                    needs = new float[6];
                    needs[1] = 4;
                    needs[2] = 2;
                    break;
                case PlantType.Tree:
                    needs = new float[6];
                    needs[1] = 8;
                    needs[2] = 4;
                    needs[3] = 2;
                    break;
                default:
                    needs = new float[6];
                    break;
            }
        }
        UpdateDependecies();
        UpdateDependecies();
    }
    public void NewDay()
    {
        tempSupported = 0;
        used = false;
        for(int i = 0; i < filled.Length; i++)
        {
            filled[i] = CheckNeedsByType((PlantType)i) >= 1.0f ;
        }
        for (int i = 0; i < neighbors.Count; i++)
        {
            lines[i].SetPosition(1, neighbors[i].transform.position + Vector3.up * 0.25f);
        }
    }
    public void UpdateDependecies()
    {
        NewDay();
        Debug.Log("FUCK");
        switch (type)
        {
            case PlantType.None:
                foreach(LineRenderer line in lines)
                {
                    line.enabled = false;
                }
                break;
            case PlantType.Spread:
                for(int i = 0; i < neighbors.Count; i++)
                {
                    if (neighbors[i].type == PlantType.Spread || neighbors[i].type == PlantType.None)
                    {
                        lines[i].enabled = false;
                    }
                    else if (!used && !neighbors[i].filled[(int)type])
                    {
                        lines[i].enabled = true;
                        dependents.Add(neighbors[i]);
                        neighbors[i].supportedBy.Add(this);
                        tempSupported++;
                        if (tempSupported >= supportNum)//they're supporting all they can
                        {
                            used = true;
                            lines[i].startColor = Color.green;
                        }
                        float percent = neighbors[i].CheckNeedsByType(type);
                        lines[i].startWidth = percent * 0.10f;
                        lines[i].endWidth = percent * 0.10f;
                        filled[i] = CheckNeedsByType((PlantType)i) >= 1.0f;
                        neighbors[i].filled[(int)type] = CheckNeedsByType(type) >= 1.0f;
                        //neighbors[i].filled[]
                    }
                    else
                    {
                        lines[i].enabled = false;
                    }
                }
                break;
            case PlantType.Grass:
                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (neighbors[i].type == PlantType.Grass || neighbors[i].type == PlantType.None)
                    {
                        lines[i].enabled = false;
                    }else if(neighbors[i].type == PlantType.Spread)
                    {
                        lines[i].enabled = false;
                        //supportedBy.Add(neighbors[i]);
                    }
                    else if (!used && !neighbors[i].filled[(int)type])
                    {
                        lines[i].enabled = true;
                        dependents.Add(neighbors[i]);
                        neighbors[i].supportedBy.Add(this);
                        tempSupported++;
                        if (tempSupported >= supportNum)//they're supporting all they can
                        {
                            used = true;
                            lines[i].startColor = Color.green;
                        }
                        float percent = neighbors[i].CheckNeedsByType(type);
                        lines[i].startWidth = percent * 0.10f;
                        lines[i].endWidth = percent * 0.10f;
                        neighbors[i].filled[(int)type] = CheckNeedsByType(type) >= 1.0f;
                    }
                    else
                    {
                        lines[i].enabled = false;
                    }
                }
                break;
            case PlantType.Shrub:
                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (neighbors[i].type == PlantType.Shrub || neighbors[i].type == PlantType.None)
                    {
                        lines[i].enabled = false;
                    }else if(neighbors[i].type == PlantType.Grass || neighbors[i].type == PlantType.Spread)
                    {
                        lines[i].enabled = false;
                        //supportedBy.Add(neighbors[i]);
                    }
                    else if (!used && !neighbors[i].filled[(int)type])
                    {
                        lines[i].enabled = true;
                        dependents.Add(neighbors[i]);
                        neighbors[i].supportedBy.Add(this);
                        tempSupported++;
                        if (tempSupported >= supportNum)//they're supporting all they can
                        {
                            used = true;
                            lines[i].startColor = Color.green;
                        }
                        float percent = neighbors[i].CheckNeedsByType(type);
                        lines[i].startWidth = percent * 0.10f;
                        lines[i].endWidth = percent * 0.10f;
                        filled[i] = CheckNeedsByType((PlantType)i) >= 1.0f;
                        neighbors[i].filled[(int)type] = CheckNeedsByType(type) >= 1.0f;
                    }
                    else
                    {
                        lines[i].enabled = false;
                    }
                }
                break;
            case PlantType.Tree:
                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (neighbors[i].type == PlantType.Tree || neighbors[i].type == PlantType.None)
                    {
                        lines[i].enabled = false;
                    }else if (neighbors[i].type == PlantType.Shrub || neighbors[i].type == PlantType.Grass || neighbors[i].type == PlantType.Spread)
                    {
                        lines[i].enabled = false;
                        //supportedBy.Add(neighbors[i]);
                    }
                    else
                    {
                        lines[i].enabled = true;
                        dependents.Add(neighbors[i]);
                    }
                }
                break;
        }
    }
    public float CheckNeeds()
    {
        
        needsMet = new float[6];
        float[] needsGrad = new float[6];
        foreach(Plant plant in supportedBy)
        {
            int index = (int)plant.type;
            needsMet[index] += 1;
        }
        float howManyTypesNeeded = 0;
        for(int i =0; i < needs.Length; i++)//figure out percentage of individual needs
        {
            if(needs[i] != 0)
            {
                howManyTypesNeeded++;
                needsGrad[i] = needsMet[i] / needs[i];//what percentage of needs did you meet
                if(needsGrad[i] > 1) { needsGrad[i] = 1; }//cap at 100%
            }
        }
        float grad = 0;
        for (int i = 0; i < needs.Length; i++)//figure out percent of total needs
        {
            if(needs[i] != 0)
            {
                grad += needsGrad[i] * (1 / howManyTypesNeeded);
            }
        }
        if(howManyTypesNeeded == 0)
        {
            grad = 1;
        }
        return grad;
    }
    public float CheckNeedsByType(PlantType specific)
    {
        float met = 0;
        float goal = needs[(int)specific];
        foreach(Plant plant in supportedBy)
        {
            Debug.Log(plant.type);
            if(plant.type == specific && plant.used)
            {
                met += 1;
                if(met >= goal)
                {
                    break;
                }
            }
        }
        Debug.Log(met+"/"+goal);
        return met/goal;
    }
    public void Propagate()
    {//have a baby
        Vector3 newSpot = new Vector3(Random.Range(0.5f, 1.5f), 0, Random.Range(0.5f, 1.5f));
        if(Random.value < 0.5)
        {
            newSpot.x *= -1;
        }
        if(Random.value < 0.5)
        {
            newSpot.z *= -1;
        }
        newSpot += transform.position;
        newSpot += Vector3.up * 5;//put it up!
        RaycastHit hit;
        Physics.Raycast(newSpot, Vector3.up * -1, out hit);
        newSpot.y -= hit.distance;
        GameObject newPlant = Instantiate(Resources.Load("Hole",typeof(GameObject)),transform.parent)as GameObject;
        Plant newishPlant = newPlant.GetComponent<Plant>();
        newPlant.transform.GetComponent<MeshRenderer>().enabled = false;
        newPlant.transform.GetComponent<BoxCollider>().enabled = false;
        newishPlant.SetType(type);
        newPlant.transform.position = newSpot;
        //newPlant.transform.GetComponent<MeshRenderer>().enabled = false;
        //newPlant.GetComponent<Plant>().SetType(type);
        newPlant.transform.name = type.ToString();
        PlantNeighborManager.instance.newPlants.Add(newishPlant);
    }
}
