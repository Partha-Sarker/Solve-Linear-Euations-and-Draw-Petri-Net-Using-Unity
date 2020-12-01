using System.Collections.Generic;
using UnityEngine;

public class AddEdgeMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;
    public GameObject edgeGameObject;
    public UIManager uiManager;

    [SerializeField] private Node prevNode;
    private Vector3 mouseWorldPos;

    private List<Vector3> positions = new List<Vector3>();
    private Edge currentEdge;
    private LineRenderer currentLine;

    public void OnClick()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            string tag = hit.transform.tag;
            if (prevNode != null && prevNode.CompareTag(tag))
                return;

            print($"new tag: {tag}");
            Node currentNode;
            if (tag == "State" || tag == "Transition")
                currentNode = hit.transform.GetComponent<Node>();
            else 
                return;

            
            string logText;
            if (prevNode == null)
            {
                prevNode = currentNode;
                currentEdge = Instantiate(edgeGameObject, currentNode.transform.position, Quaternion.identity).GetComponent<Edge>();
                Destroy(currentEdge.GetComponent<BoxCollider2D>());
                currentEdge.transform.name = "temp edge";
                currentLine = currentEdge.GetComponent<LineRenderer>();
                mouseWorldPos = currentNode.transform.position;
                AddPointToCurrentEdge(true);
            }
            else
            {
                mouseWorldPos = currentNode.transform.position;
                AddPointToCurrentEdge(true);
                logText = $"Edge has been created from {prevNode.transform.name} to {currentNode.transform.name}";
                Debug.Log(logText);
                uiManager.AddLog(logText);

                CreateEdge(prevNode, currentNode, positions.ToArray());

                currentLine.positionCount = 0;
                positions = new List<Vector3>();
                AddPointToCurrentEdge(true);
            }

            logText = $"{currentNode.transform.name} is waiting for next node to create edge";
            uiManager.AddLog(logText);
            Debug.Log(logText);
            prevNode = currentNode;
        }
        else if(prevNode != null)
        {
            AddPointToCurrentEdge();
        }
    }

    public void AddPointToCurrentEdge(bool force = false)
    {
        int currentSize = currentLine.positionCount;

        if (Input.GetKey(KeyCode.LeftShift) && currentSize > 0 && !force)
        {
            Vector3 prevPoint = currentLine.GetPosition(currentSize - 1);
            float xOffset = Mathf.Abs(prevPoint.x - mouseWorldPos.x);
            float yOffset = Mathf.Abs(prevPoint.y - mouseWorldPos.y);
            if (xOffset <= yOffset)
                mouseWorldPos.x = prevPoint.x;
            else
                mouseWorldPos.y = prevPoint.y;
        }

        if (Input.GetKey(KeyCode.LeftControl) && currentSize > 0)
        {
            currentLine.SetPosition(currentSize - 1, mouseWorldPos);
            positions.RemoveAt(currentSize - 1);
            positions.Add(mouseWorldPos);
        }
        else
        {
            currentLine.positionCount = currentSize + 1;
            currentLine.SetPosition(currentSize, mouseWorldPos);
            positions.Add(mouseWorldPos);
        }
    }

    public void ResetEverything()
    {
        prevNode = null;
        currentLine = null;
        if (currentEdge != null)
            Destroy(currentEdge.gameObject);
        currentEdge = null;
        positions = new List<Vector3>();
    }

    public Edge CreateEdge(Node fromNode, Node toNode, Vector3[] positions)
    {
        Edge edge = Instantiate(edgeGameObject, toNode.transform.position, Quaternion.identity).GetComponent<Edge>();
        edge.fromNode = fromNode;
        edge.toNode = toNode;
        edge.linePoints = positions;
        edge.Init();
        graphManager.AddEdge(edge);

        return edge;
    }
}
