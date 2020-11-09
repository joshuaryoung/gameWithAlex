using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour {

	public Transform cameraTarget;
	public float zAxis;
	// Use this for initialization
	void Start ()
	{
		QualitySettings.vSyncCount = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Vector3 cameraVector = 
		transform.position = new Vector3 (cameraTarget.position.x, cameraTarget.position.y + 1.964f, zAxis);
	}
}
