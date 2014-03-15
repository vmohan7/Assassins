using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScore : MonoBehaviour {

	/**
	 * A map that takes a guid of a NetworkPlayerr and gives their score
	 * */
	private Dictionary<string, int> playerScores;

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
	}

	public void OnKillHuman(string playerID){
		playerScores [playerID] += REWARD;
	}

	public void OnKillAgent(string playerID){
		playerScores [playerID] -= PENALTY;
	}
	
}
