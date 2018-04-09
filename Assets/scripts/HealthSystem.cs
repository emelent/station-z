using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {

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
		LinkHealthBar(HealthBar);
	}

	
	// Update is called once per frame
	void Update () {
		if(Time.time > mDrainTime && drainRate != 0f){
			mDrainTime = Time.time + 1/drainRate;
			Damage(drainAmount);
		}

		if(HealthBar){	
			float x = (mHealth == 0f)? 0f:mHealth/maxHealth * mOrigHealthBarScale;
			HealthBar.transform.localScale = new Vector3(
				x,
				HealthBar.transform.localScale.y,
				HealthBar.transform.localScale.z
			);
		}
	}

	public void LinkHealthBar(Transform bar){
		if(!bar) return;

		HealthBar = bar;
		mOrigHealthBarScale = HealthBar.localScale.x;
	}

	public void Reset(){
		mHealth = maxHealth;
	}
	
	public void Damage(float amount){
		mHealth = Mathf.Max(mHealth - amount, 0f);
	}

	public void Heal(float amount){
		mHealth = Mathf.Min(mHealth + amount, maxHealth);
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
