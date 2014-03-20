using UnityEngine;
using System.Collections;

public class GunFire : MonoBehaviour {

	public Transform bulletStart;
	public Rigidbody bullet;
	public float speed = 50.0f;

	private const double fireRate = 0.5; //only allow fire every 10 seconds
	private double nextFire = 0.0; //time of next shot

	private GameScore score;
	
	void Start(){
		score = (GameScore) GameObject.Find("Town").GetComponent("GameScore");
	}

	void FireBullet(){
		//TODO: move the bulletStart to right in front of the person
		if (networkView.isMine) {
			RaycastHit hit;
			Ray shoot = new Ray(bulletStart.position, transform.forward);
			if (Physics.Raycast(shoot, out hit) ){
				if ( hit.collider.gameObject.CompareTag( "Player" ) ){
					score.networkView.RPC ("OnNetworkCollision", RPCMode.AllBuffered, true, 
					                       networkView.owner.guid, hit.collider.gameObject.name);
				} else if ( hit.collider.gameObject.CompareTag( "AI" ) ){
					score.networkView.RPC ("OnNetworkCollision", RPCMode.AllBuffered, false, 
					                       networkView.owner.guid, hit.collider.gameObject.name);
				}
			}

			//Rigidbody bulletClone = (Rigidbody) Network.Instantiate (bullet, bulletStart.position, bullet.transform.rotation, 1);
			//bulletClone.velocity = this.transform.forward * speed;
		}
	}

	void Update () {

		//TODO put a limit on the number of bullets and have a reload
		if (Input.GetButton ("Fire1") && Time.time > nextFire ) {
			nextFire = Time.time + fireRate;
			FireBullet();
		}
	}

}
