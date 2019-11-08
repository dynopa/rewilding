using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantNeighborManager : MonoBehaviour
{
    public static PlantNeighborManager instance;
    public List<Plant> plants;
    public GameObject line;
   // public Dictionary<Plant, List<LineRenderer>> lines = new Dictionary<Plant, List<LineRenderer>>();//points to origin of line
    public float maxDistance = 5f;
    public int numLines = 0;
    public bool showLines;
    bool toggle;
    Vector3 tempPlant;
    float lastEPress;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    private void Update()
    {
        showLines = Input.GetKey(KeyCode.E) || toggle;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(Time.time < lastEPress+0.3)
            {
                toggle = !toggle;
            }
            else
            {
                lastEPress = Time.time;
            }
        }
    }
    // Update is called once per frame
    public void CheckForConnections()
    {
        ClearConnections();
        plants.Sort(SortByType);
        foreach(Plant plant in plants)
        {
            foreach(Plant plant2 in plants)
            {
                if(plant == plant2) { continue; }
                if (!plant.neighbors.Contains(plant2))
                {
                    if (maxDistance >Vector3.Distance(plant.transform.position, plant2.transform.position))
                    {
                        plant.neighbors.Add(plant2);
                        plant2.neighbors.Add(plant);
                    }
                }
            }
            //now set up lines
            tempPlant = plant.transform.position;
            plant.neighbors.Sort(SortByDistance);
            //plant.neighbors.Reverse();
            foreach (Plant neighbor in plant.neighbors)
            {
                GameObject objLr = Instantiate(line,plant.transform);
                LineRenderer lr = objLr.GetComponent<LineRenderer>();
                //float distance = Vector3.Distance(plant.transform.position, neighbor.transform.position);
                //Color color = //Color.Lerp(Color.green, Color.red, distance / maxDistance);
                lr.positionCount = 2;
                lr.SetPosition(0, plant.transform.position+Vector3.up*0.25f);
                lr.SetPosition(1, neighbor.transform.position + Vector3.up * 0.25f);
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                plant.lines.Add(lr);
            }
            plant.UpdateDependecies();
        }
        foreach(Plant plant in plants)
        {
            for (int i = 0; i < plant.neighbors.Count; i++)
            {
                Debug.Log((int)plant.type);
                if (plant.neighbors[i].filled[(int)plant.type])
                {
                    plant.lines[i].endColor = Color.green;
                }
                if (plant.neighbors[i].used)
                {
                    plant.lines[i].startColor = Color.green;
                }
            }
        }
        tempPlant = Vector3.zero;
    }
    int SortByDistance(Plant p1, Plant p2)
    {
        float d1 = Vector3.Distance(p1.transform.position, tempPlant);
        float d2 = Vector3.Distance(p2.transform.position, tempPlant);
        return d1.CompareTo(d2);

    }
    int SortByType(Plant p1, Plant p2)
    {
        float t1 = (int)p1.type;
        float t2 = (int)p2.type;
        return t1.CompareTo(t2);
    }
    void ClearConnections()
    {
        foreach(Plant plant in plants)
        {
            foreach(LineRenderer lr in plant.lines)
            {
                Destroy(lr.gameObject);
            }
            plant.lines.Clear();
            plant.neighbors.Clear();
            plant.NewDay();
        }
    }
}
