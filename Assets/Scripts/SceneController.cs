using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    public void LoadPetriNet()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLinEq()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
