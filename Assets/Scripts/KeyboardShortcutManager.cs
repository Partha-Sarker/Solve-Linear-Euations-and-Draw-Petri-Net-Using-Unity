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
        if (Input.GetKeyUp(KeyCode.D))
        {
            graphManager.currentMode = graphModes.GetComponent<DeleteMode>();
        }

    }
}
