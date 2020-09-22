using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Sprite play, pause, stop;
    public Button playButton;

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
}
