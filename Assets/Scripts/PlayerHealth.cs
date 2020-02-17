using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public int startHealth;
	public int currentHealth;
	public int invincibilityCooldown;
	public int invincibilityCooldownFlash = 0;
	public bool isDead = false;
	public bool isDying;
	public SpriteRenderer spriteR;
	public Color spriteColor;
	public Text deathScreen;
	public GameObject playerCharacter;
	public GameObject hitSparkObject;
	public GameObject blockSparkObject;
	public Animator animator;
	private cameraFollow cf;
	private PlayerInputScript PIS;
	public Rigidbody2D RB2D;
	public bool InfiniteHealth;

	// Use this for initialization
	void Start ()
	{
		currentHealth = startHealth;
		spriteR = GetComponent<SpriteRenderer> ();
		spriteColor = spriteR.color;
		invincibilityCooldown = 0;
		cf = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<cameraFollow> ();
		PIS = gameObject.GetComponent<PlayerInputScript> ();
		RB2D = gameObject.GetComponent<Rigidbody2D> ();
		if(!playerCharacter) {
			playerCharacter = gameObject;
		}
		if(hitSparkObject) {
			animator = GetComponent<Animator>();
		}
		if(blockSparkObject) {
			animator = GetComponent<Animator>();
		}
	}

	void Update()
	{
		if(invincibilityCooldown > 0)
			invincibilityCooldown--;
		//invincibility Animation
		if (invincibilityCooldown > 0) {
			spriteColor.a = invincibilityCooldownFlash;
			invincibilityCooldownFlash = (invincibilityCooldownFlash * -1) + 1;
			spriteR.color = spriteColor;
			invincibilityCooldown--;
			if (invincibilityCooldown == 0){
				spriteColor.a = (float)255;
				spriteR.color = spriteColor;
			}
		}
	}
	
	public void playerTakeDamage(object[] args)
	{
		int damage = (int)args [0];
		float pushBackValue = (float)args [1];

		PIS.attackHasAlreadyHit = false;

		pushBack(pushBackValue);
		if (invincibilityCooldown == 0 && !PIS.blockPressed)
		{
			if (currentHealth - damage <= 0) {
				currentHealth = 0;
				dying ();
			}

			if (currentHealth - damage > 0)
			{
				if(!InfiniteHealth)
					currentHealth -= damage;
				invincibilityCooldown = 100;
				PIS.canAct = true;
				hitSparkObject.SetActive(true);
				animator.SetBool("isReeling", true);
				animator.SetBool("isBlockingAnAttack", false);
				animator.SetBool("isPunching", false);
				animator.SetBool("isKicking", false);
			}
		} else if(invincibilityCooldown == 0 && PIS.blockPressed) {
			blockSparkObject.SetActive(true);
			animator.SetBool("isReeling", false);
			animator.SetBool("isBlockingAnAttack", true);
			animator.SetBool("isPunching", false);
			animator.SetBool("isKicking", false);
		}
	}

	void pushBack(float pushBackValue)
	{
		RB2D.velocity = (new Vector2 (RB2D.velocity.x + pushBackValue, RB2D.velocity.y));
	}

	void dying()
	{
		PIS.isDead = true;
		isDying = true;
		animator.SetBool("isDying", isDying);
		
	}
	void death()
	{
		isDying = false;
		animator.SetBool("isDying", isDying);
		RB2D.simulated = false;

		isDead = true;
		deathScreen.enabled = true;
		animator.SetBool("isDead", isDead);
		cf.enabled = !cf.enabled;
	}
}
