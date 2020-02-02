using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCenterTrack : MonoBehaviour
{
    public Transform center;
    public Transform left;
    public Transform right;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMovement(TrackMovement track, float cacheTime)
    {
        if (cacheTime > track.GetTimeMove())
        {
            cacheTime -= track.GetTimeMove();
        }

        cacheTime += Time.smoothDeltaTime;
        Vector3 pos=Vector3.zero;
        Vector3 scale=Vector3.one;
        Transform look=this.transform;
        bool isFinish=false;
        track.GetDataByTime(cacheTime,ref pos, ref scale, ref look,ref isFinish);
        center.position = pos;
        center.LookAt(look);
    }

    public Transform GetCenter()
    {
        return center;
    }
    
    public Transform GetLeft()
    {
        return left;
    }
    
    public Transform GetRight()
    {
        return right;
    }

}
