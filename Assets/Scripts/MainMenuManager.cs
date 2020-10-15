using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    public TMP_InputField speedInput;

    public void OnSettingsEnabled()
    {
        float speed = PlayerPrefs.GetFloat("speed", 1);
        speedInput.text = speed.ToString();
    }

    public void OnSpeedInputEndEdit()
    {
        string speedString = speedInput.text;
        if(speedString == null || speedString == "" || speedString == "-" || speedString == "0" || speedString == "-0")
        {
            speedString = "1";
        }
        float speed = float.Parse(speedString);
        if (speed < 0) speed *= -1;
        PlayerPrefs.SetFloat("speed", speed);
        PlayerPrefs.Save();
        speedInput.text = speed.ToString();
    }
}
