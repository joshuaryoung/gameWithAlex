using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class witchGroundCheck : MonoBehaviour {
  public WitchBehavior WB;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerStay2D(Collider2D col2D) {
		if (WB == null) {
			Debug.LogError($"{this.name}: WB is null!");
			return;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			WB.isGrounded = true;
		}
	}

	void OnTriggerExit2D(Collider2D col2D) {
		if (WB == null) {
			Debug.LogError($"{this.name}: WB is null!");
			return;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			WB.isGrounded = false;
		}

	}
}
