using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringMode : MonoBehaviour, IGraphMode
{
    public GraphManager graphManager;

    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null && hit.transform.CompareTag("Transition"))
        {
            Transition transition = hit.transform.GetComponent<Transition>();
            graphManager.FirePetriNetTransition(transition);
        }
    }
}
