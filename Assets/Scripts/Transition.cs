using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Node
{
    public SpriteRenderer spriteRenderer;
    public TransitionColor transitionColor;
    public float fireDuration = 1;
    public int resetReq = 0;

    private void Start()
    {
        spriteRenderer.color = transitionColor.defaultColor;
    }
    public void SetFiringColor(float duration)
    {
        resetReq++;
        spriteRenderer.color = transitionColor.activeColor;
        Invoke(nameof(ResetColor), duration);
    }

    public void SetFiringColor()
    {
        SetFiringColor(fireDuration);
    }

    private void ResetColor()
    {
        if(resetReq == 1)
            spriteRenderer.color = transitionColor.defaultColor;
        resetReq--;
    }
}
