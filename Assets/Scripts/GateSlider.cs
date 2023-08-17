using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSlider : InteractibleObject
{
   private bool isGateSlid;
   private Animator anim;

   private void Awake()
   {
      isGateSlid = false;
      anim = GetComponent<Animator>();
   }

   public override void PlayAnimation()
   {
      if (!isGateSlid)
      {
         anim.Play("SlideGate");
      }
      else
      {
         anim.Play("CloseGate");
      }

      isGateSlid = !isGateSlid;
   }
}
