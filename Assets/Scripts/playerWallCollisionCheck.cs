using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerWallCollisionCheck : MonoBehaviour {
	public PlayerInputScript PIS;
	Collider2D wallCollider;
	public Transform playerTransform;
	public float ledgeVaultHeight;
	public float ledgeVaultDistance;
	public Rigidbody2D RB2D;
	// Use this for initialization
	void Start () {
		wallCollider = GetComponent<Collider2D> ();
	}

	void initiateWallClimb(Collider2D col2D) {
		bool isFacingWall = playerTransform.localScale.x * (col2D.transform.position.x - playerTransform.position.x) > 0;
		if (PIS.hasReleasedWall) {
			PIS.wallStickDurationCurrent = PIS.wallStickDurationMax;
			PIS.hasReleasedWall = false;
		}
		RB2D.velocity = new Vector2 (RB2D.velocity.x, 0f);
		if (!isFacingWall && !PIS.isWallClimbing) {
			playerTransform.localScale = new Vector3(playerTransform.transform.localScale.x * -1, playerTransform.localScale.y, playerTransform.localScale.z);
		}
		PIS.isWallClimbing = true;
	}

	void OnTriggerExit2D(Collider2D col2D)
	{
		bool isWallHeight = col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y;
		bool isWall = col2D.gameObject.layer == LayerMask.NameToLayer ("Wall");
		bool isLedge = col2D.gameObject.layer == LayerMask.NameToLayer ("Ledge");
		if (isWallHeight && isWall && !PIS.isGrounded) {
			PIS.isWallClimbing = false;
			PIS.hasReleasedWall = true;
		}
		if (isLedge) {
			PIS.isLedgeVaulting = false;
			PIS.hasReleasedWall = true;
		}
	}

	void OnTriggerEnter2D(Collider2D col2D){
		// bool isWallHeight = col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y;
		// bool isWall = col2D.gameObject.layer == LayerMask.NameToLayer ("Wall");
		// if (isWallHeight && isWall && !PIS.isGrounded && Mathf.Abs(PIS.controllerAxisX) > 0.5f) {
		// }
	}

	void OnTriggerStay2D(Collider2D col2D)
	{
		if (playerTransform == null) {
			Debug.LogError("playerTransform is null!");
			return;
		}
		bool isWallHeight = col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y;
		bool isWall = col2D.gameObject.layer == LayerMask.NameToLayer ("Wall");
		bool isLedge = col2D.gameObject.layer == LayerMask.NameToLayer ("Ledge");
		if (isWallHeight && isWall && !PIS.isGrounded && !PIS.isLedgeVaulting && PIS.yVelo <= 0 && Mathf.Abs(PIS.controllerAxisX) > 0.5f) {
			initiateWallClimb(col2D);
		}
		if (isWallHeight && isLedge) {
			PIS.isLedgeVaulting = true;
			playerTransform.position = new Vector2 (playerTransform.position.x + (playerTransform.localScale.x * ledgeVaultDistance), col2D.transform.parent.position.y + col2D.GetComponentInParent<Collider2D> ().bounds.extents.y + ledgeVaultHeight);
		}	
	}
}
