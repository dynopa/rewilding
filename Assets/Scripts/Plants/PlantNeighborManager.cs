using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantNeighborManager : MonoBehaviour
{
    public static PlantNeighborManager instance;
    public List<Plant> plants;
    public GameObject line;
    List<LineRenderer> lines = new List<LineRenderer>();
    public float maxDistance = 5f;
    public int numLines = 0;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ClearConnections();
            CheckForConnections();
        }
    }
    // Update is called once per frame
    void CheckForConnections()
    {
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
            foreach(Plant neighbor in plant.neighbors)
            {
                GameObject objLr = Instantiate(line,plant.transform);
                LineRenderer lr = objLr.GetComponent<LineRenderer>();
                //float distance = Vector3.Distance(plant.transform.position, neighbor.transform.position);
                //Color color = //Color.Lerp(Color.green, Color.red, distance / maxDistance);
                lr.positionCount = 2;
                lr.SetPosition(0, plant.transform.position+Vector3.up*0.25f);
                lr.SetPosition(1, neighbor.transform.position + Vector3.up * 0.25f);
                lr.startColor = Color.green;
                lr.endColor = Color.blue;
                plant.lines.Add(lr);
                lines.Add(lr);
            }
            plant.UpdateDependecies();
        }
    }
    void ClearConnections()
    {
        foreach(LineRenderer lr in lines)
        {
            Destroy(lr.gameObject);
        }
        foreach(Plant plant in plants)
        {
            plant.lines.Clear();
            plant.neighbors.Clear();
        }
        lines.Clear();
    }
}
