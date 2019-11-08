﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlantType
{
    None,Spread,Grass,Shrub,Tree,Special
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
    public float[] needs = new float[] { 0, 0, 0, 0, 0, 0 };
    public float supportNum = 1;//how many can this single thing support?
    public bool met = false;
    MeshRenderer mr;
    public Material good;
    public Material bad;

    GameObject child;
    float size = 1;
    int age;
    float growthStage;
    public bool used; //if this is true, its used up all its resources to support others
    float tempSupported; //used to check how much they're supporting in a day
    public bool[] filled = new bool[] { false, false, false, false, false, false };//true means its needs are met!

    // Start is called before the first frame update
    void Awake()
    {
        filled = new bool[] { false, false, false, false, false, false };
    }

    // Update is called once per frame
    void Update()
    {
        met = CheckNeeds();
        if (mr)
        {
            mr.material = met ? good : bad;
            child.transform.localScale = met ? new Vector3(1, 1, 1)*size : new Vector3(0.5f, 0.5f, 0.5f)*size;
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
        type = newType;
        if (type != PlantType.None && !child)
        {
            string modelName = prefabs[(int)type];
            child = Instantiate(Resources.Load(modelName), transform) as GameObject;
            size = child.transform.localScale.x;
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
    }
    public void NewDay()
    {
        tempSupported = 0;
        used = false;
        for(int i = 0; i < filled.Length; i++)
        {
            filled[i] = CheckNeedsByType((PlantType)i);
        }
        supportedBy.Clear();
        dependents.Clear();
    }
    public void UpdateDependecies()
    {
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
                    if(neighbors[i].type == PlantType.Spread || neighbors[i].type == PlantType.None)
                    {
                        lines[i].enabled = false;
                    }
                    else
                    {
                        if (!used && !neighbors[i].filled[(int)type])
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
                            if (neighbors[i].CheckNeedsByType(type))//the neighbor has been supported
                            {
                                neighbors[i].filled[(int)type] = true;
                                lines[i].endColor = Color.green;
                            }
                        }
                        else
                        {
                            lines[i].enabled = false;
                        }
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
                    else
                    {
                        if (!used && !neighbors[i].filled[(int)type])
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
                            if (neighbors[i].CheckNeedsByType(type))//the neighbor has been supported
                            {
                                neighbors[i].filled[(int)type] = true;
                                lines[i].endColor = Color.green;
                            }
                        }
                        else
                        {
                            lines[i].enabled = false;
                        }
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
                    else
                    {
                        if (!used && !neighbors[i].filled[(int)type])
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
                            if (neighbors[i].CheckNeedsByType(type))//the neighbor has been supported
                            {
                                neighbors[i].filled[(int)type] = true;
                                lines[i].endColor = Color.green;
                            }
                        }
                        else
                        {
                            lines[i].enabled = false;
                        }
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
    public bool CheckNeeds()
    {
        bool met = false;
        float[] needsMet = new float[6];
        foreach(Plant plant in supportedBy)
        {
            int index = (int)plant.type;
            needsMet[index] += 1;
        }
        for(int i =0; i < needs.Length; i++)
        {
            if (needsMet[i] >= needs[i])
            {
                met = true;
                continue;
            }
            else
            {
                met = false;
                break;
            }
        }
        return met;
    }
    public bool CheckNeedsByType(PlantType specific)
    {
        bool met = false;
        float goal = needs[(int)specific];
        Debug.Log(goal);
        foreach(Plant plant in supportedBy)
        {
            Debug.Log(plant.type);
            if(plant.type == specific && plant.used)
            {
                Debug.Log("A");
                goal -= 1;
                if(goal <= 0)
                {
                    met = true;
                    break;
                }
            }
        }
        Debug.Log(goal);
        return met;
    }
}
