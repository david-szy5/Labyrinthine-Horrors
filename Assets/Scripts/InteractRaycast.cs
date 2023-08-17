using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractRaycast : MonoBehaviour
{
    public Camera FPSCam;

    public int rayLength = 5;
    public LayerMask layerMaskInteract;
    [SerializeField] private string excludeLayerName = null;

    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [SerializeField] public RawImage crosshair;

    private bool isCrosshairActive;
    private bool doOnce;

    private const string interactiveTag = "Interactible";

    private InteractibleObject myInteractable;
    public float animCoolDown = 1.0f;
    private bool allowAnimation = true;
    

    private void Update()
    {
        RaycastHit hit;
        Vector3 dirForward = FPSCam.transform.forward;

        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value;

        if (Physics.Raycast(FPSCam.transform.position, dirForward, out hit, rayLength, mask))
        {
            CrosshairChange(true);
            isCrosshairActive = true;

            if (hit.collider.CompareTag(interactiveTag))
            {
                if (!doOnce)
                {
                    // i've refactored this but its kind of a sad workaround at the moment
                    // myInteractable = hit.collider.gameObject.GetComponentInParent<InteractibleObject>();
                    GameObject parent = hit.collider.gameObject.transform.parent.gameObject;
                    Debug.Log(parent);
                    if (parent.GetComponent<InteractibleObject>() != null)
                    {
                        myInteractable = parent.GetComponent<InteractibleObject>();
                    }
                    else
                    {
                        myInteractable = parent.GetComponentInParent<InteractibleObject>();
                    }
                    
                }

                
                doOnce = true;
                
                // check if we are opening the door
                if (Input.GetKeyDown(interactKey) && allowAnimation)
                {
                    myInteractable.PlayAnimation();
                    allowAnimation = false;
                    Invoke(nameof(ResetAllowAnimation), animCoolDown);
                    
                }

            }
            
        }
        else
        {
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                doOnce = false;
            }
        }


    }

    private void ResetAllowAnimation()
    {
        allowAnimation = true;
    }
    private void CrosshairChange(bool change)
    {
        isCrosshairActive = change;
        if (isCrosshairActive)
        {
            crosshair.color = Color.magenta;
        }
        else
        {
            crosshair.color = Color.white;
        }
        // add some other stuff to change the color of the crosshair when interactible
    }
}
