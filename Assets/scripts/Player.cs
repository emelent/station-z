using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Player : MonoBehaviour {

	[Range(1, 4)]
	public int playerNumber = 1;
	public bool canMove = true;
	public float movementSpeed = 200f;
	public float rotateStep = 1f;

	float motion = 0f;
	Vector2 velocity = Vector2.zero;
	Rigidbody2D rb;
	Transform forwardPoint;	
	// Animator anim;


	void Awake(){
		// anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		forwardPoint = transform.Find("ForwardPoint");
	}

	void Update(){
		handleInput();
	}

	void FixedUpdate(){
		handleMovement();
	}


	void  handleInput(){
		string pk = "Player" + playerNumber.ToString() + "_";
		motion = Input.GetAxisRaw(pk + "Motion");		

		// turn player
		if(Input.GetButton(pk + "TurnRight")){
			transform.Rotate(0f, 0f, -rotateStep);
		}else if (Input.GetButton(pk + "TurnLeft")){
			transform.Rotate(0f, 0f, rotateStep);
		}
	}

	void handleMovement(){
		if(canMove){
			Vector2 direction = (transform.position - forwardPoint.position).normalized;
			velocity = direction * motion * movementSpeed * Time.deltaTime;
		}

		rb.velocity = velocity;
	}

	public Vector2 GetVelocity(){
		return velocity;
	}

	public void SetVelocity(Vector2 v){
		velocity = v;
	}

	public void Reset(){
		velocity = Vector2.zero;
		canMove = true;
	}
}
