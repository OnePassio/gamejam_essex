using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheInitialPosition : MonoBehaviour
{
    private Vector3 pos;

    private Vector3 scale;

    private Vector3 rotate;


    public void Awake()
    {
        pos = this.transform.position;
        scale = this.transform.localScale;
        rotate = this.transform.eulerAngles;
    }

    public void ResetBeginning()
    {
        this.transform.position=pos;
        this.transform.localScale=scale;
        this.transform.eulerAngles=rotate;
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
