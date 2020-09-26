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
    public CameraController camController;
    public Sprite play, pause, stop;
    public Button playButton;
    public RectTransform logParent;
    public GameObject log;
    public bool logScreenEnabled = false;

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
        graphManager.enableGraph = false;        
    }

    public void OnBottomPanelMouseExit()
    {
        graphManager.enableGraph = true;
    }

    public void OnLogPanelMouseEnter() 
    {
        if (camController.isDragging)
            return;

        camController.canZoom = false;
        if (logScreenEnabled)
            graphManager.enableGraph = false;
    }

    public void OnLogPanelMouseExit() 
    {
        camController.canZoom = true;
        if (logScreenEnabled)
            graphManager.enableGraph = true;
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
