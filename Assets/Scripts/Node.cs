using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //public List<Edge> incomingEdges, outgoingEdges;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
