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

	void OnTriggerExit2D(Collider2D col2D)
	{
		if ((col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y) && col2D.gameObject.layer == LayerMask.NameToLayer ("Wall") && !PIS.isGrounded) {
			PIS.isWallClimbing = false;
		}
		if (col2D.gameObject.layer == LayerMask.NameToLayer ("Ledge")) {
			PIS.isLedgeVaulting = false;
		}
	}

	void OnTriggerEnter2D(Collider2D col2D){
		if ((col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y) && col2D.gameObject.layer == LayerMask.NameToLayer ("Wall") && !PIS.isGrounded) {
			PIS.wallStickDurationCurrent = PIS.wallStickDurationMax;
			RB2D.velocity = new Vector2 (RB2D.velocity.x, 0f);
		}
	}

	void OnTriggerStay2D(Collider2D col2D)
	{
		if ((col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y) && col2D.gameObject.layer == LayerMask.NameToLayer ("Wall") && !PIS.isGrounded && !PIS.isLedgeVaulting && PIS.yVelo <=0 && PIS.controllerAxisX * (col2D.transform.position.x - playerTransform.position.x) > 0) {
			PIS.isWallClimbing = true;
		}
		if ((col2D.bounds.extents.y + col2D.gameObject.transform.position.y > wallCollider.transform.position.y - wallCollider.bounds.extents.y) && col2D.gameObject.layer == LayerMask.NameToLayer ("Ledge")) {
			PIS.isLedgeVaulting = true;
			playerTransform.position = new Vector2 (playerTransform.position.x + (playerTransform.localScale.x * ledgeVaultDistance), col2D.transform.parent.position.y + col2D.GetComponentInParent<Collider2D> ().bounds.extents.y + ledgeVaultHeight);
		}	
	}
}
