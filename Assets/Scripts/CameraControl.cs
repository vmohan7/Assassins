using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	//private Camera myCam; 
	private GameObject rotatePnt;
	
	void Start(){
		//do not use camera's that are not yours
		//if (!networkView.isMine) {
		rotatePnt = this.transform.FindChild("RotatePoint").gameObject;
		rotatePnt.SetActive (false);

		//myCam = this.GetComponentInChildren<Camera> ();
		//myCam.gameObject.SetActive(false);	
		//}
	}

	public void TurnOnCamera(){
		//do not use camera's that are not yours
		if (networkView.isMine) {
			rotatePnt.SetActive(true);	
			skydomeScript2 sky = GameObject.Find ("Skydome controller").GetComponent<skydomeScript2> ();
			sky.cam = rotatePnt.GetComponentInChildren<Camera> ();
		
			if (Network.isServer) {
				this.StartCoroutine( TransitionNight() );	
			}
		}

	}

	private int TIME_TILL_NIGHT = 100;
	private Color eveningColor = new Color(0.6745F, 0.6392F, 0.4980F);
	private Color nightColor = new Color(0.01F, 0.01F, 0.01F);

	//should only be called by the server
	IEnumerator TransitionNight() {
		Debug.Log ("I am here");
		skydomeScript2 sky = GameObject.Find ("Skydome controller").GetComponent<skydomeScript2> ();
		for (int i = 0; i < TIME_TILL_NIGHT; i++) {
			sky.TIME += ( .21F ) /TIME_TILL_NIGHT ;

			float deltaTime = ( (float) i )/ TIME_TILL_NIGHT;
			RenderSettings.ambientLight = Color.Lerp (eveningColor, nightColor, deltaTime);
			networkView.RPC ("ChangeDayTime", RPCMode.OthersBuffered, sky.TIME, deltaTime);
			yield return new WaitForSeconds (0.1F); 
		}
	}


	[RPC] void ChangeDayTime(float time, float deltaTime){
		skydomeScript2 sky = GameObject.Find ("Skydome controller").GetComponent<skydomeScript2> ();
		RenderSettings.ambientLight = Color.Lerp (eveningColor, nightColor, deltaTime);
		sky.TIME = time; 
		Debug.Log("The time is: " + time);
		Debug.Log("The delta time is: " + deltaTime);
	}

}
