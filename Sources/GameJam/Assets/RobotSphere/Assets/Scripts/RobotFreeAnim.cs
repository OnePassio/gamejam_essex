using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFreeAnim : MonoBehaviour {

	[SerializeField]
	private Vector3 rot = Vector3.zero;
	[SerializeField]
	float rotSpeed = 40f;
	[SerializeField]
	Animator anim;

	// Use this for initialization
	void Awake()
	{
		transform.eulerAngles = rot;
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 vec=transform.eulerAngles;
		vec =vec+rot;  
		transform.eulerAngles = vec;
	}
	

}
