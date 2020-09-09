using UnityEngine;

public class AddStateMode : MonoBehaviour, IGraphMode
{
    public GameObject state;
    private Vector2 mouseWorldPos;

    public void OnClick()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(state, mouseWorldPos, Quaternion.identity);
    }
}
