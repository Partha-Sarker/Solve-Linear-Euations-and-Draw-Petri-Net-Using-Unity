using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    public void LoadPetriNet()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
