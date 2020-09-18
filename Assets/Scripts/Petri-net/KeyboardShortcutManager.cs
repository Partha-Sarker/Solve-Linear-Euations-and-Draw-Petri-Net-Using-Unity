using UnityEngine;

public class KeyboardShortcutManager : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject graphModes;

    public CameraController camController;
    public bool EnableShortcut { set; get; } = true;
    private bool ctrlDown = false, shiftDown = false;
    private Vector2 arrowInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!EnableShortcut)
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            ctrlDown = true;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            ctrlDown = false;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            shiftDown = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            shiftDown = false;


        if (Input.GetKeyDown(KeyCode.S) && !ctrlDown)
        {
            Debug.Log("Add state mode activated");
            graphManager.currentMode = graphModes.GetComponent<AddStateMode>();
        }
        else if (Input.GetKeyDown(KeyCode.T) && !ctrlDown)
        {
            Debug.Log("Add transition mode activated");
            graphManager.currentMode = graphModes.GetComponent<AddTransitionMode>();
        }
        else if (Input.GetKeyDown(KeyCode.D) && ctrlDown)
        {
            Debug.Log("Delete mode activated");
            graphManager.currentMode = graphModes.GetComponent<DeleteMode>();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !ctrlDown)
        {
            Debug.Log("Add edge mode activated");
            AddEdgeMode addEdgeMode = graphModes.GetComponent<AddEdgeMode>();
            addEdgeMode.prevNode = null;
            graphManager.currentMode = addEdgeMode;
        }
        else if (Input.GetKeyDown(KeyCode.E) && ctrlDown)
        {
            Debug.Log("Edit mode activated");
            graphManager.currentMode = graphModes.GetComponent<InputMode>();
        }
        else if(Input.GetKeyDown(KeyCode.F) && ctrlDown)
        {
            Debug.Log("Firing mode activate");
            graphManager.currentMode = graphModes.GetComponent<FiringMode>();
        }
        else if(Input.GetKeyDown(KeyCode.R) && ctrlDown && !shiftDown)
        {
            Debug.Log("Resetting states");
            graphManager.ResetStates();
        }
        else if(Input.GetKeyDown(KeyCode.R) && ctrlDown && shiftDown)
        {
            Debug.Log("Clearing petri net");
            graphManager.ClearAll();
        }
        else if(Input.GetKeyDown(KeyCode.X) && ctrlDown && shiftDown)
        {
            Debug.Log("Exiting application");
            Application.Quit();
        }

        GetMovementInput();

        camController.MoveCamera(arrowInput);
    }

    private void GetMovementInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
            arrowInput.y = 0;
        else if (Input.GetKey(KeyCode.UpArrow))
            arrowInput.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            arrowInput.y = -1;
        else
            arrowInput.y = 0;

        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            arrowInput.x = 0;
        else if (Input.GetKey(KeyCode.RightArrow))
            arrowInput.x = 1;
        else if (Input.GetKey(KeyCode.LeftArrow))
            arrowInput.x = -1;
        else
            arrowInput.x = 0;
    }
}
