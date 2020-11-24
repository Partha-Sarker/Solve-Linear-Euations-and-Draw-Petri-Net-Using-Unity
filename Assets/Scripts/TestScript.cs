using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform g1, g2, m;
    Vector2 midPos;
    public float offset = 2;

    // Start is called before the first frame update
    void Start()
    {
        TestMethod(20);
    }

    // Update is called once per frame
    void Update()
    {
        midPos = (g1.position + g2.position) / 2;
        m.position = midPos;
        midPos = g2.position - g1.position;
        float angle = Mathf.Atan2(midPos.y, midPos.x);
        m.eulerAngles = new Vector3(0, 0, angle) * Mathf.Rad2Deg;
        midPos = m.position;
        midPos += (Vector2)m.up * offset;
        m.position = midPos;
    }

    public void TestMethod(int h = 10)
    {

    }
}
