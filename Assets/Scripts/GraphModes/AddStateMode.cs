using UnityEngine;

public class AddStateMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;
    public GameObject state;
    private Vector2 mouseWorldPos;
    public float minDistance = 2f;

    public void OnClick()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapCircle(mouseWorldPos, minDistance);
        if (col != null)
            return;
        AddState(mouseWorldPos);
    }

    public Node AddState(Vector3 position)
    {
        Node node = Instantiate(state, position, Quaternion.identity).GetComponent<Node>();
        graphManager.AddNode(node);
        return node;
    }
}
