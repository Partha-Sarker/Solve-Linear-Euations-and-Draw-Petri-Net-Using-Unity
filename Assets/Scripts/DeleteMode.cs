using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMode : MonoBehaviour, IGraphMode
{
    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if ( hit.collider != null && hit.transform.CompareTag("Node") )
        {
            Destroy(hit.transform.gameObject);
        }
    }
}
