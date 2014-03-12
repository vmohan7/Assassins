using UnityEngine;
using System.Collections;

public class GunFire : MonoBehaviour {

	public Transform bulletStart;
	public Rigidbody bullet;
	public float speed = 50.0f;

	private const double fireRate = 0.5; //only allow fire every 10 seconds
	private double nextFire = 0.0; //time of next shot



	void FireBullet(){
		if (networkView.isMine) {
			FireBulletFromLocation (bulletStart.position, bulletStart.forward);
			networkView.RPC("FireBulletFromLocation", RPCMode.OthersBuffered, bulletStart.position, bulletStart.forward);
		}
	}

	void Update () {

		//TODO put a limit on the number of bullets and have a reload
		if (Input.GetButton ("Fire1") && Time.time > nextFire ) {
			nextFire = Time.time + fireRate;
			FireBullet();
		}
	}

	[RPC] void FireBulletFromLocation(Vector3 bulletLoc, Vector3 forward){
		Rigidbody bulletClone = (Rigidbody) Instantiate (bullet, bulletLoc, bullet.transform.rotation);
		bulletClone.velocity = forward * speed;
	}
	
}
