using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class AIControlScript : MonoBehaviour
{
	[System.NonSerialized]					
	public float lookWeight;					// the amount to transition when using head look
	
	//[System.NonSerialized]
	//public Transform enemy;						// a transform to Lerp the camera to during head look
	
	public float animSpeed = 5.0f;				// a public setting for overall animator animation speed
	public float lookSmoother = 3f;				// a smoothing setting for camera motion
	//public bool useCurves;						// a setting for teaching purposes to show use of curves
	public bool death;
	
	
	private Animator anim;							// a reference to the animator on the character
	private NavMeshAgent agent;
	//private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	//private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	//private CapsuleCollider col;					// a reference to the capsule collider of the character
	//private MouseLook mouse;
	
	private float horz;
	private float vert;
	
	
	/* Example States
	 * 
	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
	static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
	static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		// within our FixedUpdate() function below
	static int fallState = Animator.StringToHash("Base Layer.Fall");
	static int rollState = Animator.StringToHash("Base Layer.Roll");
	static int waveState = Animator.StringToHash("Layer2.Wave");
	*/
	
	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent> ();
		//col = GetComponent<CapsuleCollider>();		
		//mouse = GetComponent<MouseLook>();
		if(anim.layerCount ==2)
			anim.SetLayerWeight(1, 1);
		death = false;
	}
	
	
	void FixedUpdate ()
	{

		vert = agent.velocity.magnitude;				// setup v variables as our vertical input axis
		horz = 0;
	
		
		anim.SetBool("Death", death);
		anim.SetFloat("Speed", vert);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		anim.SetFloat("Direction", horz); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		
		//rigidbody.velocity = gameObject.transform.forward * animSpeed * vert;
		anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
		
	}

	public void GotKilled(){
		death = true;
	}
	/*
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		
		bool d = false;
		float v = 0.0F ,h = 0.0F;
		Vector3 syncPos = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		
		if (stream.isWriting && networkView.isMine) {
			d = death;
			v = vert;
			h = horz;
			syncPos = this.gameObject.transform.position;
			rotation = this.gameObject.transform.rotation;
			
			stream.Serialize(ref d);
			stream.Serialize(ref v);
			stream.Serialize(ref h);
			stream.Serialize(ref syncPos);
			stream.Serialize(ref rotation);
		}
		else if (stream.isReading){
			
			stream.Serialize(ref d);
			stream.Serialize(ref v);
			stream.Serialize(ref h);
			stream.Serialize(ref syncPos);
			stream.Serialize(ref rotation);
			
			death = d;
			vert = v;
			horz = h;
			//if (mouse != null)
			//	mouse.rotationX = horz;
			
			//In case the positions get really out of sync
			Vector3 diffPos = this.gameObject.transform.position - syncPos; 
			Vector3 diffRot = this.gameObject.transform.rotation.eulerAngles - rotation.eulerAngles;
			
			if (diffPos.sqrMagnitude >= 1){
				this.gameObject.transform.position = syncPos;
			}
			
			if (diffRot.sqrMagnitude >= 1){	
				this.gameObject.transform.rotation = rotation;
			}
		}
	}*/
	
}

/*
 * 		OTHER SAMPLE CODE
 * 

		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
		if(anim.layerCount ==2)		
			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation

		// LOOK AT ENEMY
		
		// if we hold Alt..
		if(Input.GetButton("Fire2"))
		{
			// ...set a position to look at with the head, and use Lerp to smooth the look weight from animation to IK (see line 54)
			anim.SetLookAtPosition(enemy.position);
			lookWeight = Mathf.Lerp(lookWeight,1f,Time.deltaTime*lookSmoother);
		}
		// else, return to using animation for the head by lerping back to 0 for look at weight
		else
		{
			lookWeight = Mathf.Lerp(lookWeight,0f,Time.deltaTime*lookSmoother);
		}


		// STANDARD JUMPING
		
		// if we are currently in a state called Locomotion (see line 25), then allow Jump input (Space) to set the Jump bool parameter in the Animator to true
		if (currentBaseState.nameHash == locoState)
		{
			if(Input.GetButtonDown("Jump"))
			{
				anim.SetBool("Jump", true);
			}
		}
		
		// if we are in the jumping state... 
		else if(currentBaseState.nameHash == jumpState)
		{
			//  ..and not still in transition..
			if(!anim.IsInTransition(0))
			{
				if(useCurves)
					// ..set the collider height to a float curve in the clip called ColliderHeight
					col.height = anim.GetFloat("ColliderHeight");
				
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool("Jump", false);
			}
			
			// Raycast down from the center of the character.. 
			Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit();
			
			if (Physics.Raycast(ray, out hitInfo))
			{
				// ..if distance to the ground is more than 1.75, use Match Target
				if (hitInfo.distance > 1.75f)
				{
					
					// MatchTarget allows us to take over animation and smoothly transition our character towards a location - the hit point from the ray.
					// Here we're telling the Root of the character to only be influenced on the Y axis (MatchTargetWeightMask) and only occur between 0.35 and 0.5
					// of the timeline of our animation clip
					anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
				}
			}
		}
		
		
		// JUMP DOWN AND ROLL 
		
		// if we are jumping down, set our Collider's Y position to the float curve from the animation clip - 
		// this is a slight lowering so that the collider hits the floor as the character extends his legs
		else if (currentBaseState.nameHash == jumpDownState)
		{
			col.center = new Vector3(0, anim.GetFloat("ColliderY"), 0);
		}
		
		// if we are falling, set our Grounded boolean to true when our character's root 
		// position is less that 0.6, this allows us to transition from fall into roll and run
		// we then set the Collider's Height equal to the float curve from the animation clip
		else if (currentBaseState.nameHash == fallState)
		{
			col.height = anim.GetFloat("ColliderHeight");
		}
		
		// if we are in the roll state and not in transition, set Collider Height to the float curve from the animation clip 
		// this ensures we are in a short spherical capsule height during the roll, so we can smash through the lower
		// boxes, and then extends the collider as we come out of the roll
		// we also moderate the Y position of the collider using another of these curves on line 128
		else if (currentBaseState.nameHash == rollState)
		{
			if(!anim.IsInTransition(0))
			{
				if(useCurves)
					col.height = anim.GetFloat("ColliderHeight");
				
				col.center = new Vector3(0, anim.GetFloat("ColliderY"), 0);
				
			}
		}
		// IDLE
		// check if we are at idle, if so, let us Wave!
		else if (currentBaseState.nameHash == idleState)
		{
			if(Input.GetButtonUp("Jump"))
			{
				anim.SetBool("Wave", true);
			}
		}
		// if we enter the waving state, reset the bool to let us wave again in future
		if(layer2CurrentState.nameHash == waveState)
		{
			anim.SetBool("Wave", false);
		}
	*/
