using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    public int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        
        //Collect and build next part
        if (other.gameObject.tag.Equals("Part"))
        {
            count++;
            GetComponent<Build>().BuildPiece(count);

            //Destroy the collectable part
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag.Equals("Obstacle"))
        {
            GetComponent<Build>().ObstacleHitted();
        }


    }

    public int GetCount()
    {
        return this.count;
    }


}
