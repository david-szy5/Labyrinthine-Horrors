using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepRun : MonoBehaviour
{
    public List<AudioSource> footstepRun;

    private void Awake()
    {
        footstepRun.AddRange(transform.GetComponents<AudioSource>());
    }

    public List<AudioSource> getRunAudio()
    {
        return footstepRun;
    }
}
