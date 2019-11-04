using UnityEngine;
using System.Collections;
using Pathfinding;

public class WanderingDestinationSetter : MonoBehaviour
{
    public bool ready;
    public float radius = 10;

    IAstarAI ai;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
    }

    Vector3 PickRandomPoint(float mod)
    {
        var point = Random.insideUnitSphere * (radius*mod);

        point.y = 0;
        point += ai.position;
        return point;
    }

    void Update()
    {
        ready = !ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath);
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        /*if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.destination = PickRandomPoint();
            ai.SearchPath();
        }*/
    }
    public void NewPoint(float mod)
    {
        ai.destination = PickRandomPoint(mod);
        ai.SearchPath();
    }
}
