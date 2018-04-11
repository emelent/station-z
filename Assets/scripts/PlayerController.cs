using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Range(1, 3)]
	public float recoilFactor = 0.5f;
	public int playerNumber = 1;
	public float rotateStep = 4f;
	public Sprite[] playerSprites;

	[SerializeField]
	private Weapon weapon;
	private GameCharacter character;
	private Rigidbody2D rb;
	private Transform forwardPoint;	

	void Awake(){
		character = GetComponent<GameCharacter>();
		rb = GetComponent<Rigidbody2D>();
		forwardPoint = transform.Find("ForwardPoint");
		SetPlayerNumber(playerNumber);
	}

	void Update(){
		handleInput();
	}

	void  handleInput(){
		string pk = "Player" + playerNumber.ToString() + "_";

		// move player	
		if(character.allowedToMove){
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

		// shoot
		if(weapon){
			if(weapon.attackRate > 0f){
				if(Input.GetButton(pk + "Fire") && weapon.canAttack){
					if(!weapon.attackCoolDown.refilling)
						recoil();
					weapon.Attack(character.name);
				}
			}else if(Input.GetButtonDown(pk + "Fire")){
					if(!weapon.attackCoolDown.refilling)
						recoil();
					weapon.Attack(character.name);
			}
		}
	}

	void recoil(){
		Vector2 direction = (transform.position - forwardPoint.position).normalized;
		character.KnockBack(direction * weapon.attackKnockBack * recoilFactor);
	}

	public void SetPlayerNumber(int num){
		if(playerNumber < 0 || playerNumber > playerSprites.Length)
			return;

		playerNumber = num;
		character.name = "player" + playerNumber;
		character.origSprite = playerSprites[num - 1];
		GetComponent<SpriteRenderer>().sprite = playerSprites[num - 1];
	}

	public void EquipItem(Transform itemPrefab){
		if(!itemPrefab) return;
		if(itemPrefab.tag == "Weapon"){
			GM.instance.CreatePlayerWeapon(this, itemPrefab);
		}
	}

	public void EquipWeapon(Weapon _weapon){
		if(weapon){
			Destroy(weapon.gameObject);
		}
		weapon =  _weapon;
	}

	public void Reset(){
		character.Reset();
	}
}
