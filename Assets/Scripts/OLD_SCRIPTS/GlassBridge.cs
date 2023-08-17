using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBridge : MonoBehaviour
{
    public List<GameObject> glassRows;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject glassRow in glassRows)
        {
            // each glass row has 4 children
            for (int i = 0; i < 4; i++)
            {
                GameObject cube = glassRow.transform.GetChild(i).gameObject;
                if (cube.tag.Equals("drop"))
                {
                    Rigidbody cubeRigid = cube.GetComponent<Rigidbody>();
                    cubeRigid.isKinematic = true;
                    cubeRigid.useGravity = true;
                    glassRow.GetComponent<Rigidbody>().useGravity = true;
                }
            }
            
        }
    }
}
