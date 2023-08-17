using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleObject : MonoBehaviour
{
    // This class is set up so that we can just use the InteractableObject structure and call play animation wherever needed.
    public virtual void PlayAnimation()
    {
        Debug.Log("Base Class play animation");
    }
}
