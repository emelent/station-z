
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour {

	public enum State{Roaming, Chasing, Attacking, Frozen};

	public State aiState = State.Roaming;
	public float chaseFactor = 1.2f;
	public float attackDamage = 10f;
	public float attackRate = 2f;


	[HideInInspector]
	public SightRange sightRange;

	protected AttackRange attackRange;
	protected Rigidbody2D rb;
	protected GameCharacter character;
	protected Transform forwardPoint;

	void Awake(){
		character = GetComponent<GameCharacter>();
		rb = GetComponent<Rigidbody2D>();
		forwardPoint = transform.Find("ForwardPoint");
		sightRange = transform.Find("SightRange").GetComponent<SightRange>();
		attackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
	}

	public void SetTarget(Transform target){
		sightRange.target =  target;
	}

}
