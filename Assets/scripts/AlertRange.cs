using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertRange : MonoBehaviour {
	public SightRange sightRange;

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Enemy"){
			SightRange s = collider.GetComponent<ZombieAI>().sightRange;
			if(s.target == null)
				s.target = sightRange.target;
		}
	}
}
