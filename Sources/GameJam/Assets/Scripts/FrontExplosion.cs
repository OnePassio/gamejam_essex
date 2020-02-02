using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(1000, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
