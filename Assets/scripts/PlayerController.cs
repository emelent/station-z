using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Range(1, 3)]
	public int playerNumber = 1;
	public float rotateStep = 4f;
	public Sprite[] playerSprites;

	Character character;
	Rigidbody2D rb;
	Transform forwardPoint;	

	void Awake(){
		character = GetComponent<Character>();
		rb = GetComponent<Rigidbody2D>();
		forwardPoint = transform.Find("ForwardPoint");
		SetPlayerNumber(playerNumber);
	}

	void Update(){
		handleInput();
	}

	void  handleInput(){
		string pk = "Player" + playerNumber.ToString() + "_";
		
		if(character.allowedToMove){
			print("moving");
			float motion = Input.GetAxisRaw(pk + "Motion");		
			Vector2 dir = (transform.position - forwardPoint.position).normalized;
			rb.velocity = dir * motion * character.movementSpeed * Time.deltaTime;
		}

		// turn player
		if(Input.GetButton(pk + "TurnRight")){
			transform.Rotate(0f, 0f, -rotateStep);
		}else if (Input.GetButton(pk + "TurnLeft")){
			transform.Rotate(0f, 0f, rotateStep);
		}
	}

	public void SetPlayerNumber(int num){
		if(playerNumber < 0 || playerNumber > playerSprites.Length)
			return;

		playerNumber = num;
		character.origSprite = playerSprites[num -1];
		GetComponent<SpriteRenderer>().sprite = playerSprites[num -1];
	}
}
