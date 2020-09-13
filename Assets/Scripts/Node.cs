using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public int position;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }
}
