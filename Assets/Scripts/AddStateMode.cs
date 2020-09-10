using UnityEngine;

public class AddStateMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;
    public GameObject state;
    private Vector2 mouseWorldPos;

    public void OnClick()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapCircle(mouseWorldPos, 2);
        if (col != null)
            return;
        Node node = Instantiate(state, mouseWorldPos, Quaternion.identity).GetComponent<State>();
        graphManager.AddNode(node);
    }
}
