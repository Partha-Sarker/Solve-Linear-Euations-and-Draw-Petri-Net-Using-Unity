using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Color logEnableColor, logDisableColor;
    public CanvasGroup logCanvasGroup;
    public Image logImage;
    public GraphManager graphManager;
    public KeyboardShortcutManager shortcutManager;
    public CameraController camController;
    public TMP_InputField speedInput;
    public Sprite play, pause, stop;
    public Button playButton;
    public RectTransform logParent;
    public GameObject log;
    public bool logScreenEnabled = false;

    private void Start()
    {
        float speed = PlayerPrefs.GetFloat("speed", 1);
        speedInput.text = speed.ToString();
    }

    public void SetPlayUI()
    {
        playButton.GetComponent<Image>().sprite = play;
    }

    public void SetStopUI()
    {
        playButton.GetComponent<Image>().sprite = stop;
    }

    public void SetPauseUI()
    {
        playButton.GetComponent<Image>().sprite = pause;
    }

    public void AddLog(string logText)
    {
        GameObject newLog = Instantiate(log, logParent);
        newLog.GetComponent<TextMeshProUGUI>().text = logText;
    }

    public void OnBottomPanelMouseEnter()
    {
        graphManager.pendingStuffs++;        
    }

    public void OnBottomPanelMouseExit()
    {
        graphManager.pendingStuffs--;
    }

    public void OnLogPanelMouseEnter() 
    {
        if (camController.isDragging)
            return;

        camController.canZoom = false;
        if (logScreenEnabled)
            graphManager.pendingStuffs++;
    }

    public void OnLogPanelMouseExit() 
    {
        camController.canZoom = true;
        if (logScreenEnabled)
            graphManager.pendingStuffs--;
    }

    public void OnSpeedInputSelect()
    {
        Debug.Log("Speed input select");
        graphManager.pendingStuffs++;
        shortcutManager.enableShortcut = false;
    }

    public void OnSpeedInputEndEdit()
    {
        Debug.Log("Speed input end edit");

        string speedString = speedInput.text;
        if (speedString == null || speedString == "" || speedString == "-" || speedString == "0" || speedString == "-0")
        {
            speedString = "1";
        }
        float speed = float.Parse(speedString);
        if (speed < 0) speed *= -1;
        PlayerPrefs.SetFloat("speed", speed);
        PlayerPrefs.Save();
        speedInput.text = speed.ToString();

        graphManager.pendingStuffs--;
        shortcutManager.enableShortcut = true;
    }

    public void ToggleLogScreenVisibility()
    {
        if (logScreenEnabled)
        {
            logCanvasGroup.alpha = .5f;
            logImage.color = logDisableColor;
        }
        else
        {
            logCanvasGroup.alpha = 1;
            logImage.color = logEnableColor;
        }
        logScreenEnabled = !logScreenEnabled;
    }
}
