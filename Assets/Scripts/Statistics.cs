using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class Statistics : MonoBehaviour
{
    public int shotsFired;
    public int shotsHit;
    public int numPickups;
    public double timeToKey;
    public int gateCollisions;

    // Start is called before the first frame update
    void Start()
    {
        shotsFired = 0;
        shotsHit = 0;
        numPickups = 0;
        timeToKey = 0;
        gateCollisions = 0;
    }

    public void fireShot()
    {
        shotsFired += 1;
        
    }

    public void hitShot()
    {
        shotsHit += 1;
        
    }

    public void pickup()
    {
        numPickups += 1;
        
    }

    public void findKey()
    {
        timeToKey = Time.realtimeSinceStartup;
        
    }

    public void gateCollide()
    {
        gateCollisions += 1;
        
    }
}
