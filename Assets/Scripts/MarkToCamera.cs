using UnityEngine;
using System.Collections;

public class MarkToCamera : MonoBehaviour {
	
	void Update(){
		Camera mCam = Camera.main;
		transform.LookAt(mCam.transform.position + mCam.transform.rotation * Vector3.back,
		                 mCam.transform.rotation * Vector3.up);
		transform.Rotate (90, 0, 0);
	}
}
