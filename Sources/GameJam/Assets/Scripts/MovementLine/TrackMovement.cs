using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class TrackMovement : MonoBehaviour
{
    /// <summary>
    /// track all position of life time note movement, Note 2 is sync Note
    /// </summary>
    [SerializeField]
    private Transform[] linePos;

    /// <summary>
    /// timeDuration : Time movement from 2 position of linePos by order
    /// </summary>
    [SerializeField] 
    private float timeMove=120;//120 second
    

    private float[] timeSum; // pre calculate time by mockup
    private float[] timeDuration;
    /// <summary>
    /// show graphics on gizmos view for easy editing in editor mode
    /// </summary>
    [SerializeField] 
    private bool isDrawGizmos;


    public float GetTimeMove()
    {
        return timeMove;
    }

    /// <summary>
    /// lineColor : Color line in editor mode
    /// </summary>
    [SerializeField] 
    private Color lineColor=Color.blue;

    void Start()
    {
        float[] distance = new float[linePos.Length];
        timeDuration=new float[linePos.Length];
        timeSum=new float[linePos.Length];
        distance[0] = 0;
        timeDuration[0] = 0;

        float sumDist = 0;
        for (int i = 0; i < linePos.Length-1; i++)
        {
            distance[i + 1] = Vector3.Distance(linePos[i].position, linePos[i + 1].position);
            sumDist += distance[i + 1];
        }

        float sum = 0;
        for (int i = 0; i < linePos.Length; i++)
        {
            float timePart = (distance[i]/sumDist)*timeMove;
            
            timeDuration[i] = timePart;
            sum += timePart;
            timeSum[i] = sum;
        }
        Assert.IsTrue(sum-timeMove<0.001f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GetDataByTime(float currentTime, ref Vector3 pos, ref Vector3 scale, ref Transform look, ref bool isFinish)
    {
        if (currentTime < 0)
            currentTime = 0;
        float sum = 0;
        int index = 0;
        for(int i=0;i<timeDuration.Length;i++)
        {
            sum += timeDuration[i];
            index = i;
            if (currentTime < sum)
            {
                break;
            }
        }

        if (currentTime>timeSum[timeSum.Length-1]-Mathf.Epsilon) // run over
        {
            isFinish = true;
            currentTime = sum;
            pos = linePos[linePos.Length - 1].position;
            scale = linePos[linePos.Length - 1].localPosition;
            look = linePos[linePos.Length - 1];
        }
        else
        {
            float t1 = currentTime - timeSum[index-1];
            float t2 = timeSum[index]-timeSum[index-1];
            float percent = t1 / t2;
            pos = Vector3.Lerp(linePos[index-1].position, linePos[index].position,percent);
            scale=Vector3.Lerp(linePos[index-1].localPosition, linePos[index].localPosition,percent);
            look = linePos[index];
            isFinish = false;
        }
    }

    public Vector3 GetShowEffectPosition()
    {
        return linePos[1].position;
    }

    #region  Debug Utility
    public void OnDrawGizmos()
    {
        if (isDrawGizmos) {
            Gizmos.color = lineColor;
            for (int i = 0; i < linePos.Length - 1; i++) {
                Gizmos.DrawSphere(linePos[i].position,0.5f);
                Gizmos.DrawLine (linePos [i].position, linePos [i + 1].position);
            }
            Gizmos.DrawSphere(linePos[linePos.Length - 1].position,0.5f);
        }
    }
    #endregion
}
