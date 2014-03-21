using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public const string PLAYER_NAME = "Network Player";

	[RPC] public void SetID(string id){
		this.gameObject.name = PLAYER_NAME + id;

		//TODO see why the first connection has a empty network id
		if (networkView.isMine) {
			networkView.RPC ("SetID", RPCMode.OthersBuffered, id);
		}
	}

}
