using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Node
{
    public SpriteRenderer spriteRenderer;
    public TransitionColor transitionColor;
    public float resetDelay = 1;

    private void Start()
    {
        spriteRenderer.color = transitionColor.defaultColor;
    }

    public void SetFiringColor()
    {
        spriteRenderer.color = transitionColor.activeColor;
        Invoke("ResetColor", resetDelay);
    }

    private void ResetColor()
    {
        spriteRenderer.color = transitionColor.defaultColor;
    }
}
