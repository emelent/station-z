using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float maxHealth= 100f; 
	public float drainAmount = 0.1f;
	public float drainRate = 0f;

	private float mDrainTime = 0f;
	private float mHealth;

	void Awake(){
		mHealth = maxHealth;
	}

	
	// Update is called once per frame
	void Update () {
		if(drainRate == 0f) return;
		
		if(Time.time > mDrainTime){
			mDrainTime = Time.time + 1/drainRate;
			Damage(drainAmount);
		}
	}

	public void Damage(float amount){
		mHealth = Mathf.Clamp(mHealth - amount, 0, maxHealth);
	}

	public void Heal(float amount){
		mHealth = Mathf.Clamp(mHealth + amount, 0, maxHealth);
	}

	public float GetHealth(){
		return mHealth;
	}

	public float GetMaxHealth(){
		return maxHealth;
	}

	public void SetMaxHealth(float amount){
		float percent = mHealth / maxHealth;
		maxHealth = Mathf.Max(amount, 1f);
		mHealth = percent * maxHealth;
	}
}
