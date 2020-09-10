using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public GameObject graphModes;
    public IGraphMode currentMode;
    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();

    // Start is called before the first frame update
    void Start()
    {
        currentMode = graphModes.GetComponent<AddStateMode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            currentMode.OnClick();
        }
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
        Refresh();
    }

    public void RemoveNode(Node node)
    {
        List<Edge> edgeRemoveList = new List<Edge>();
        foreach(Edge edge in edges) 
        {
            if ((edge.toNode == node || edge.fromNode == node) && !edgeRemoveList.Contains(edge))
                edgeRemoveList.Add(edge);
        }
        foreach (Edge edge in edgeRemoveList)
            RemoveEdge(edge);

        nodes.Remove(node);
        node.DestroySelf();
        Refresh();
    }

    public void AddEdge(Edge edge)
    {
        // check if edge exists between this nodes
        Edge reverseEdge = GetEdge(edge.fromNode, edge.toNode);
        if (reverseEdge != null)
            RemoveEdge(reverseEdge);
        edges.Add(edge);
        Refresh();
    }

    public void RemoveEdge(Edge edge)
    {
        edges.Remove(edge);
        edge.DestroySelf();
        Refresh();
    }

    public void Refresh()
    {
        int stateCount = 0, transitionCount = 0;
        foreach(Node node in nodes)
        {
            GameObject nodeGameObject = node.gameObject;
            string tag = nodeGameObject.tag;
            string title;
            if (tag == "State")
            {
                stateCount++;
                title = "S" + stateCount;
            }
            else
            {
                transitionCount++;
                title = "T" + transitionCount;
            }
            nodeGameObject.name = title;
            node.SetTitleText(title);
        }
        for(int i = 0; i < edges.Count; i++)
        {
            string title = "Edge" + i;
            edges[i].gameObject.name = title;
        }
    }

    public Edge GetEdge(Node toNode, Node fromNode)
    {
        foreach(Edge edge in edges)
        {
            if (edge.toNode == toNode && edge.fromNode == fromNode)
                return edge;
            else if (edge.fromNode == toNode && edge.toNode == fromNode)
                return edge;
        }
        return null;
    }
}
