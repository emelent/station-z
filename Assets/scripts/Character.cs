using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: MonoBehaviour{
	
	public string characterName;
	public string killer;
	public string dieSound;
	public float healthEffectDuration =  0.1f;
	public Sprite healthChangeSprite;
	public float knockBackDuration = 0.1f;

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
	public bool allowedToMove = false;

	[HideInInspector]
	public HealthSystem healthSystem;

	SpriteRenderer spriteRenderer;
	Sprite origSprite;
	Rigidbody2D rb;
	float knockBackTime = 0f;
	[SerializeField]

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		healthSystem = GetComponent<HealthSystem>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		origSprite = spriteRenderer.sprite;
	}

	IEnumerator damageEffect(){
		spriteRenderer.sprite = healthChangeSprite;
		spriteRenderer.color = hurtColor;
		//show hurt particles
		if(hurtParticles)
			hurtParticles.Play();
		
		yield return new WaitForSeconds(healthEffectDuration);
		clearHealthEffects();
		
	}

	IEnumerator healEffect(){
		spriteRenderer.sprite = healthChangeSprite;
		spriteRenderer.color = healColor;

		//show heal particles
		if(healParticles)
			healParticles.Play();
		
		yield return new WaitForSeconds(healthEffectDuration);
		clearHealthEffects();
	}

	void knockBackProcess(){
		if(Time.time > knockBackTime){
			allowedToMove = true;
			rb.velocity = Vector2.zero;
		}
	}

	IEnumerator knockBack(Vector2 force){
		rb.velocity = force;
		allowedToMove = false;
		yield return new WaitForSeconds(knockBackDuration);
		allowedToMove = true;
		rb.velocity = Vector2.zero;
	}

	void clearHealthEffects(){
		spriteRenderer.sprite = origSprite;
		spriteRenderer.color = Color.white;
	}

	public void KnockBack(Vector2 force){
		if(allowedToMove)
			StartCoroutine(knockBack(force));
	}

	public void Hurt(float amount, string _killer="environment"){
		GameMaster.PlayAudio(hurtSound);
		healthSystem.Damage(amount);

		if(healthSystem.GetHealth() == 0f){
			killer=_killer;
			StopAllCoroutines();
			clearHealthEffects();
		}else{
			StartCoroutine(damageEffect());
		}
	}

	public void Heal(float amount){
		GameMaster.PlayAudio(healSound);
		healthSystem.Heal(amount);

		StartCoroutine(healEffect());
	}

	public virtual void Reset(){
		healthSystem.Reset();
		clearHealthEffects();
		rb.Sleep();
		allowedToMove = true;
	}
}