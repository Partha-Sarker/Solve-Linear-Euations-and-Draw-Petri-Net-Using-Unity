using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class Edge : MonoBehaviour, IEditable
{
    public GameObject triangle, input;
    [HideInInspector] public Node fromNode, toNode;
    [HideInInspector] public Vector3[] linePoints;
    public int weight = 1;
    public float offset = 2;
    public LineRenderer line;
    public BoxCollider2D col;
    public TMP_InputField inputField;
    public TextMeshProUGUI weightText;
    public RectTransform inputCanvas;
    private GraphManager graphManager;
    private KeyboardShortcutManager shortcutManager;


    private void Start()
    {
        graphManager = FindObjectOfType<GraphManager>();
        shortcutManager = FindObjectOfType<KeyboardShortcutManager>();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ActivateEditmode()
    {
        shortcutManager.enableShortcut = false;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
        inputField.text = weightText.text;
        weightText.text = "";
    }

    public void OnEndEdit()
    {
        FindObjectOfType<KeyboardShortcutManager>().enableShortcut = true;
        inputField.gameObject.SetActive(false);
        string text = inputField.text;
        if (text == "" || text == "0")
            text = "1";
        weight = Int32.Parse(text);
        weightText.text = text;
    }

    public void SetWeightText()
    {
        weightText.text = weight.ToString();
    }

    public void Init()
    {
        print("Init");
        int pointCount = line.positionCount;
        if (linePoints.Length == 0)
        {
            linePoints = new Vector3[pointCount];
            for (int i = 0; i < pointCount; i++)
                linePoints[i] = line.GetPosition(i);
        }
        else
        {
            line.positionCount = linePoints.Length;
            for (int i = 0; i < linePoints.Length; i++)
            {
                line.SetPosition(i, linePoints[i]);
            }
        }
        triangle.SetActive(true);
        input.SetActive(true);
        Vector2 startPos = linePoints[linePoints.Length - 2];
        Vector2 endPos = linePoints[linePoints.Length - 1];
        Vector3 midPos = (startPos + endPos) / 2;
        midPos.z = 10; // putting edge behind the nodes so that node gets clicked first
        transform.position = midPos;

        
        //float lineLength = Vector2.Distance(startPos, endPos);

        //line.SetPosition(0, startPos);
        //line.SetPosition(1, endPos);

        //BoxCollider2D col = line.GetComponent<BoxCollider2D>();
        //col.size = new Vector2(lineLength, col.size.y);

        Vector2 diff = endPos - startPos;
        float angle = Mathf.Atan2(diff.y, diff.x);
        transform.eulerAngles = new Vector3(0, 0, angle) * Mathf.Rad2Deg;

        inputCanvas.rotation = Quaternion.identity;
    }

    public void MoveRight()
    {
        print("MoveRight");
        //Vector2 startPos = fromNode.transform.position;
        //Vector2 endPos = toNode.transform.position;
        //Vector3 midPos = (startPos + endPos) / 2;

        //midPos += transform.up * offset;
        //startPos += (Vector2)transform.up * offset;
        //endPos += (Vector2)transform.up * offset;
        //midPos.z = 10;

        //line.SetPosition(0, startPos);
        //line.SetPosition(1, endPos);
        //transform.position = midPos;
    }

    public void SetDefaultPos()
    {
        print("SetDefaultPos");
        //Vector2 startPos = fromNode.transform.position;
        //Vector2 endPos = toNode.transform.position;
        //Vector3 midPos = (startPos + endPos) / 2;

        //midPos.z = 10;

        //line.SetPosition(0, startPos);
        //line.SetPosition(1, endPos);
        //transform.position = midPos;
    }

}
