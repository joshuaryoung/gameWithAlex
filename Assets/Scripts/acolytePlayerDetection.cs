﻿/*
Bugs

Enemy punches when in reel state



*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acolytePlayerDetection : MonoBehaviour {
	public acolyteBehavior AB;
	public Animator anim;
	public bool playerDetected;
	public LayerMask playerLayer;
	public Collider2D currentCol;
	public int currentColLayer;
	public Vector3 currentColPos;
	public float currentColDistance;
	public Collider2D thisColl2D;
	public Vector3 thisGamObjPos;
	public Vector3 thisGameObjLocalScale;
	public Collider2D upperHurtboxCol;
	public Vector3 upperHurtboxColBounds;
	public GameObject parentGameObj;
	public Transform parentGameObjTrans;
	public float distanceFlipCalc;
  	public AIBlockerScript AIBS;

	// Use this for initialization
	void Start () {
		thisColl2D = gameObject.GetComponent<Collider2D>();
		if (AB == null) {
			AB = GetComponentInParent<acolyteBehavior>();
		}
	}

	void Update() {
		thisGamObjPos = gameObject.transform.position;
		upperHurtboxColBounds = upperHurtboxCol.bounds.center;
		thisGameObjLocalScale = parentGameObjTrans.localScale;
	}

	void OnTriggerEnter2D(Collider2D col2D) {
		OnTriggerCommonRoutines(col2D);
	}

	void OnTriggerStay2D(Collider2D col2D) {
		OnTriggerCommonRoutines(col2D);
	}

	void OnTriggerExit2D(Collider2D col2D) {
		playerDetected = false;
	}

	void OnTriggerCommonRoutines(Collider2D col2D) {
		if (AIBS == null) {
			Debug.LogError("AIBlockerScript is null!");
			return;
		}
		if (AB == null) {
			Debug.LogError("AB is null!");
			return;
		}
		if (AB.isDead || AB.isDying || !AB.isNotInAnimation) {
			return;
		}
		playerDetected = col2D.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox");
		if(playerDetected) {
			currentColLayer = col2D.gameObject.layer;
			currentCol = col2D;
			currentColPos = col2D.gameObject.transform.position;
			currentColDistance = upperHurtboxColBounds.x - currentColPos.x;
			distanceFlipCalc = currentColDistance * thisGameObjLocalScale.x * -1;
			
			if(distanceFlipCalc < 0 && !AIBS.isCollidingWithAIBlocker && AB.flipCoolDown <= 0) {
				gameObject.SendMessageUpwards("flip");
			}
		}
	}
}