using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeInfo
{
    public int value;
    public Vector3 WorldPos;
    public string tag;

    public NodeInfo(Node node)
    {
        this.value = node.value;
        this.WorldPos = node.transform.position;
        tag = node.transform.tag;
    }

    //public void LoadNode()
    //{
    //    Debug.Log($"Loading node {position}");
        
    //}
}
