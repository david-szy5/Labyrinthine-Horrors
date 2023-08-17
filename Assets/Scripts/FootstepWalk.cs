using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepWalk : MonoBehaviour
{
    public List<AudioSource> footstepWalk;

    private void Awake()
    {
        footstepWalk.AddRange(transform.GetComponents<AudioSource>());
    }

    public List<AudioSource> getWalkAudio()
    {
        return footstepWalk;
    }
}
