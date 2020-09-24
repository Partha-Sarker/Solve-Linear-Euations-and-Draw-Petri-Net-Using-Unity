using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EdgeInfo
{
    public int fromNodePosition, toNodePosition;
    public string fromNodeTag, toNodeTag;
    public int weight = 1;

    public EdgeInfo(Edge edge)
    {
        fromNodePosition = edge.fromNode.position;
        toNodePosition = edge.toNode.position;
        fromNodeTag = edge.fromNode.tag;
        toNodeTag = edge.toNode.tag;
        weight = edge.weight;
    }
}
