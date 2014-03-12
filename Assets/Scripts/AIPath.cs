using UnityEngine;
using System.Collections;

public class AIPath : MonoBehaviour {
	private NavMeshAgent agent;
	public Vector3 destination;
	// Use this for initialization

	Vector3 getNewDestination(){
		return new Vector3(Random.Range(-100.0F, 100.0F), 0, Random.Range(-100.0F, 100.0F));
	}

	void Start () {
		agent = GetComponent<NavMeshAgent>();
		destination = this.gameObject.transform.position;
		agent.SetDestination (destination);
	}
	
	// Update is called once per frame
	void Update () {
		if (agent.remainingDistance < 3 && Network.isServer) {
			destination = getNewDestination();
			agent.SetDestination (destination);
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncDest = Vector3.zero;
		if (stream.isWriting && Network.isServer)
		{
			syncDest = destination;
			stream.Serialize(ref syncDest);
		}
		else if (stream.isReading)
		{
			stream.Serialize(ref syncDest);
			destination = syncDest;
			agent.SetDestination (destination);
		}
	}
}
