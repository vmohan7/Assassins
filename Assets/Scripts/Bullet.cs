using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public GameScore score;

	void Start(){
		score = (GameScore) GameObject.Find("Town").GetComponent("GameScore");
	}

	void OnCollisionEnter(Collision collision){
		//destroys the bullet
		Destroy (this.gameObject);

		//assume that the server is the TRUE state
		if (Network.isServer) {
			if ( collision.gameObject.CompareTag( "Player" ) ){
				networkView.RPC ("OnNetworkCollision", RPCMode.AllBuffered, true, networkView.owner.guid, collision.gameObject.name);
			} else if ( collision.gameObject.CompareTag( "AI" ) ){
				networkView.RPC ("OnNetworkCollision", RPCMode.AllBuffered, false, networkView.owner.guid, collision.gameObject.name);
			}

		}
	}

	[RPC] void OnNetworkCollision(bool isHuman, string playerID, string collideObjectName){
		Destroy( GameObject.Find(collideObjectName) );
		if (isHuman) {
			score.OnKillHuman( playerID );
		} else {
			score.OnKillAgent( playerID );
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 velocity = Vector3.zero;
		if (stream.isWriting && rigidbody.velocity.magnitude != 0)
		{
			velocity = rigidbody.velocity;
			stream.Serialize(ref velocity);
		}
		else
		{
			stream.Serialize(ref velocity);
			rigidbody.velocity = velocity;
		}
	}

}
