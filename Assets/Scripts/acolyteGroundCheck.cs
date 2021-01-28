using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acolyteGroundCheck : MonoBehaviour {
	public acolyteBehavior AB;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerStay2D(Collider2D col2D) {
		if (AB == null) {
			Debug.LogError($"{this.name}: AB is null!");
			return;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			AB.isGrounded = true;
		}
	}

	void OnTriggerExit2D(Collider2D col2D) {
		if (AB == null) {
			Debug.LogError($"{this.name}: AB is null!");
			return;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			AB.isGrounded = false;
		}

	}
}
