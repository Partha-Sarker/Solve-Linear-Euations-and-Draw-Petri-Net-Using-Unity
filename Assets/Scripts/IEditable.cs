using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface IEditable
{
    void OnEndEdit();

    void ActivateEditmode();
}
