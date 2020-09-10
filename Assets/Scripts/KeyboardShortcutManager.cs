using UnityEngine;

public class KeyboardShortcutManager : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject graphModes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            graphManager.currentMode = graphModes.GetComponent<AddStateMode>();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            graphManager.currentMode = graphModes.GetComponent<AddTransitionMode>();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            graphManager.currentMode = graphModes.GetComponent<DeleteMode>();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            AddEdgeMode addEdgeMode = graphModes.GetComponent<AddEdgeMode>();
            addEdgeMode.prevNode = null;
            graphManager.currentMode = addEdgeMode;
        }
    }
}
