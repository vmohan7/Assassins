using UnityEngine;
using System.Collections;

public class AIPath : MonoBehaviour {
	private NavMeshAgent agent;
	public Vector3 destination;
	private NavMeshPath tempPath;
	private Vector3 finalDestination;
	private bool onTempPath;
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
		onTempPath = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!onTempPath){
			if (agent.remainingDistance < 5 && Network.isServer) {
				destination = getNewDestination();
				agent.SetDestination (destination);
			}
		}
	/*	else{
			if (agent.remainingDistance < .2 && Network.isServer){
				destination = finalDestination;
				onTempPath = false;
				agent.SetDestination(destination);
			}
		}*/
		bool death = GetComponent<AIControlScript> ().death;
		if (death){
			agent.Stop();
		}

		/*NavMeshHit hit;


		if (agent.Raycast (agent.transform.position + agent.velocity * 2, out hit)){
			if( hit.distance < 2 && Mathf.Abs (Vector3.Dot (hit.normal.normalized, agent.velocity.normalized)) < .5) {
				onTempPath = true;
				finalDestination = agent.destination;
				Vector3 offset = Quaternion.AngleAxis (-45, agent.transform.up) * agent.velocity * 2;
				Vector3 target = agent.transform.position + offset;

				NavMesh.SamplePosition(target, out hit, MAX_SEARCH, 1);
				agent.SetDestination(hit.position);
			}
		}*/
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
