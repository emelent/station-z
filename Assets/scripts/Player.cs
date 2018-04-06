using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Player : MonoBehaviour {

	[Range(1, 3)]
	public int playerNumber = 1;
	public bool canMove = true;
	public float movementSpeed = 200f;
	public float rotateStep = 1f;
	public Weapon weapon;
	public Color damageColor = Color.red;
	public ParticleSystem bloodSplatter;

	float motion = 0f;
	Vector2 velocity = Vector2.zero;
	Rigidbody2D rb;
	Transform forwardPoint;	
	Health health;
	SpriteRenderer spriteRenderer;



	void Awake(){
		// anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		health = GetComponent<Health>();
		spriteRenderer = GetComponent<SpriteRenderer>();
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

		// shoot
		if(weapon){
			if(weapon.fireRate > 0f){
				if(Input.GetButton(pk + "Fire") && weapon.canShoot){
					weapon.Shoot(transform.position);
				}
			}else if(Input.GetButtonDown(pk + "Fire")){
				weapon.Shoot(transform.position);
			}
		}
	}

	void handleMovement(){
		if(canMove){
			Vector2 direction = (transform.position - forwardPoint.position).normalized;
			velocity = direction * motion * movementSpeed * Time.deltaTime;
		}

		rb.velocity = velocity;
	}

	IEnumerator showDamage(){
		spriteRenderer.color = damageColor;
		
		//blood particles
		if(bloodSplatter){
			bloodSplatter.Play();
		}
		
		yield return new WaitForSeconds(0.5f);
		spriteRenderer.color = Color.white;
	}

	public void EquipWeapon(Weapon _weapon){
		if(weapon){
			Destroy(weapon.gameObject);
		}
		weapon = _weapon;
	}

	public Vector2 GetVelocity(){
		return velocity;
	}

	public void SetVelocity(Vector2 v){
		velocity = v;
	}

	public Health GetHealth(){
		return health;
	}

	public void Damage(float amount){
		health.Damage(amount);
		GameMaster.PlayAudio(
			"PlayerHurt" + (int) Random.Range(1, 3)
		);
		StartCoroutine(showDamage());
	}

	public void Reset(){
		health.Reset();
		velocity = Vector2.zero;
		canMove = true;
	}
}
