using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	void OnCollisionEnter(Collision collision){
		Destroy (this.gameObject);
		//TODO logic if this is a player for AI
	}
}
