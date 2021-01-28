using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bigboyGroundCheck : MonoBehaviour {
  public BigBoyBehavior BBB;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerStay2D(Collider2D col2D) {
		if (BBB == null) {
			Debug.LogError($"{this.name}: BBB is null!");
			return;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			BBB.isGrounded = true;
		}
	}

	void OnTriggerExit2D(Collider2D col2D) {
		if (BBB == null) {
			Debug.LogError($"{this.name}: BBB is null!");
			return;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			BBB.isGrounded = false;
		}

	}
}
