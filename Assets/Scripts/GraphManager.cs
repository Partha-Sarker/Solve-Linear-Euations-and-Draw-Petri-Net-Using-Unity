using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public GameObject graphModes;
    public IGraphMode currentMode;
    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();
    public List<int> initialStates;
    public int stateCount = 0, transitionCount = 0;

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


    public void FirePetriNetTransition(Transition transition)
    {
        Debug.Log("Firing " + transition.transform.name);

        int[] m = new int[stateCount];
        int[] star_t = new int[stateCount];
        int[] t_star = new int[stateCount];

        GetValuesForSimulation(transition, ref m, ref star_t, ref t_star);
        if (initialStates.Count == 0)
            initialStates = new List<int>(m);

        string mString = string.Join(", ", m);
        string star_tString = string.Join(", ", star_t);
        string t_starString = string.Join(", ", t_star);

        Debug.Log($"m: {mString}, *t: {star_tString}, t*: {t_starString}");

        if (TestStateMovingCondition(star_t, m) == false)
        {
            Debug.LogError("Can't fire " + transition.transform.name);
            return;
        }

        int[] m_prime = GetNewStates(m, star_t, t_star);
        string m_primeString = string.Join(", ", m_prime);
        Debug.Log($"New state: {m_primeString}");

        SetNewStates(m_prime);
    }

    private void GetValuesForSimulation(Transition transition, ref int[] m, ref int[] star_t, ref int[] t_star)
    {
        int count = 0;

        foreach (Node node in nodes)
        {
            string tag = node.tag;
            if (tag == "State")
            {
                State state = (State)node;
                m[count++] = state.tokenCount;
            }
        }

        foreach (Edge edge in edges)
        {
            if (edge.toNode == transition)
            {
                Node fromNode = edge.fromNode;
                star_t[fromNode.position - 1] = edge.weight;
            }
            else if (edge.fromNode == transition)
            {
                Node toNode = edge.toNode;
                t_star[toNode.position - 1] = edge.weight;
            }
        }
    }

    private bool TestStateMovingCondition(int[] star_t, int[] m)
    {
        for(int i=0; i<stateCount; i++)
        {
            if (star_t[i] > m[i])
                return false;
        }
        return true;
    }

    private int[] GetNewStates(int[] m, int[] star_t, int[] t_star)
    {
        int[] m_prime = new int[stateCount];
        for(int i=0; i<stateCount; i++)
        {
            m_prime[i] = m[i] - star_t[i] + t_star[i];
        }
        return m_prime;
    }

    private void SetNewStates(int[] m_prime)
    {
        foreach(Node node in nodes)
        {
            if (node.CompareTag("State"))
            {
                State state = (State)node;
                state.tokenCount = m_prime[state.position - 1];
            }
        }
        Refresh();
    }


    public void ResetStates()
    {
        if(initialStates == null)
        {
            Debug.Log("Nothing to reset");
        }
        if(stateCount == initialStates.Count)
        {
            SetNewStates(initialStates.ToArray());
            Debug.Log("States has been reset");
        }
        else
        {
            Debug.LogError("State number mismatch");
        }
    }

    public void Refresh()
    {
        stateCount = 0; transitionCount = 0;
        foreach(Node node in nodes)
        {
            GameObject nodeGameObject = node.gameObject;
            string tag = nodeGameObject.tag;
            string title;
            if (tag == "State")
            {
                stateCount++;
                title = "S" + stateCount;
                State state = (State)node;
                state.SetTokenText();
                node.position = stateCount;
            }
            else
            {
                transitionCount++;
                title = "T" + transitionCount;
                node.position = transitionCount;
            }
            nodeGameObject.name = title;
            node.SetTitleText(title);
        }
        for(int i = 0; i < edges.Count; i++)
        {
            int count = i + 1;
            string title = "Edge" + count;
            edges[i].gameObject.name = title;
            edges[i].SetWeightText();
        }
    }

    public void ClearAll()
    {
        foreach(Node node in nodes)
        {
            Destroy(node.gameObject);
        }
        nodes.Clear();
        foreach(Edge edge in edges)
        {
            Destroy(edge.gameObject);
        }
        edges.Clear();
        Debug.Log("Petri net cleared");
        Refresh();
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
