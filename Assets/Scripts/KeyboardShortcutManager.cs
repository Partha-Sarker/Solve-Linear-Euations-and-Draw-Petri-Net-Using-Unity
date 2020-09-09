using UnityEngine;

public class KeyboardShortcutManager : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject graphModes;

    //public GameObject demoState;

    // Start is called before the first frame update
    void Start()
    {
        //Node node;
        //State state;
        //node = demoState.GetComponent<State>();
        //state = demoState.GetComponent<State>();
        //if (node == null)
        //    Debug.LogError("node not working");
        //if (state == null)
        //    Debug.LogError("state not working");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            graphManager.currentMode = graphModes.GetComponent<AddStateMode>();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            graphManager.currentMode = graphModes.GetComponent<DeleteMode>();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            graphManager.currentMode = graphModes.GetComponent<AddEdgeMode>();
        }
    }
}
