using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;

public class GameGraphicsManager : SingletonMono<GameGraphicsManager>
{
    public List<GameObject> objForGraphicsMode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowGraphics()
    {
        bool isActive = true;
        int mode = PlayerPrefs.GetInt("GRAPHICS_MODE", 1);
        if (mode == 0)
        {
            isActive = false;
        }

        if (objForGraphicsMode != null)
        {
            for (int i = 0; i < objForGraphicsMode.Count; i++)
            {
                if(objForGraphicsMode[i]!=null){
                    objForGraphicsMode[i].SetActive(isActive);
                }
            }
        }

        
    }
}
