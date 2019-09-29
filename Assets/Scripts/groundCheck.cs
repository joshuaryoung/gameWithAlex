using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour {
	public PlayerInputScript PIS;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerStay2D(Collider2D col2D)
	{
		if (/*(col2D.collider.bounds.extents.y + col2D.gameObject.transform.position.y <= transform.position.y - lowerHitBox.bounds.extents.y) && */col2D.gameObject.layer == LayerMask.NameToLayer ("GroundLayer")) {
			PIS.isGrounded = true;
			PIS.isWallClimbing = false;
		}
	}
}
