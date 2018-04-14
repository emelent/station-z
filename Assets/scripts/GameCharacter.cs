using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
	Base class for interactable Game Characters.
	Players and Enemies alike. All the goodies a Game Character
	needs.
 */
[RequireComponent(typeof(HealthSystem))]
public class GameCharacter: MonoBehaviour{
	
	public string characterName;
	public string dieSound;
	public Sprite healthChangeSprite;
	public float knockBackDuration = 0.1f;
	public float movementSpeed = 200f;
	public bool allowedToMove = true;
	public float healthEffectDuration =  0.1f;

	[Header("Hurt Effect")]
	public string hurtSound;
	public Color hurtColor  = Color.red;

	[Header("Heal Effect")]
	public string healSound;
	public Color healColor = Color.cyan;
	
	[HideInInspector]
	public string killer;
	[HideInInspector]
	public bool inWater;
	

	[HideInInspector]
	public HealthSystem healthSystem;
	[HideInInspector]
	public Sprite origSprite;

	SpriteRenderer spriteRenderer;
	Rigidbody2D rb;
	ParticleSystem hurtParticles;
	ParticleSystem healParticles;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		healthSystem = GetComponent<HealthSystem>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		origSprite = spriteRenderer.sprite;


		Transform t = transform.Find("HurtParticles");
		if(t)
			hurtParticles = t.GetComponent<ParticleSystem>();
		t = transform.Find("HealParticles");
		if(t)
			healParticles = t.GetComponent<ParticleSystem>();
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

	IEnumerator knockBack(Vector2 force){
		rb.velocity = force;
		allowedToMove = false;
		yield return new WaitForSeconds(knockBackDuration);
		allowedToMove = true;
		rb.velocity = Vector2.zero;
	}

	IEnumerator die(){
		yield return new WaitForEndOfFrame();
		GM.PlayAudio(dieSound);
		StopAllCoroutines();
		GM.KillCharacter(this);
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
		GM.PlayAudio(hurtSound);
		healthSystem.Damage(amount);

		if(healthSystem.GetHealth() == 0f){
			killer=_killer;
			StartCoroutine(die());
		}else{
			StartCoroutine(damageEffect());
		}
	}

	public void Heal(float amount){
		healthSystem.Heal(amount);

		StartCoroutine(healEffect());
	}

	public virtual void Reset(){
		healthSystem.Reset();
		clearHealthEffects();
		rb.Sleep();
		allowedToMove = true;
		inWater = false;
	}
}