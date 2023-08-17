using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimationEnd : MonoBehaviour
{
    public void EndAnimation() {
        gameObject.SetActive(false);
    }
}
