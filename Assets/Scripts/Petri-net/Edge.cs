using UnityEngine;
using TMPro;
using System;

public class Edge : MonoBehaviour, IEditable
{
    //[HideInInspector]
    public Node fromNode, toNode;
    public int weight = 1;
    public LineRenderer line;
    public BoxCollider2D col;
    public TMP_InputField inputField;
    public TextMeshProUGUI weightText;
    public RectTransform inputCanvas;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void OnEndEdit()
    {
        FindObjectOfType<KeyboardShortcutManager>().EnableShortcut = true;
        inputField.gameObject.SetActive(false);
        string text = inputField.text;
        if (text == "" || text == "0")
            text = "1";
        weight = Int32.Parse(text);
        weightText.text = text;
    }

    public void ActivateEditmode()
    {
        FindObjectOfType<KeyboardShortcutManager>().EnableShortcut = false;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
        inputField.text = weightText.text;
        weightText.text = "";
    }

    public void SetWeightText()
    {
        weightText.text = weight.ToString();
    }

    public void Init()
    {
        Vector2 startPos = fromNode.transform.position;
        Vector2 endPos = toNode.transform.position;
        Vector3 midPos = (startPos + endPos) / 2;
        midPos.z = 10; // putting edge behind the nodes so that node gets clicked first
        transform.position = midPos;

        float lineLength = Vector2.Distance(startPos, endPos);

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

        BoxCollider2D col = line.GetComponent<BoxCollider2D>();
        col.size = new Vector2(lineLength, col.size.y);

        Vector2 diff = endPos - startPos;
        float angle = Mathf.Atan2(diff.y, diff.x);
        transform.eulerAngles = new Vector3(0, 0, angle) * Mathf.Rad2Deg;

        inputCanvas.rotation = Quaternion.identity;
    }
}
