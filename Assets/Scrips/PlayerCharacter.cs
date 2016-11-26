using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerCharacter : MonoBehaviour
{
	public float speed = 1.0f;
	public string axisName = "Horizontal";
	private Animator anim;
	public string jumpButton = "Fire1";
	public float jumpPower = 10.0f;
	public float minJumpDelay = 0.5f;
	public Transform[] groundChecks;
	
	private float jumpTime = 0.0f;
	private Transform currentPlatform = null;
	private Vector3 lastPlatformPosition = Vector3.zero;
	private Vector3 currentPlatformDelta = Vector3.zero;
	
	
	// Use this for initialization
	void Start ()
	{
		anim = gameObject.GetComponent<Animator>();
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		//Left and right movement
		anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis(axisName)));
		if(Input.GetAxis(axisName) < 0)
		{
			Vector3 newScale = transform.localScale;
			newScale.x = -1.0f;
			transform.localScale = newScale;
			Debug.Log("Move to left");
		}
		else if(Input.GetAxis(axisName) > 0)
		{
			Vector3 newScale = transform.localScale;
			newScale.x = 1.0f;
			transform.localScale = newScale;
			Debug.Log ("Move to Right");
		}
		transform.position += transform.right*Input.GetAxis(axisName)*speed*Time.deltaTime;
		
		//Jump logic
		bool grounded = false;
		foreach(Transform groundCheck in groundChecks)
		{
			grounded |= Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		}
		anim.SetBool("Grounded", grounded);
		if(jumpTime > 0)
		{
			jumpTime -= Time.deltaTime;
		}
		if(Input.GetButton("jumpButton")  && anim.GetBool("Grounded") )
			
		{
			anim.SetBool("Jump",true);
			GetComponent<Rigidbody2D>().AddForce(transform.up*jumpPower);
			jumpTime = minJumpDelay;
		}
		if(anim.GetBool("Grounded") && jumpTime <= 0)
		{
			anim.SetBool("Jump",false);
		}
		//Moving platform logic
		//Check what platform we are on
		List<Transform> platforms = new List<Transform>();
		bool onSamePlatform = false;
		foreach(Transform groundCheck in groundChecks)
		{
			RaycastHit2D hit = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
			if(hit.transform != null)
			{
				platforms.Add(hit.transform);
				if(currentPlatform == hit.transform)
				{
					onSamePlatform = true;
				}
			}
		}
		
		if(!onSamePlatform)
		{
			foreach(Transform platform in platforms)
			{
				currentPlatform = platform;
				lastPlatformPosition = currentPlatform.position;
			}
		}
	}
	
	void LateUpdate()
	{
		if(currentPlatform != null)
		{
			//Determine how far platform has moved
			currentPlatformDelta = currentPlatform.position - lastPlatformPosition;
			
			lastPlatformPosition = currentPlatform.position;
		}
		if(currentPlatform != null)
		{
			//Move with the platform
			transform.position += currentPlatformDelta;
		}
	}   
}