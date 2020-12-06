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
    public GameObject keyboardShortcut;

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
        TextMeshProUGUI textMesh = newLog.GetComponent<TextMeshProUGUI>();
        textMesh.text = logText;
        textMesh.ForceMeshUpdate();
        textMesh.autoSizeTextContainer = true;
    }

    public void ClearLog()
    {
        int childCount = logParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(logParent.GetChild(i).gameObject);
        }
    }

    public void OnBottomPanelMouseEnter()
    {
        graphManager.IncrementPendingStuffs();        
    }

    public void OnBottomPanelMouseExit()
    {
        graphManager.DecrementPendingStuffs();
    }

    public void OnLogPanelMouseEnter() 
    {
        if (camController.isDragging)
            return;

        camController.canZoom = false;
        if (log.activeSelf)
            graphManager.IncrementPendingStuffs();
    }

    public void OnLogPanelMouseExit() 
    {
        camController.canZoom = true;
        if (log.activeSelf)
            graphManager.DecrementPendingStuffs();
    }

    public void OnSpeedInputSelect()
    {
        Debug.Log("Speed input select");
        graphManager.IncrementPendingStuffs();
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

        graphManager.DecrementPendingStuffs();
        shortcutManager.enableShortcut = true;

        string logText = $"Current transition unit is {speed} sec";
        AddLog(logText);
        Debug.Log(logText);
    }

    public void ToggleKeyboardShortcut()
    {
        if (keyboardShortcut.activeSelf)
        {
            keyboardShortcut.SetActive(false);
            graphManager.DecrementPendingStuffs();
        }
        else
        {
            keyboardShortcut.SetActive(true);
            graphManager.IncrementPendingStuffs();
        }
    }

    public void ToggleLogScreenVisibility()
    {
        if (log.activeSelf)
        {
            logCanvasGroup.alpha = .5f;
            logImage.color = logDisableColor;
        }
        else
        {
            logCanvasGroup.alpha = 1;
            logImage.color = logEnableColor;
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
