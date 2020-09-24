using System;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour, IEditable
{
    [HideInInspector]
    public int value;
    [HideInInspector]
    public TextMeshProUGUI titleText;
    public TMP_InputField valueInputField;
    public TextMeshProUGUI valueText;
    public int position;

    public void ActivateEditmode()
    {
        FindObjectOfType<KeyboardShortcutManager>().enableShortcut = false;
        valueInputField.gameObject.SetActive(true);
        valueInputField.ActivateInputField();
        valueInputField.text = valueText.text;
        valueText.text = "";
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void OnEndEdit()
    {
        FindObjectOfType<KeyboardShortcutManager>().enableShortcut = true;
        valueInputField.gameObject.SetActive(false);
        string text = valueInputField.text;
        if (text == "")
            text = "0";
        value = Int32.Parse(text);
        valueText.text = text;
    }

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }

    public void SetValueText()
    {
        valueText.text = value.ToString();
    }
}
