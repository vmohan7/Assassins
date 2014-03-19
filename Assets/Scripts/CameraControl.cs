using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private Camera myCam; 

	void Start(){
		//do not use camera's that are not yours
		//if (!networkView.isMine) {
		myCam = this.GetComponentInChildren<Camera> ();
		myCam.gameObject.SetActive(false);	
		//}
	}

	public void TurnOnCamera(){
		//do not use camera's that are not yours
		if (networkView.isMine) {
			myCam.gameObject.SetActive(true);	
		}

		if (Network.isServer) {
			this.StartCoroutine( TransitionNight() );	
		}
	}

	private int TIME_TILL_NIGHT = 100;
	
	IEnumerator TransitionNight() {
		skydomeScript2 sky = GameObject.Find ("Skydome controller").GetComponent<skydomeScript2> ();
		for (int i = 0; i < TIME_TILL_NIGHT; i++) {
			sky.TIME += ( 1.0F ) /TIME_TILL_NIGHT;
			networkView.RPC ("ChangeDayTime", RPCMode.OthersBuffered, sky.TIME);
			yield return new WaitForSeconds (1.0F); 
		}
	}

	//TODO check if this works
	[RPC] void ChangeDayTime(float time){
		skydomeScript2 sky = GameObject.Find ("Skydome controller").GetComponent<skydomeScript2> ();
		sky.TIME = time; 
	}

}
