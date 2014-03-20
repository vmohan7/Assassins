using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScore : MonoBehaviour {

	/**
	 * A map that takes a guid of a NetworkPlayerr and gives their score
	 * */
	private Dictionary<string, int> playerScores;
	private int numPlayers;

	public Camera networkCamera;

	public const int REWARD = 10;
	public const int PENALTY = 10;

	public void StartGame() {
		playerScores = new Dictionary<string, int> ();
		NetworkPlayer[] players = Network.connections;

		foreach (NetworkPlayer player in players){
			playerScores.Add(player.guid, 0);
		}

		//add the server player also
		playerScores.Add(Network.player.guid, 0); 
		numPlayers = playerScores.Count;
	}

	public void OnKillHuman(string playerID){
		playerScores [playerID] += REWARD;
		numPlayers--;
	}

	public void OnKillAgent(string playerID){
		playerScores [playerID] -= PENALTY;
	}

	[RPC] void OnNetworkCollision(bool isHuman, string playerID, string collideObjectName){
		//TODO do game mechs here
		//The game score should maintain how many humans are left and send an RPC when there is only 1 person left
		//It should also turn on the network camera when you die

		GameObject capsule = GameObject.Find (collideObjectName);
		if (isHuman) {
			if (Network.isServer){ //only maintain the score on the server
				OnKillHuman( playerID );
				if (numPlayers == 1){ //only one player standing
					//the shooter is the winner
					networkView.RPC ("OnGameOver", RPCMode.AllBuffered, playerID);
				}
			}

			if ( capsule.networkView.owner.guid.Equals(Network.player.guid) ){
				networkCamera.gameObject.SetActive(true);	
				( (NetworkManager) networkCamera.GetComponent<NetworkManager>() ).SetGameOver(false);
			}
			BotControlScript control = capsule.GetComponent<BotControlScript>();
			if ( !control.isKilled() ){
				control.GotKilled();
				this.StartCoroutine( FadeDeath(capsule, BotControlScript.animSpeed) );
			}
		} else {
			if (Network.isServer){ //only maintain the score on the server
				OnKillAgent( playerID );
				networkView.RPC ("markPlayer", RPCMode.AllBuffered, playerID);
			}

			AIControlScript control = capsule.GetComponent<AIControlScript>();
			if ( !control.isKilled() ){
				control.GotKilled();
				this.StartCoroutine( FadeDeath(capsule, AIControlScript.animSpeed) );
			}
		}

		//TODO: remove capsule from field

	}

	[RPC] void markPlayer(string playerID){
		GameObject player = GameObject.Find ("Network Controller" + playerID);
		if (player != null)
			player.GetComponentInChildren<MarkToCamera> ().gameObject.SetActive (true);
	}

	IEnumerator FadeDeath(GameObject capsule, float animSpeed) {
		Renderer render = capsule.GetComponentInChildren<Renderer> ();
		yield return new WaitForSeconds (2*animSpeed);

		//TODO: see why we are not fading away
		/*
		for (float f = 1f; f >= 0; f -= 0.1f) {
			Color c = render.material.color;
			c.a = f;
			render.material.SetColor("_Color", c);
			yield return new WaitForSeconds(0.5f);
		}
		*/

		Destroy (capsule);
		yield return new WaitForSeconds(0.0f);
	}

	[RPC] void OnGameOver(string winner){
		if ( winner.Equals(Network.player.guid) ) {
			networkCamera.gameObject.SetActive(true);	
			( (NetworkManager) networkCamera.GetComponent<NetworkManager>() ).SetGameOver(true);
		}
	}
	
}
