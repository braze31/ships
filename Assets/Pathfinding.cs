using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Pathfinding : MonoBehaviour
{

    public Transform[] points;
    private NavMeshAgent nav;
    private int destPoint;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixUpdate()
    {
        if (!nav.pathPending && nav.remainingDistance < 0.5f)
            GoToNextPoint();
    }
    void GoToNextPoint()
    {
        if (points.Length == 0)
        {
            return;
        }
        nav.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }
}
