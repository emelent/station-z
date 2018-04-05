using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {


	void Awake () {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		if(sr){
			sr.enabled = false;
		}	
	}
}
