using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTransitionMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;
    public GameObject transition;
    private Vector2 mouseWorldPos;
    public float minDistance = 2f;

    public void OnClick()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapCircle(mouseWorldPos, minDistance);
        if (col != null)
            return;
        AddTransition(mouseWorldPos);
    }

    public Node AddTransition(Vector3 position)
    {
        Node node = Instantiate(transition, position, Quaternion.identity).GetComponent<Node>();
        graphManager.AddNode(node);
        return node;
    }
}
