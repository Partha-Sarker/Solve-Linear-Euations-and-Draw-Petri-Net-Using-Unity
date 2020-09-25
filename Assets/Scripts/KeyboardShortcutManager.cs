using UnityEngine;

public class KeyboardShortcutManager : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject graphModes;
    public SceneController sceneController;
    public UIManager uiManager;

    public CameraController camController;
    public bool enableShortcut = true;
    private bool ctrlDown = false, shiftDown = false, altDown;
    private Vector2 moveInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!enableShortcut)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            sceneController.Back();

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            altDown = true;
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
            altDown = false;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            ctrlDown = true;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            ctrlDown = false;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            shiftDown = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            shiftDown = false;


        if (Input.GetKeyDown(KeyCode.D) && ctrlDown && !shiftDown)
        {
            Debug.Log("Delete mode activated");
            graphManager.currentMode = graphModes.GetComponent<DeleteMode>();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !ctrlDown && !shiftDown)
        {
            Debug.Log("Add edge mode activated");
            AddEdgeMode addEdgeMode = graphModes.GetComponent<AddEdgeMode>();
            addEdgeMode.prevNode = null;
            graphManager.currentMode = addEdgeMode;
        }
        else if (Input.GetKeyDown(KeyCode.E) && ctrlDown && !shiftDown)
        {
            Debug.Log("Edit mode activated");
            graphManager.currentMode = graphModes.GetComponent<InputMode>();
        }
        else if (Input.GetKeyDown(KeyCode.F) && ctrlDown && !shiftDown)
        {
            Debug.Log("Firing mode activate");
            graphManager.currentMode = graphModes.GetComponent<FiringMode>();
        }
        else if (Input.GetKeyDown(KeyCode.R) && ctrlDown && !shiftDown)
        {
            ResetGraphStates();
        }
        else if (Input.GetKeyDown(KeyCode.R) && ctrlDown && shiftDown)
        {
            ClearAll();
        }
        else if (Input.GetKeyDown(KeyCode.S) && !ctrlDown && !shiftDown)
        {
            ActivateAddStateMode();
        }
        else if (Input.GetKeyDown(KeyCode.T) && !ctrlDown && !shiftDown)
        {
            Debug.Log("Add transition mode activated");
            graphManager.currentMode = graphModes.GetComponent<AddTransitionMode>();
        }
        else if(Input.GetKeyDown(KeyCode.X) && ctrlDown && shiftDown)
        {
            Debug.Log("Exiting application");
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && ctrlDown && shiftDown)
        {
            SimulateGraph();
        }
        else
        {
            for(int i=0; i<10; i++)
            {
                if (Input.GetKeyDown("" + i) && altDown && !shiftDown)
                {
                    graphManager.SaveGraph(i);
                }
                else if(Input.GetKeyDown("" + i) && altDown && shiftDown)
                {
                    graphManager.LoadGraph(i);
                }
            }
        }



        GetMovementInput();

        camController.MoveCamera(moveInput);
    }

    public void ActivateAddStateMode()
    {
        string logText = "Add state mode activated";
        Debug.Log(logText);
        uiManager.AddLog(logText);
        graphManager.currentMode = graphModes.GetComponent<AddStateMode>();
    }

    public void ResetGraphStates()
    {
        Debug.Log("Resetting states");
        graphManager.ResetStates();
    }

    public void SimulateGraph()
    {
        bool isPlaying = graphManager.Simulate();
        if (isPlaying)
            uiManager.SetStopUI();
        else
            uiManager.SetPlayUI();
    }

    public void ClearAll()
    {
        string logText = "Clearing petri net";
        Debug.Log(logText);
        uiManager.AddLog(logText);
        graphManager.ClearAll();

    }

    private void GetMovementInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
            moveInput.y = 0;
        else if (Input.GetKey(KeyCode.UpArrow))
            moveInput.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            moveInput.y = -1;
        else
            moveInput.y = 0;

        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            moveInput.x = 0;
        else if (Input.GetKey(KeyCode.RightArrow))
            moveInput.x = 1;
        else if (Input.GetKey(KeyCode.LeftArrow))
            moveInput.x = -1;
        else
            moveInput.x = 0;
    }
}
