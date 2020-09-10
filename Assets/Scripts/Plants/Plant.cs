using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant
{
    //states (unique runtime plant data)
    public GrowthState growthState;
    public int age;
    public GameObject plantObj;
    //TODO: zone id

    public Plant(PlantType planttype)
    {
        plantType = planttype;
    }

    // ---- Scriptable object data ----
    public GameObject plantPrefab => plantType.plantPrefab;
    public PlantType plantType;

    //reproduction behavior
    public int BabiesAllowed => plantType.babiesAllowed;
    public int BabiesPerDay => plantType.babiesPerDay;
    public float BabyDistance_max => plantType.babyDistance_max;
    public float BabyDistance_min => plantType.babyDistance_min;
    
    //trait requirements
    public Elevation Req_elevation => plantType.req_elevation;
    public int Req_airQuality => plantType.req_airQuality;

    //factor requirements
    public Shade Req_shade => plantType.req_shade; //none,half,full
    public float Req_moistureMin => plantType.req_moistureMin;
    public float Req_moistureMax => plantType.req_moistureMax;
    public float Req_tempMin => plantType.req_tempMin;
    public float Req_tempMax => plantType.req_tempMax;
    public Soil Req_soilMin => plantType.req_soilMin;
    public Soil Req_soilMax => plantType.req_soilMax;
    public Nutrients Req_nutrients => plantType.req_nutrients;

    public float Req_distanceForSame => plantType.req_distanceForSame;
    public float Req_distanceForOther => plantType.req_distanceForOther;

    //factor outputs
    public Shade Out_shade => plantType.out_shade; //none,half,full
    public float Out_moisture => plantType.out_moisture;
    public Soil Out_soil => plantType.out_soil;
    public Nutrients Out_nutrients => plantType.out_nutrients;


}

[CreateAssetMenu(fileName = "Plant")]
public class PlantType : ScriptableObject
{
    public GameObject plantPrefab;

    //reproduction behavior
    public int babiesAllowed;
    public int babiesPerDay;
    [Range(0, 20)]
    public float babyDistance_max;
    [Range(0, 20)]
    public float babyDistance_min;

    //trait requirements
    public Elevation req_elevation;
    [Range(0, 10)]
    public int req_airQuality;

    //factor requirements
    public Shade req_shade; //none,half,full
    [Range(0, 1)]
    public float req_moistureMin, req_moistureMax;
    public float req_tempMin, req_tempMax;
    public Soil req_soilMin, req_soilMax;
    public Nutrients req_nutrients;

    [Range(0, 20)]
    public float req_distanceForSame;
    [Range(0, 20)]
    public float req_distanceForOther;

    //factor outputs
    public Shade out_shade; //none,half,full
    [Range(-1, 1)]
    public float out_moisture;

    [SerializeField]
    public Soil out_soil;
    public Nutrients out_nutrients;
}

public enum GrowthState
{
    baby, adult, sick, dead
}
public enum Elevation
{
    low,mid,high
}
public enum Shade
{
    none,half,full
}

[System.Serializable]
public class Soil
{
    [Range(-10, 10)]
    public int clay;
    [Range(-10, 10)]
    public int sand;
    [Range(-10, 10)]
    public int silt;
    [Range(-1, 1)]
    public float horizon;

    public Soil(int clay, int sand, int silt, float horizon) //TODO: Make sure this rounds properly?
    {
        int sum = clay + sand + silt;
        if (sum > 10)
        {
            clay = 10 * (clay / sum);
            sand = 10 * (sand / sum);
            silt = 10 * (silt / sum);
        }
        this.clay = clay;
        this.sand = sand;
        this.silt = silt;
        this.horizon = horizon;
    }
}

[System.Serializable]
public class Nutrients
{
    [Range(-10, 10)]
    public int nitrogen;
    [Range(-10, 10)]
    public int phosphorous;
    [Range(-10, 10)]
    public int potassium;


    public Nutrients(int nitrogen, int phosphorous, int potassium)
    {
        this.nitrogen = nitrogen;
        this.phosphorous = phosphorous;
        this.potassium = potassium;
    }
}
