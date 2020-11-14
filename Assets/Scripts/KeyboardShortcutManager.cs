using UnityEngine;
using SimpleFileBrowser;
using System.Collections;
using System;

public class KeyboardShortcutManager : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject graphModes;
    public SceneController sceneController;
    public UIManager uiManager;

    public CameraController camController;
    public bool enableShortcut = true;
    private bool ctrlDown = false, shiftDown = false;
    private Vector2 moveInput;

    void Update()
    {
        if (!enableShortcut)
            return;

        //if (Input.GetKeyDown(KeyCode.Escape))
        //    sceneController.GoToMainMenu();

        //if (Input.GetKeyDown(KeyCode.LeftAlt))
        //    altDown = true;
        //else if (Input.GetKeyUp(KeyCode.LeftAlt))
        //    altDown = false;

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
            string logText = "Delete mode activated";
            uiManager.AddLog(logText);
            Debug.Log(logText);
            graphManager.currentMode = graphModes.GetComponent<DeleteMode>();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !ctrlDown && !shiftDown)
        {
            string logText = "Add edge mode activated";
            Debug.Log(logText);
            uiManager.AddLog(logText);
            AddEdgeMode addEdgeMode = graphModes.GetComponent<AddEdgeMode>();
            addEdgeMode.prevNode = null;
            graphManager.currentMode = addEdgeMode;
        }
        else if (Input.GetKeyDown(KeyCode.E) && !ctrlDown && shiftDown)
        {
            string logText = "Add double edge mode activated";
            Debug.Log(logText);
            uiManager.AddLog(logText);
            AddDoubleEdgeMode addDoubleEdgeMode = graphModes.GetComponent<AddDoubleEdgeMode>();
            addDoubleEdgeMode.prevNode = null;
            graphManager.currentMode = addDoubleEdgeMode;
        }
        else if (Input.GetKeyDown(KeyCode.E) && ctrlDown && !shiftDown)
        {
            string logText = "Edit mode activated";
            Debug.Log(logText);
            uiManager.AddLog(logText);
            graphManager.currentMode = graphModes.GetComponent<InputMode>();
        }
        else if (Input.GetKeyDown(KeyCode.F) && ctrlDown && !shiftDown)
        {
            string logText = "Firing mode activate";
            Debug.Log(logText);
            uiManager.AddLog(logText);
            graphManager.currentMode = graphModes.GetComponent<FiringMode>();
        }
        else if (Input.GetKeyDown(KeyCode.L) && ctrlDown)
        {
            StartCoroutine(ShowLoadDialogCoroutine());
        }
        else if (Input.GetKeyDown(KeyCode.R) && ctrlDown && !shiftDown)
        {
            ResetGraphStates();
        }
        else if (Input.GetKeyDown(KeyCode.R) && ctrlDown && shiftDown)
        {
            graphManager.ClearAll();
        }
        else if (Input.GetKeyDown(KeyCode.S) && !ctrlDown && !shiftDown)
        {
            ActivateAddStateMode();
        }
        else if (Input.GetKeyDown(KeyCode.S) && ctrlDown)
        {
            StartCoroutine(ShowSaveDialogCoroutine());
        }
        else if (Input.GetKeyDown(KeyCode.T) && !ctrlDown && !shiftDown)
        {
            string logText = "Add transition mode activated";
            Debug.Log(logText);
            uiManager.AddLog(logText);
            graphManager.currentMode = graphModes.GetComponent<AddTransitionMode>();
        }
        else if(Input.GetKeyDown(KeyCode.X) && ctrlDown && shiftDown)
        {
            string logText = "Exiting application";
            Debug.Log(logText);
            uiManager.AddLog(logText);
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && ctrlDown && shiftDown)
        {
            SimulateGraph();
        }
        //else
        //{
        //    for(int i=0; i<10; i++)
        //    {
        //        if (Input.GetKeyDown("" + i) && altDown && shiftDown)
        //        {
        //            graphManager.SaveGraph(i);
        //        }
        //        else if(Input.GetKeyDown("" + i) && altDown && !shiftDown)
        //        {
        //            graphManager.LoadGraph(i);
        //        }
        //    }
        //}


        GetMovementInput();

        camController.MoveCamera(moveInput);
    }


    public void SavePetriNet()
    {
        StartCoroutine(ShowSaveDialogCoroutine());
    }

    public void LoadPetriNet()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        camController.canZoom = false;
        enableShortcut = false;
        graphManager.pendingStuffs++;

        FileBrowser.SetFilters(false, new FileBrowser.Filter("Graph", ".json"));
        //FileBrowser.AddQuickLink("File Browser", "C:\\", null);
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, true, "C:\\", "Load File", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];
            Debug.Log(path);
            try
            {
                graphManager.LoadGraph(path);
            }
            catch (Exception)
            {
                string log = $"Can't load graph from {path}";
                Debug.LogWarning(log);
                uiManager.AddLog(log);
            }
        }

        graphManager.pendingStuffs--;
        enableShortcut = true;
        camController.canZoom = true;
    }

    IEnumerator ShowSaveDialogCoroutine()
    {
        camController.canZoom = false;
        enableShortcut = false;
        graphManager.pendingStuffs++;
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Graph", ".json"));

        yield return FileBrowser.WaitForSaveDialog(false, true, "C:\\", "Save Graph", "Save");
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];
            Debug.Log(path);
            try
            {
                graphManager.SaveGraph(path);
            }
            catch (Exception)
            {
                string log = $"Can't save graph to {path}";
                Debug.LogWarning(log);
                uiManager.AddLog(log);
            }
        }
        graphManager.pendingStuffs--;
        enableShortcut = true;
        camController.canZoom = true;
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
        string logText = "Resetting states";
        Debug.Log(logText);
        uiManager.AddLog(logText);
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
