using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float maxHealth= 100f; 
	public float drainAmount = 0.1f;
	public float drainRate = 0f;
	public Transform HealthBar;

	private float mDrainTime = 0f;
	[SerializeField]
	private float mHealth;

	private float mOrigHealthBarScale;

	void Awake(){
		mHealth = maxHealth;
		if(HealthBar){
			mOrigHealthBarScale = HealthBar.localScale.x;
		}
	}

	
	// Update is called once per frame
	void Update () {
		if(Time.time > mDrainTime && drainRate != 0f){
			mDrainTime = Time.time + 1/drainRate;
			Damage(drainAmount);
		}

		if(HealthBar){	
			HealthBar.transform.localScale = new Vector3(
				mHealth/maxHealth * mOrigHealthBarScale,
				HealthBar.transform.localScale.y,
				HealthBar.transform.localScale.z
			);
		}
	}

	public void Reset(){
		mHealth = maxHealth;
	}
	
	public void Damage(float amount){
		mHealth = Mathf.Clamp(mHealth - amount, 0, maxHealth);

		//kill at 0HP
		if(mHealth == 0f){
			GameMaster.Kill(gameObject);
		}
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
