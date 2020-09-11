using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class State : Node, IEditable
{
    [HideInInspector]
    public int tokenCount = 0;
    public TMP_InputField inputField;
    public TextMeshProUGUI tokenCountText;

    public void OnEndEdit()
    {
        FindObjectOfType<KeyboardShortcutManager>().EnableShortcut = true;
        inputField.gameObject.SetActive(false);
        string text = inputField.text;
        if (text == "")
            text = "0";
        tokenCountText.text = text;
        tokenCount = Int32.Parse(text);
    }

    public void ActivateEditmode()
    {
        FindObjectOfType<KeyboardShortcutManager>().EnableShortcut = false;
        inputField.gameObject.SetActive(true);
        inputField.text = tokenCountText.text;
        inputField.ActivateInputField();
        tokenCountText.text = "";
    }
}
