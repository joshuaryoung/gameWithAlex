﻿/*
Bugs

Enemy punches when in reel state



*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchPlayerDetection : MonoBehaviour {
	public WitchBehavior WB;
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
  	public witchAIBlockerScript WAIBS;

	// Use this for initialization
	void Start () {
		thisColl2D = gameObject.GetComponent<Collider2D>();
		if (WB == null) {
			WB = GetComponentInParent<WitchBehavior>();
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
		if (WAIBS == null) {
			Debug.LogError("AIBlockerScript is null!");
			return;
		}
		if (WB == null) {
			Debug.LogError("WB is null!");
			return;
		}
		if (WB.isDead || WB.isDying || !WB.isNotInAnimation) {
			return;
		}
		playerDetected = col2D.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox");
		if(playerDetected) {
			currentColLayer = col2D.gameObject.layer;
			currentCol = col2D;
			currentColPos = col2D.gameObject.transform.position;
			currentColDistance = upperHurtboxColBounds.x - currentColPos.x;
			distanceFlipCalc = currentColDistance * thisGameObjLocalScale.x * -1;
			
			if(distanceFlipCalc < 0 && !WAIBS.isCollidingWithAIBlocker && WB.flipCoolDown <= 0) {
				gameObject.SendMessageUpwards("flip");
			}
		}
	}
}