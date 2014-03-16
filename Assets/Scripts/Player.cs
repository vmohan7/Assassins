using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private string agentID;

	[RPC] public void SetID(string id){
		agentID = id;
		this.gameObject.name += agentID;

		//TODO see why the first connection has a empty network id
		if (networkView.isMine) {
			networkView.RPC ("SetID", RPCMode.OthersBuffered, id);
		}
	}

}
