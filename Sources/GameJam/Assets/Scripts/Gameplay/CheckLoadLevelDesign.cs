using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLoadLevelDesign : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        TextAsset textAsset = (TextAsset) Resources.Load("Pegasus");
        if (textAsset != null)
        {
            ConfigLevelDesign design = ConfigService.LoadLevelDesign(textAsset.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
