using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamera : MonoBehaviour
{
    public GameObject constructObject;

    public int rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        constructObject = GameObject.FindGameObjectWithTag("Robot");

        this.transform.parent = constructObject.transform;
    }

    void Update()
    {
        transform.LookAt(constructObject.transform);
        transform.Translate(Vector3.right * Time.deltaTime * rotationSpeed);
    }
}
