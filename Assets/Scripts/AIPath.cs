using UnityEngine;
using System.Collections;

public class AIPath : MonoBehaviour {
	private NavMeshAgent agent;
	public Vector3 destination;
	// Use this for initialization

	private const float MAX_SEARCH = 100.0f;

	Vector3 getNewDestination(){
		Vector3 loc =  new Vector3(Random.Range(-MAX_SEARCH, MAX_SEARCH), 0, Random.Range(-MAX_SEARCH, MAX_SEARCH));
		NavMeshHit hit;
		NavMesh.SamplePosition(loc, out hit, MAX_SEARCH, 1);
		return hit.position;
	}

	void Start () {
		agent = GetComponent<NavMeshAgent>();
		agent.updatePosition = true;
		agent.updateRotation = true;
		destination = this.gameObject.transform.position;
		agent.SetDestination (destination);
	}
	
	// Update is called once per frame
	void Update () {
		if (agent.remainingDistance < 3 && Network.isServer) {
			destination = getNewDestination();
			agent.SetDestination (destination);
		}
		bool death = GetComponent<AIControlScript> ().death;
		if (death){
			agent.Stop();
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncDest = Vector3.zero;
		Vector3 syncPos = Vector3.zero;
		if (stream.isWriting && Network.isServer)
		{
			syncDest = destination;
			syncPos = this.gameObject.transform.position;
			stream.Serialize(ref syncDest);
			stream.Serialize(ref syncPos);
		}
		else if (stream.isReading)
		{
			stream.Serialize(ref syncDest);
			stream.Serialize(ref syncPos);

			destination = syncDest;
			if (agent != null)
				agent.SetDestination (destination);

			Vector3 diffPos = new Vector3( this.gameObject.transform.position.x - syncPos.x, 
			                              this.gameObject.transform.position.y - syncPos.y,
			                              this.gameObject.transform.position.z - syncPos.z);
			if (diffPos.sqrMagnitude >= 1){
				this.gameObject.transform.position = syncPos;
			}
		}

	}
}
