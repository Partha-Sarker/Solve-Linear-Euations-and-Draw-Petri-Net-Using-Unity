using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GraphManager : MonoBehaviour
{
    public GameObject graphModes;
    public UIManager uiManager;
    public IGraphMode currentMode;
    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();
    public List<int> initialStates;
    public int stateCount = 0, transitionCount = 0;
    public bool isSimulating = false;
    public bool enableGraph = true;

    private Object _lock = new Object();

    // Start is called before the first frame update
    void Start()
    {
        currentMode = graphModes.GetComponent<AddStateMode>();
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enableGraph)
            return;

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


    public bool Simulate()
    {
        if (isSimulating)
        {
            StopAllCoroutines();
            isSimulating = false;
            return false;
        }

        foreach(Node node in nodes)
        {
            if(node.CompareTag("State") && node.position == 1)
            {
                if (node.value <= 0)
                {
                    Debug.LogError("S1 has no token. Simulation can't be started.");
                    return false;
                }
                isSimulating = true;
                string logText = "Simulation Started";
                Debug.Log(logText);
                uiManager.AddLog(logText);
                StartCoroutine(StartStochasticSimulation(node));
            }
        }
        return true;
    }

    IEnumerator StartStochasticSimulation(Node state)
    {
        if (state.value <= 0)
            yield break;

        Debug.Log($"Activating {state.transform.name}:");
        yield return new WaitForSeconds(1f);

        List<Node> connectedTransitions = GetOutgoingNodes(state);
        Debug.Log($"{state.transform.name} is connected with:");
        foreach(Node connectedTransition in connectedTransitions)
        {
            Debug.Log(connectedTransition.transform.name);
        }

        Node selectedTransition = SelectTransitionByProbability(connectedTransitions);
        if (selectedTransition == null)
            yield break;

        float lambda = (float)GetSumOfProbability(connectedTransitions);
        float randomNum = Random.Range(0f, 1f);
        float waitingTime = -(1 / lambda) * Mathf.Log(1 - randomNum);

        uiManager.AddLog($"Waiting time for {selectedTransition.transform.name} is {waitingTime}");

        if (CheckStochasticTransitionFireCondition(selectedTransition))
        {
            FirePetriNetTransition((Transition)selectedTransition);
            List<Node> connectedStates = GetOutgoingNodes(selectedTransition);
            foreach (Node connectedState in connectedStates)
                StartCoroutine(StartStochasticSimulation(connectedState));
        }
    }

    private Node SelectTransitionByProbability(List<Node> transitions)
    {
        int sumOfProb = GetSumOfProbability(transitions);
        int randomNumebr = Random.Range(1, sumOfProb + 1);
        int tempSum = 0;
        foreach(Node node in transitions)
        {
            if (randomNumebr >= tempSum && randomNumebr <= node.value + tempSum)
                return node;
            tempSum += node.value;
        }
        return null;
    }

    private int GetSumOfProbability(List<Node> transitions)
    {
        int sumOfProb = 0;
        foreach (Node node in transitions)
        {
            sumOfProb += node.value;
        }
        return sumOfProb;
    }

    private bool CheckStochasticTransitionFireCondition(Node transition)
    {
        List<Node> states = GetIncomingNodes(transition);
        foreach(Node state in states)
        {
            if (state.value <= 0)
                return false;
        }
        return true;
    }


    public void FirePetriNetTransition(Transition transition)
    {
        lock (_lock)
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

            //Debug.Log($"m: {mString}, *t: {star_tString}, t*: {t_starString}");

            if (TestStateMovingCondition(star_t, m) == false)
            {
                //Debug.LogError("Can't fire " + transition.transform.name);
                return;
            }

            //SetTransitionFireColor(transition);
            transition.SetFiringColor();

            int[] m_prime = GetNewStates(m, star_t, t_star);
            string m_primeString = string.Join(", ", m_prime);
            //Debug.Log($"New state: {m_primeString}");

            SetNewStates(m_prime);
        }
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
                m[count++] = state.value;
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
                state.value = m_prime[state.position - 1];
            }
        }
        Refresh();
    }


    public void ResetStates()
    {
        //SetTransitionFireColor(null);
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

    private void Refresh()
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
                node.position = stateCount;
            }
            else
            {
                transitionCount++;
                title = "T" + transitionCount;
                node.position = transitionCount;
            }
            node.SetValueText();
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

    private Edge GetEdge(Node toNode, Node fromNode)
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

    private List<Node> GetOutgoingNodes(Node node)
    {
        List<Node> nodes = new List<Node>();
        foreach(Edge edge in edges)
        {
            if(edge.fromNode == node)
            {
                nodes.Add(edge.toNode);
            }
        }

        return nodes;
    }

    private List<Node> GetIncomingNodes(Node node)
    {
        List<Node> nodes = new List<Node>();
        foreach (Edge edge in edges)
        {
            if (edge.toNode == node)
            {
                nodes.Add(edge.fromNode);
            }
        }

        return nodes;
    }

    public void SaveGraph(int index)
    {
        if(nodes.Count == 0)
        {
            string log = "Nothing to save";
            uiManager.AddLog(log);
            Debug.LogError(log);
            return;
        }
        Refresh();
        PetriNet petriNet = new PetriNet(nodes, edges);
        string jsonString = JsonUtility.ToJson(petriNet);
        Debug.Log(jsonString);
        string path = Application.persistentDataPath + $"graph_{index}.json";
        File.WriteAllText(path, jsonString);
        //LoadGraph(0);
        string logText = "Petri net saved on slot " + index;
        uiManager.AddLog(logText);
        Debug.Log(logText);
    }

    public void LoadGraph(int index)
    {
        string path = Application.persistentDataPath + $"graph_{index}.json";
        string jsonString = "";
        try
        {
            jsonString = File.ReadAllText(path);
        }
        catch (System.Exception) { }

        if(jsonString == "")
        {
            string log = $"slot {index} is empty";
            uiManager.AddLog(log);
            Debug.LogError(log);
            return;
        }

        if (isSimulating)
            Simulate();

        ClearAll();
        PetriNet petriNet = JsonUtility.FromJson<PetriNet>(jsonString);

        AddStateMode addStateMode = FindObjectOfType<AddStateMode>();
        AddTransitionMode addTransitionMode = FindObjectOfType<AddTransitionMode>();
        AddEdgeMode addEdgeMode = FindObjectOfType<AddEdgeMode>();

        foreach (NodeInfo nodeInfo in petriNet.nodeInfos)
        {
            int value = nodeInfo.value;
            Vector3 pos = nodeInfo.WorldPos;
            if (nodeInfo.tag == "State")
                addStateMode.AddState(pos).value = value;
            else
                addTransitionMode.AddTransition(pos).value = value;
        }
        foreach(EdgeInfo edgeInfo in petriNet.edgeInfos)
        {
            Node toNode = FindNode(edgeInfo.toNodeTag, edgeInfo.toNodePosition);
            Node fromNode = FindNode(edgeInfo.fromNodeTag, edgeInfo.fromNodePosition);
            addEdgeMode.CreateEdge(fromNode, toNode).weight = edgeInfo.weight;
        }
        string logText = $"Petri-net loaded from slot {index}";
        uiManager.AddLog(logText);
        Debug.Log(logText);
        Refresh();
    }

    private Node FindNode(string tag, int position)
    {
        int count = 0;
        foreach(Node node in nodes)
        {
            if (node.CompareTag(tag))
            {
                count++;
                if (count == position)
                    return node;
            }
        }
        return null;
    }


    //private void SetTransitionFireColor(Transition targetTransition)
    //{
    //    foreach(Node node in nodes)
    //    {
    //        if (node.CompareTag("Transition"))
    //        {
    //            Transition transition = (Transition)node;
    //            if (transition == targetTransition)
    //                transition.SetFiringColor();
    //            else
    //                transition.ResetColor();
    //        }
    //    }
    //}
}
