using UnityEngine;

public class AddEdgeMode : MonoBehaviour, IGraphMode
{
    private Node prevNode = null;
    public GameObject edge;

    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
        string tag = hit.transform.tag;

        if (hit.collider != null)
        {
            Debug.Log("Got " + tag);
            Node currentNode;
            if (tag == "State")
            {
                currentNode = hit.transform.GetComponent<State>();
                if (currentNode == null)
                    Debug.LogError("Can't get state node");
            }
            else if (tag == "Transition")
            {
                currentNode = hit.transform.GetComponent<Transition>();
                if (currentNode == null)
                    Debug.LogError("can't get transition node");
            }
            else
                currentNode = null;

            if (prevNode == null && currentNode != null)
                prevNode = currentNode;
            else if (prevNode != null)
                CreateEdge(prevNode, currentNode);
            prevNode = currentNode;
        }
    }

    private void CreateEdge(Node prevNode, Node currentNode)
    {
        Debug.Log("Creating edge...");
        Vector2 startPos = prevNode.transform.position;
        Vector2 endPos = currentNode.transform.position;
        Vector2 midPos = (startPos + endPos) / 2;
        LineRenderer line = Instantiate(edge, midPos, Quaternion.identity).GetComponent<LineRenderer>();
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }
}
