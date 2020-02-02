using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(0, 200, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
