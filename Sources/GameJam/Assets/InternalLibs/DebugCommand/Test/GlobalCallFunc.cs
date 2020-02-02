using UnityEngine;
using System.Collections;
using OneP.Samples;
using System.Linq;
using System;
using System.Reflection;
public class GlobalCallFunc : SingletonMono<GlobalCallFunc> {
	public TestFunction testFunc;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Test1(){
		// get all public static methods of given type(public would suffer in your case, only to show how you could other BindingFlags)
		Type myType =(typeof(TestFunction));
		// Get the public methods.
		MethodInfo[] myArrayMethodInfo = myType.GetMethods();

		Debug.LogError (myArrayMethodInfo.Length);
		for (int i=0; i<myArrayMethodInfo.Length; i++) {
			Debug.LogError (Pathfinding.Serialization.JsonFx.JsonWriter.Serialize (myArrayMethodInfo [i].Name+","+myArrayMethodInfo[i].GetParameters().Length));
			ParameterInfo[] param= myArrayMethodInfo[i].GetParameters();
			for(int x=0;x<param.Length;x++){
				Debug.LogError(param[x].ParameterType.ToString());
				Debug.LogError(param[x].ToString());
			}
		}
	}
	public void Test2(){
		testFunc.SendMessage ("Test1");
	}
	public void Test3(){
		testFunc.SendMessage ("Test2", "abcd");
	}
}
