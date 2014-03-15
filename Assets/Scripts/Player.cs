using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private string agentID;

	[RPC] public void SetID(string id){
		agentID = id;
		this.gameObject.name += agentID;

		if (networkView.isMine) {
			networkView.RPC ("SetID", RPCMode.OthersBuffered, id);
		}
	}

}
