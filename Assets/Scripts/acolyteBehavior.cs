using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acolyteBehavior : MonoBehaviour {
	//Raycast class;
	Collider2D hit;
	Rigidbody2D RB2D;

	public GameObject playerObject;
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
	public bool canAttack;
	Animator acolyteAnim;
	public GameObject hitSparkObject;
	public AudioSource audioSrc;
	public AudioClip impactSoundEffect;
	public int reelLength;
	public float pushBackDistance;

	// Use this for initialization
	void Start () {
		audioSrc = GetComponent<AudioSource> ();
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
		if (transform.localScale.x > 0 && acolyteAnim.GetBool("isReeling") == false) {
			//if facing right, increment x axis
			transform.position = new Vector3(
				transform.position.x + (0.01f * enemyHorizontalSpeed),
				transform.position.y
			);
		}

		//if facing left
		else if (transform.localScale.x < 0 && acolyteAnim.GetBool("isReeling") == false) {
			//if facing left, decrement x axis
			transform.position = new Vector3(
				transform.position.x - (0.01f * enemyHorizontalSpeed),
				transform.position.y
			);
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
		//if (reelLength > 0) {
			//reelLength--;
			//if (reelLength == 0) {
				//acolyteAnim.SetBool ("isReeling", false);
			//}
		//}
	}

	void flip ()
	{
		Vector2 flipLocalScale = gameObject.transform.localScale;
		flipLocalScale.x *= -1;
		transform.localScale = flipLocalScale;
	}

	void pushBack(float pushBackValue)
	{
		RB2D.velocity = (new Vector2 (RB2D.velocity.x + (pushBackValue * transform.localScale.x * -1), RB2D.velocity.y));
	}

	//method for punching
	public void punch()
	{
		//hitbox stuff
		Collider2D[] cols = Physics2D.OverlapBoxAll (punchHitBox.bounds.center, punchHitBox.bounds.size, 0f, LayerMask.GetMask("Player"));

		if ( cols.Length > 0 ) 
		{
			audioSrc.clip = impactSoundEffect;
			audioSrc.Play();
			foreach (Collider2D c in cols) 
			{
				c.SendMessageUpwards ("playerTakeDamage", punchDamageValue);
			}
		}
	}

	public void enemyTakeDamage(int damage){
		if (currentHealth > 0 && invincibilityCooldownCurrent == 0) {
			currentHealth -= damage;
			hitSparkObject.SetActive(true);
			reelStateEnter();
			if(currentHealth <= 0)
				enemyDeath ();
		} 
	}

	//for the state that occurs right after receiving damage where acolyte is combo-able
	public void reelStateEnter()
	{
		pushBack (pushBackDistance);
		if (invincibilityCooldownCurrent <= 0) {
			acolyteAnim.SetBool ("isReeling", true);
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
		if (col2D.transform.position.y + col2D.collider.bounds.extents.y > transform.position.y - (spriteR.bounds.size.y / 2)  && col2D.gameObject.tag != "PlayerCharacter" && col2D.gameObject.layer != 2) {
			Debug.Log("tag != PlayerCharacter");
			flip ();
		}
		// if (col2D.gameObject.tag == "PlayerCharacter" && invincibilityCooldownCurrent <= 0) {
		// 	flip ();
		// }
	}
}
