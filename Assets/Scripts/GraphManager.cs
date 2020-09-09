using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public GameObject graphModes;
    public IGraphMode currentMode;


    // Start is called before the first frame update
    void Start()
    {
        currentMode = graphModes.GetComponent<AddStateMode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            currentMode.OnClick();
        }
    }
}
