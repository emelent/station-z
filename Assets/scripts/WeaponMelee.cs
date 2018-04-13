using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : Weapon {
	
	string attacker;
	void OnTriggerStay2D(Collider2D collider){
		if(!isOn) return;
		if(collider.tag == "Enemy" || collider.tag == "Player"){
			GameCharacter character = collider.GetComponent<GameCharacter>();
			character.Hurt(attackDamage, attacker);
			Vector2 dir = (firePoint.position - transform.position).normalized;
			character.KnockBack(dir * attackKnockBack);
		}
	}

	protected override void attack(string _attacker){
		if(!isOn){
			isOn = true;
			attacker = _attacker;
		}
		GM.PlayAudio(attackSound);
	}
}
