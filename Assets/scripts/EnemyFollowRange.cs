using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowRange : MonoBehaviour {
	public SightRange sightRange;

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Enemy"){
			print("entered");
			SightRange s = collider.GetComponent<ZombieAI>().vision;
			if(s.target == null)
				s.target = sightRange.target;

		}
	}
}
