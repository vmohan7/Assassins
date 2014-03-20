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
			//TODO: Do a raycast
			Rigidbody bulletClone = (Rigidbody) Network.Instantiate (bullet, bulletStart.position, bullet.transform.rotation, 1);
			bulletClone.velocity = this.transform.forward * speed;
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
