using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPull : InteractibleObject
{
    private bool leverPulled;
    private Animator anim;
    public InteractibleObject gate;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        leverPulled = false;
    }

    public override void PlayAnimation()
    {
        if (!leverPulled)
        {
            anim.Play("LeverForward");
        }
        else
        {
            anim.Play("LeverBackward");
        }
        gate.PlayAnimation();
        leverPulled = !leverPulled;
    }
}
