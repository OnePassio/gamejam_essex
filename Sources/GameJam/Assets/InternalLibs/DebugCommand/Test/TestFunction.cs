using UnityEngine;
using System.Collections;

public class TestFunction : MonoBehaviour {

	public enum ABC{
		a=1,
		b=2,
		fuck=3
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Test1(){
		Debug.LogError("TestFunction: test1");
	}

	public void Test2(string a){
		Debug.LogError("TestFunction: test2:"+a);
	}

	public void Test3(string a,int b){
		Debug.LogError("TestFunction: test2:"+a);
	}

	public void Test3(GameObject a,ABC b){
		Debug.LogError("TestFunction: Test3:"+a+","+b.ToString());
	}
}
