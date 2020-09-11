using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;

    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            string tag = hit.transform.tag;
            if (tag == "State" || tag == "Transition")
                graphManager.RemoveNode(hit.transform.GetComponent<Node>());
            else if (tag == "Edge")
                graphManager.RemoveEdge(hit.transform.GetComponent<Edge>());

        }
    }
}
