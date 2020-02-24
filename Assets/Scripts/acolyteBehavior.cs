using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acolyteBehavior : MonoBehaviour {
	//Raycast class;
	Collider2D hit;
	Rigidbody2D RB2D;

	public GameObject playerObject;
	PlayerInputScript PIS;
	public int startHealth;
	public int currentHealth;
	public int invincibilityCooldownPeriod;
	public int invincibilityCooldownCurrent;
	public int invincibilityCooldownFlash = 0;
	public float enemyHorizontalSpeed;
	public SpriteRenderer spriteR;
	public Color spriteColor;
	public bool isWithinAttackDistance;
	public Collider2D punchHitBox;
	public int punchDamageValue;
	public float punchPushbackOnHit;
	public float punchPushbackOnBlock;
	public bool canAttack;
	Animator acolyteAnim;
	public GameObject hitSparkObject;
	public int currentReelLengthCooldown;
	public float pushBackDistance;
	public int punchReelLength;
	public bool infiniteHealth;

	// Use this for initialization
	void Start () {
		PIS = GameObject.Find("PlayerCharacter").GetComponent<PlayerInputScript> ();
		currentHealth = startHealth;
		spriteR = GetComponent<SpriteRenderer> ();
		acolyteAnim = GetComponent<Animator> ();
		spriteColor = spriteR.color;
		invincibilityCooldownCurrent = 0;
		RB2D = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		//if facing right
		if (transform.localScale.x > 0 && acolyteAnim.GetBool("isReeling") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isHeavyPunching") == false) {
			//if facing right, increment x axis
			transform.position = new Vector3(
				transform.position.x + (0.01f * enemyHorizontalSpeed),
				transform.position.y
			);
		}

		//if facing left
		else if (transform.localScale.x < 0 && acolyteAnim.GetBool("isReeling") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isHeavyPunching") == false) {
			//if facing left, decrement x axis
			transform.position = new Vector3(
				transform.position.x - (0.01f * enemyHorizontalSpeed),
				transform.position.y
			);
		}
	
		if (currentReelLengthCooldown > 0) {
			currentReelLengthCooldown--;
			if (currentReelLengthCooldown == 0){
				reelStateExit();
			}
		}
	
		if (invincibilityCooldownCurrent > 0) {
			spriteColor.a = invincibilityCooldownFlash;
			invincibilityCooldownFlash = (invincibilityCooldownFlash * -1) + 1;
			spriteR.color = spriteColor;
			invincibilityCooldownCurrent--;
			if (invincibilityCooldownCurrent == 0){
				spriteColor.a = (float)255;
				spriteR.color = spriteColor;
			}
		}
	}

void disableIsPunching () {
	acolyteAnim.SetBool ("isLightPunching", false);
	acolyteAnim.SetBool ("isHeavyPunching", false);
}
 	void flip ()
	{
		Vector2 flipLocalScale = gameObject.transform.localScale;
		flipLocalScale.x *= -1;
		transform.localScale = flipLocalScale;
	}

	void pushBack(float pushBackValue)
	{
		RB2D.velocity = (new Vector2 (RB2D.velocity.x + pushBackValue, RB2D.velocity.y));
	}

	//method for punching
	public void punch()
	{
		//hitbox stuff
		Collider2D[] cols = Physics2D.OverlapBoxAll (punchHitBox.bounds.center, punchHitBox.bounds.size, 0f, LayerMask.GetMask("Player"));

		if ( cols.Length > 0 ) 
		{
			foreach (Collider2D c in cols) 
			{
				object[] args = {punchDamageValue, transform.localScale.x * (PIS.blockPressed ? punchPushbackOnBlock : punchPushbackOnHit), punchReelLength};
				c.SendMessageUpwards ("playerTakeDamage", args);
			}
		}
	}

	public void enemyTakeDamage(object[] args){
		int damage = (int)args[0];
		float pushBackDistance = (float)args[1];
		int reelLength = (int)args[2];

		if (currentHealth > 0 && invincibilityCooldownCurrent == 0) {
			if(!infiniteHealth)
				currentHealth -= damage;
			hitSparkObject.SetActive(true);
			pushBack (pushBackDistance);
			reelStateEnter(reelLength);
			if(currentHealth <= 0)
				enemyDeath ();
		} 
	}

	//for the state that occurs right after receiving damage where acolyte is combo-able
	public void reelStateEnter(int reelLength)
	{
		currentReelLengthCooldown = reelLength;
		if (invincibilityCooldownCurrent <= 0) {
			foreach(AnimatorControllerParameter parameter in acolyteAnim.parameters) {            
				acolyteAnim.SetBool(parameter.name, false);            
			}
			acolyteAnim.SetBool ("isReeling", true);
			acolyteAnim.Play("reel", 0, 0.0f);
		}
	}

	public void reelStateExit(){
		acolyteAnim.SetBool ("isReeling", false);
		invincibilityCooldownCurrent = invincibilityCooldownPeriod;
	}

	public void enemyDeath ()
	{
		gameObject.SetActive(false);
	}

	public void resetCanAttack ()
	{
		canAttack = true;
	}

	void OnCollisionEnter2D(Collision2D col2D)
	{
		/*if (hit.GetComponent<Collider>().tag == "Player") {
			playerObject.GetComponent<PlayerHealth> ().playerTakeDamage (1);
		}*/
		if (acolyteAnim.GetBool("isHeavyPunching") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isReeling") == false && col2D.transform.position.y + col2D.collider.bounds.extents.y > transform.position.y - (spriteR.bounds.size.y / 2) && col2D.gameObject.layer != 2) {
			if(col2D.gameObject.tag == "PlayerCharacter" && !PIS.isDead) {
				return;
			}
			
			flip ();
		}
		// if (col2D.gameObject.tag == "PlayerCharacter" && invincibilityCooldownCurrent <= 0) {
		// 	flip ();
		// }
	}
}
