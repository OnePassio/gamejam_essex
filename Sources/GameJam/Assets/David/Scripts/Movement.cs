using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public int speed;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("up"))
        {
            // Move the object forward along its z axis 1 unit/second.
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey("down"))
        {
            // Move the object backward along its z axis 1 unit/second.
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }
        if (Input.GetKey("left"))
        {
            // Move the object left along its z axis 1 unit/second.
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKey("right"))
        {
            // Move the object right along its z axis 1 unit/second.
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }


        // Move the object upward in world space 1 unit/second.
        //transform.Translate(Vector3.up * Time.deltaTime, Space.World);
    }



}