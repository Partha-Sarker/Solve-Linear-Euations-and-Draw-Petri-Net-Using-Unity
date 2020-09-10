using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTransitionMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;
    public GameObject transition;
    private Vector2 mouseWorldPos;

    public void OnClick()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapCircle(mouseWorldPos, 2);
        if (col != null)
            return;
        Node node = Instantiate(transition, mouseWorldPos, Quaternion.identity).GetComponent<Transition>();
        graphManager.AddNode(node);
    }
}
