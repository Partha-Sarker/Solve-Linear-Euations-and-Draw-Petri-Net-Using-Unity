using UnityEngine;

public class AddEdgeMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;
    public GameObject edgeGameObject;
    public UIManager uiManager;
    [HideInInspector] public Node prevNode;

    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            string tag = hit.transform.tag;
            Node currentNode;
            if (tag == "State" || tag == "Transition")
            {
                currentNode = hit.transform.GetComponent<Node>();
            }
            else return;

            if (prevNode == null && currentNode != null)
            {
                prevNode = currentNode;
                string logText = $"{currentNode.transform.name}is waiting for next node to create edge";
                uiManager.AddLog(logText);
                Debug.Log(logText);
            }
            else if (prevNode != null && prevNode != currentNode)
            {
                if (!prevNode.CompareTag(currentNode.tag))
                {
                    string logText = $"Edge has been created between {prevNode.transform.name} {currentNode.transform.name}";
                    Debug.Log(logText);
                    uiManager.AddLog(logText);
                    CreateEdge(prevNode, currentNode);
                }
            }

            prevNode = currentNode;
        }
    }

    public Edge CreateEdge(Node fromNode, Node toNode)
    {
        GameObject newEdgeGameObject = Instantiate(edgeGameObject);
        Edge edge = newEdgeGameObject.GetComponent<Edge>();

        edge.toNode = toNode;
        edge.fromNode = fromNode;

        edge.Init();

        graphManager.AddEdge(edge);
        return edge;
    }
}
