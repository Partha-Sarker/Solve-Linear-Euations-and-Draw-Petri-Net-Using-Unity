using UnityEngine;
using TMPro;
using System;

public class InputMode : MonoBehaviour, IGraphMode
{
    public KeyboardShortcutManager keyboardShortcutManager;


    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            string tag = hit.transform.tag;
            if(tag == "State" || tag == "Edge" || tag == "Transition")
            {
                Debug.Log("Should input now");
                IEditable editable = hit.transform.GetComponent<IEditable>();
                editable.ActivateEditmode();
            }

        }
    }
}
