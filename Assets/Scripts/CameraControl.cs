using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	void Start(){
		//do not use camera's that are not yours
		if (!networkView.isMine) {
			this.GetComponentInChildren<Camera>().gameObject.SetActive(false);	
		}
	}

}
