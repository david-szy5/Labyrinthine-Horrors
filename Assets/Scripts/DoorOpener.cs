using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : InteractibleObject
{
    private Animator door_animator;
    private bool doorOpen = false;

    private void Awake()
    {
        door_animator = gameObject.GetComponent<Animator>();
    }

    public override void PlayAnimation()
    {
        if (!doorOpen)
        {
            door_animator.Play("DoorOpen", 0, 0.0f);
        }
        else
        {
            door_animator.Play("DoorClose", 0, 0.0f);
        }

        doorOpen = !doorOpen;
    }
}
