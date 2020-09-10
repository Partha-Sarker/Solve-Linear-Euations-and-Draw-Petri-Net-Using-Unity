using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    //[HideInInspector]
    public Node fromNode, toNode;
    public int weight = 0;
    public LineRenderer line;
    public BoxCollider2D col;

    public void DestroySelf()
    {
        Destroy(gameObject);
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
    }
}
