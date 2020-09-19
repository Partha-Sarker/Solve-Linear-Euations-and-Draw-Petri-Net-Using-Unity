using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Node
{
    public SpriteRenderer spriteRenderer;
    public Color fireColor;
    public Color defaultColor;
    public float resetDelay = 1;

    public void SetFiringColor()
    {
        spriteRenderer.color = fireColor;
        Invoke("ResetColor", resetDelay);
    }

    private void ResetColor()
    {
        spriteRenderer.color = defaultColor;
    }
}
