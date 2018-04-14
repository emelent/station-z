using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Weapon component
	Base class for all weapons.

			    Weapon
			      |
		---------------------
		|			        |
	MeleeWeapon	      RangedWeapon

*/
public class Weapon: MonoBehaviour{


	// Prevents  spamming of attack by adding a sort of cost for an
	// attack, and a sort of cool down system. So after a certain
	// amount of attacks, a weapon needs to cool down or "reload"
	// or whatever.
	[System.Serializable]
	public class AttackLimit{ 
		public float maxCoolDown = 100f;
		public float coolDown = 100f;
		public bool refilling = false;


		public void Drop(float amount){
			coolDown = Mathf.Max(0f, coolDown - amount);
			refilling = (coolDown == 0f);
		}
		
		public void Refill(float amount){
			if(!refilling) return;
			coolDown = Mathf.Min(maxCoolDown, coolDown + amount);
			refilling = (coolDown != maxCoolDown);
		}
	}


	public LayerMask whatToHit;
	public float attackKnockBack = 2f;
	public float attackRate = 0f;
	public float attackDamage = 10f;
	public string attackSound;
	

	[Header("Attack CoolDown")]
	[SerializeField]
	public AttackLimit attackCoolDown = new AttackLimit();
	public float attackCost = 20f;
	public float attackRefill = 10f;
	public string attackNotReadySound = "NoAmmo";

	[HideInInspector]
	public bool canAttack = true;
	[HideInInspector]
	public bool isOn = false;
	public bool hasRecoil = true;

	protected Transform firePoint;

	private static float maxCoolDownScaleX = -1f;
	private float attackTime;
	private float playNotReadySoundTime = 0f;
	private float notReadySoundRate = 2f;
	private Transform CoolDownGauge;
	
	void Awake(){
		firePoint = transform.Find("FirePoint");
	}

	void Update(){
		if(attackRate > 0f)
			canAttack = Time.time > attackTime;
		
		attackCoolDown.Refill(attackRefill * Time.deltaTime);
		if(CoolDownGauge && maxCoolDownScaleX != -1f){
			float x = (attackCoolDown.coolDown == 0)? 0: attackCoolDown.coolDown / attackCoolDown.maxCoolDown * maxCoolDownScaleX;
			CoolDownGauge.localScale = new Vector3(
				x,
				CoolDownGauge.localScale.y,
				CoolDownGauge.localScale.z
			);
		}
	}

	public void LinkCoolDownBar(Transform bar){
		if(!bar) return;
		if(maxCoolDownScaleX == -1f)
			maxCoolDownScaleX = bar.localScale.x;

		CoolDownGauge = bar;
	}

	protected virtual void attack(string attacker){
		print("Attacking");
	}

	public void Attack(string attacker){
		if(attackCoolDown.refilling){
			if(Time.time > playNotReadySoundTime){
				GM.PlayAudio(attackNotReadySound);
				playNotReadySoundTime = Time.time + 1/notReadySoundRate;
			}
			return;
		}
		if(attackRate != 0f)
			attackTime = Time.time + 1/attackRate;
		attackCoolDown.Drop(attackCost);
		attack(attacker);
	}
}