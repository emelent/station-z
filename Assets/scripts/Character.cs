using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: MonoBehaviour{
	
	public string characterName;
	public string killer;
	public string dieSound;
	public float healthEffectDuration =  0.5f;
	public Sprite healthChangeSprite;
	

	[Header("Hurt Effect")]
	public string hurtSound;
	public Color hurtColor  = Color.red;
	public ParticleSystem hurtParticles;

	[Header("Heal Effect")]
	public string healSound;
	public Color healColor = Color.cyan;
	public ParticleSystem healParticles;
	

	[HideInInspector]
	public bool inWater;
	
	[HideInInspector]
	public bool canMove = false;

	[HideInInspector]
	public HealthSystem healthSystem;

	SpriteRenderer spriteRenderer;
	Sprite origSprite;
	Rigidbody2D rb;
	float knockBackTime = 0f;
	[SerializeField]
	float knockBackDuration = 0.5f;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		healthSystem = GetComponent<HealthSystem>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		origSprite = spriteRenderer.sprite;
	}

	IEnumerator showDamage(){
		spriteRenderer.sprite = healthChangeSprite;
		spriteRenderer.color = hurtColor;
		//show hurt particles
		if(hurtParticles)
			hurtParticles.Play();
		
		yield return new WaitForSeconds(healthEffectDuration);
		spriteRenderer.sprite = origSprite;
		spriteRenderer.color = Color.white;
		
	}

	IEnumerator showHeal(){
		spriteRenderer.sprite = healthChangeSprite;
		spriteRenderer.color = healColor;

		//show heal particles
		if(healParticles)
			healParticles.Play();
		
		yield return new WaitForSeconds(healthEffectDuration);
		spriteRenderer.sprite = origSprite;
		spriteRenderer.color = Color.white;
	}

	void knockBackProcess(){
		if(Time.time > knockBackTime){
			canMove = true;
			rb.velocity = Vector2.zero;
		}
	}

	public void KnockBack(Vector2 force){
		rb.Sleep();
		rb.velocity = force;
		canMove = false;
		knockBackTime = Time.time + knockBackDuration;
	}

	public void Hurt(float amount, string _killer="environment"){
		GameMaster.PlayAudio(hurtSound);
		healthSystem.Damage(amount);

		if(healthSystem.GetHealth() == 0f){
			killer=_killer;
			StopAllCoroutines();
		}else{
			StartCoroutine(showDamage());
		}
	}

	public void Heal(float amount){
		GameMaster.PlayAudio(healSound);
	}
}