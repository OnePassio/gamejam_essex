using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Gamejam.Utils
{
    public class GlobalData : MonoBehaviour
    {
        private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();


        // Use this for initialization
        void Awake()
        {
            //#pragma warning disable 
            DontDestroyOnLoad(gameObject);
            //#pragma warning restore 
            if (cache.ContainsKey(name))
            {
                //			Debug.LogWarning("Object [" + name + "] exists. Destroy new one");
                Object.DestroyImmediate(this.gameObject);
            }
            else
                cache[name] = gameObject;
        }
        void Start()
        {

        }
    }
}