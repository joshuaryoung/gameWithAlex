/*

TODO: Completely rewrite controller polling and state updates

Bugs

Can't crouch with S key while blocking - attempts to save scene? -Alex
-> What key should we change crouch to then?

Vaulting over the lip of a wall after wall jump seems to rely on the exact size of the "Wall" object in the scene.
Changing the proportions of a wall makes the character behavior buggy.



*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputScript : MonoBehaviour {

	Animator anim;
	public bool isDead;
	public AnimationClip punchAnimationClip;
	public AnimationClip kickAnimationClip;
	public float jumpForce = 200.0f;
	//timer to delay another jump
	int jumpCoolDown = 0;

	RaycastHit2D hit;
	Rigidbody2D RB2D;
	public Collider2D upperHurtBox;
	public Collider2D lowerHitBox;
	public Collider2D punchHitBox;
	public Collider2D jumpingPunchHitBox;
	public Collider2D kickHitBox;
	public Collider2D jumpingKickHitBox;
	public Collider2D sweepHitBox;
	public Collider2D uppercutHitBox;
	public Collider2D throwHitBox;
	public AudioClip punchSoundEffect;
	public AudioClip kickSoundEffect;
	public AudioClip impactSoundEffect;
	public AudioSource audioPlayer;
	public int punchDamageValue;
	public int punchReelLength;
	public float punchPushbackValue;
	public int jumpingPunchDamageValue;
	public int jumpingPunchReelLength;
	public float jumpingPunchPushbackValue;
	public int kickDamageValue;
	public int kickReelLength;
	public float kickPushbackValue;
	public int jumpingKickDamageValue;
	public int jumpingKickReelLength;
	public float jumpingKickPushbackValue;
	public int sweepDamageValue;
	public float sweepPushbackValue;
	public int uppercutDamageValue;
	public int uppercutReelLength;
	public float uppercutPushbackValue;
	public float walkVelocity;
	public float runVelocity;
	public float crouchVelocity;
	public float blockVelocity;
	public float wallJumpVelocity;
	public int wallStickDurationCurrent;
	public int wallStickDurationMax;
	public float airBrakeDeceleration;
	public float xVelo;
	public float yVelo;
	public float RBgravityScale;
	public float controllerAxisX;
	public bool runHeld;
	public bool jumpPressed;
	public bool jumpReleased;
	public bool punchPressed;
	public bool kickPressed;
	public bool blockPressed;
	public bool sweepPressed;
	public bool uppercutPressed;
	public bool grabPressed;
	public bool canAct;
	public bool canCombo;
	public bool isRunning;
	public bool isWalking;
	public bool isCrouching;
	public bool isGrounded;
	public bool isWallClimbing;
	public bool isWallSliding;
	public bool isWallJumping;
	public bool isLedgeVaulting;
	public int wallJumpMinXAxisCooldownMax;
	public int wallJumpMinXAxisCooldownCurrent;
	public bool wasGroundedPreviousFrame;
	public int groundedCoolDown;
	public float localScaleX;
	public bool freeFallAvailable;		//ability to release the jump button and immediately start to fall
	public float groundCollisionOffset;
	public bool attackHasAlreadyHit;	//has an attack already registered damage?
	public int currentAutoComboIndex = 0;
	public KeyCode jumpKeyCode = new KeyCode();
	public KeyCode punchKeyCode = new KeyCode();
	public KeyCode kickKeyCode = new KeyCode();
	public KeyCode runKeyCode = new KeyCode();
	public KeyCode blockKeyCode = new KeyCode();

	// Use this for initialization
	void Start ()
	{
		audioPlayer = gameObject.GetComponent<AudioSource>();
		anim = GetComponent<Animator> ();
		punchAnimationClip.wrapMode = WrapMode.Once;
		kickAnimationClip.wrapMode = WrapMode.Once;
		RB2D = GetComponent<Rigidbody2D> ();
		isGrounded = true;
		groundedCoolDown = 0;
		canAct = true;
		attackHasAlreadyHit = false;
		wasGroundedPreviousFrame = true;
		freeFallAvailable = true;
		wallStickDurationCurrent = wallStickDurationMax;
		wallJumpMinXAxisCooldownCurrent = wallJumpMinXAxisCooldownMax;
		jumpKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump", "JoystickButton0"));
		punchKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Punch", "JoystickButton1"));
		kickKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Kick", "JoystickButton2"));
		runKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Run", "JoystickButton3"));
		blockKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Block", "JoystickButton4"));
		jumpReleased = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if(isDead) {
			return;
		}
		
		yVelo = RB2D.velocity.y;
		xVelo = RB2D.velocity.x;

		localScaleX = transform.localScale.x;

		// Poll controller
		punchPressed = Input.GetKeyDown(punchKeyCode) && !isWallClimbing && !blockPressed;
		kickPressed = (Input.GetKeyDown(kickKeyCode)  && !blockPressed && !isWallClimbing);
		sweepPressed = (Input.GetAxis ("Vertical") < 0 && isGrounded && kickPressed && !isWallClimbing);
		uppercutPressed = (Input.GetAxis ("Vertical") < 0 && isGrounded && punchPressed && !isWallClimbing);
		blockPressed = Input.GetKey(blockKeyCode);
		grabPressed = (punchPressed && blockPressed && isGrounded && !isCrouching && !isWallClimbing);
		runHeld = Input.GetKey(runKeyCode);
		isCrouching = (Input.GetAxis ("Vertical") < 0 && isGrounded && !isWallClimbing);
		controllerAxisX = Input.GetAxis ("Horizontal");
		jumpPressed = Input.GetKeyDown(jumpKeyCode);
		jumpReleased = Input.GetKeyUp(jumpKeyCode);

		if (isWallClimbing) {
			isWallJumping = false;
			if (wallStickDurationCurrent > 0 && !jumpPressed) {
				wallStickDurationCurrent--;
				//if( jumpCoolDown == 0)
				//RB2D.velocity = new Vector2 (RB2D.velocity.x, Vector2.zero.y);
				//RB2D.angularVelocity = Vector2.zero.magnitude;
				RB2D.gravityScale = 0;
			} else {
				RB2D.gravityScale = RBgravityScale;
			}
			anim.SetBool ("isFalling", false);
			anim.SetBool ("isJumping", false);
		} else {
			RB2D.gravityScale = RBgravityScale;
		}
		if (yVelo > 0f && !isWallClimbing) {
			isGrounded = false;
			anim.SetBool ("isJumping", true);
			anim.SetBool ("isFalling", false);
		}
		if (yVelo < 0f && !isWallClimbing) {
			isGrounded = false;
			anim.SetBool ("isFalling", true);
			anim.SetBool ("isJumping", false);
		}

		if (isGrounded) {
			anim.SetBool ("isJumping", false);
			anim.SetBool ("isFalling", false);
		}
		anim.SetBool ("isGrounded", isGrounded);
		anim.SetBool ("isWallClimbing", isWallClimbing);

		if(freeFallAvailable && RB2D.velocity.y > 0 && wallJumpMinXAxisCooldownCurrent <= 0){
			if (jumpReleased) {
				RB2D.velocity = new Vector2 (RB2D.velocity.x, 0);
				freeFallAvailable = false;
			}
		}

		if (!wasGroundedPreviousFrame && isGrounded) {
			canAct = true;
			isWallJumping = false;
			upperHurtBox.offset.Set (upperHurtBox.offset.x, 0.5f);
			wallStickDurationCurrent = wallStickDurationMax;
			RB2D.gravityScale = RBgravityScale;
			attackHasAlreadyHit = false;
		}

		if (canAct) {
			canCombo = false;
			attackHasAlreadyHit = false;
			currentAutoComboIndex = 0;
			anim.SetInteger("currentAutoComboIndex", currentAutoComboIndex);

			if ((isGrounded || isWallClimbing) && jumpCoolDown == 0)
			{
				if(jumpPressed && !blockPressed)
					jump();
			}
			anim.SetBool ("isBlocking", blockPressed);
			anim.SetBool ("isBackDashing", blockPressed && controllerAxisX * transform.localScale.x < 0);
			anim.SetBool ("isForwardDashing", blockPressed && controllerAxisX * transform.localScale.x > 0);
			// anim.SetBool ("isBlockWalking", blockPressed && xVelo != 0 && isGrounded);

			anim.SetBool ("isCrouching", isCrouching);
			anim.SetBool ("isPunching", (punchPressed && !uppercutPressed && !grabPressed));
			if(punchPressed && !uppercutPressed && !grabPressed) {
				anim.Play("Punch", 0, 0.0f);
				playSoundEffect(punchSoundEffect);
			}
			anim.SetBool ("isKicking", (kickPressed && !sweepPressed  && !grabPressed));
			if(kickPressed && !sweepPressed  && !grabPressed) {
				anim.Play("Kick", 0, 0.0f);
				playSoundEffect(kickSoundEffect);
			}
			anim.SetBool ("isSweeping", sweepPressed);
			anim.SetBool ("isUppercutting", uppercutPressed);
			anim.SetBool ("isGrabbing", grabPressed);

			isWalking = (controllerAxisX != 0 && !runHeld);
			isRunning = (controllerAxisX != 0 && runHeld);
			anim.SetBool ("isWalking", isWalking );
			anim.SetBool ("isRunning", isRunning);
			anim.SetBool ("isLedgeVaulting", isLedgeVaulting);

			if (isLedgeVaulting) {
				RB2D.gravityScale = RBgravityScale;
				isWallClimbing = false;
				wallJumpMinXAxisCooldownCurrent = 0;
			}

			//Check to see if X Axis was pressed
			if (controllerAxisX != 0) {
				movePlayer ();
			}
			//Airbrake logic
			if(controllerAxisX == 0 && !isGrounded && wallJumpMinXAxisCooldownCurrent <= 0)
			{
				if (xVelo * transform.localScale.x > 0) {
					RB2D.velocity = new Vector2 (xVelo * airBrakeDeceleration, RB2D.velocity.y);
				}
				//if less than zero then reset to zero
				if (xVelo * transform.localScale.x < 0) {
					RB2D.velocity = new Vector2 (0f, RB2D.velocity.y);
				}
			}
		}

		if (canCombo) {
			if ((punchPressed || kickPressed || sweepPressed || uppercutPressed) && isGrounded) {
				attackHasAlreadyHit = false;
				if(currentAutoComboIndex < 2) {
					currentAutoComboIndex++;
					anim.SetInteger("currentAutoComboIndex", currentAutoComboIndex);
					
					foreach(AnimatorControllerParameter parameter in anim.parameters) {
						switch (parameter.type.ToString())
						{
								case "Int":
									break;
								default:
									anim.SetBool(parameter.name, false);
									break;
						}
					}
					anim.SetBool ("isPunching", (punchPressed && !uppercutPressed && !grabPressed));
					anim.SetBool ("isKicking", (kickPressed && !sweepPressed && !grabPressed));
					anim.SetBool ("isSweeping", sweepPressed);
					anim.SetBool ("isUppercutting", uppercutPressed);
					anim.SetBool ("isGrounded", isGrounded);
				}

			}
		}

		if (jumpCoolDown > 0) {
			jumpCoolDown--;
		}
		if (groundedCoolDown > 0) {
			groundedCoolDown--;
		}
		if (wallJumpMinXAxisCooldownCurrent > 0) {
			wallJumpMinXAxisCooldownCurrent--;
		}
		wasGroundedPreviousFrame = isGrounded;
	}

	void movePlayer ()
	{
		//If direction pushed == direction facing right, then move that way
		if (transform.localScale.x * controllerAxisX > 0 && !blockPressed) {
			if (isWalking && !isCrouching && wallJumpMinXAxisCooldownCurrent <= 0) {
				RB2D.velocity = new Vector2 (controllerAxisX * walkVelocity, RB2D.velocity.y);
			}
			if (isRunning && !isCrouching && wallJumpMinXAxisCooldownCurrent <= 0) {
				RB2D.velocity = new Vector2 (controllerAxisX * runVelocity, RB2D.velocity.y);
			}
			if (isCrouching && wallJumpMinXAxisCooldownCurrent <= 0) {
				RB2D.velocity = new Vector2 (controllerAxisX * crouchVelocity, RB2D.velocity.y);
			}

		} else {
			if(!blockPressed)
				flipPlayer ();
			if (blockPressed && transform.localScale.x * controllerAxisX < 0 && isGrounded) {
				RB2D.velocity = new Vector2 (controllerAxisX * blockVelocity, RB2D.velocity.y);
			}
		}

	}

	//Method for flipping the sprite on the horizontal axis
	void flipPlayer ()
	{
		Vector2 flipLocalScale = transform.localScale;
		flipLocalScale.x *= -1;
		transform.localScale = flipLocalScale;
	}

	//method for jumping
	void jump()
	{
		RB2D.gravityScale = RBgravityScale;
		//jump logic
		if (!isWallClimbing) {
			RB2D.velocity = new Vector2 (RB2D.velocity.x, jumpForce);
			isWallClimbing = false;
			freeFallAvailable = true;
		}else {
			isWallJumping = true;
			wallJumpMinXAxisCooldownCurrent = wallJumpMinXAxisCooldownMax;
			freeFallAvailable = false;
			RB2D.velocity = new Vector2 (wallJumpVelocity * (transform.localScale.x * -1), jumpForce);
			isWallClimbing = false;
		}
		//set cooldown

		jumpCoolDown = 5;
	}

	void attack(Collider2D hitBox, int damageValue, float pushbackValue, int reelLength, AudioClip soundEffect){
		string nameOfPreviousCol="null";
		//hitbox stuff
		Collider2D[] cols = Physics2D.OverlapBoxAll (hitBox.bounds.center, hitBox.bounds.size, 0f, LayerMask.GetMask("EnemyLayer"));
	
		if ( cols.Length > 0 ) 
		{
			canCombo = true;
			attackHasAlreadyHit = true;
			playSoundEffect(soundEffect);
			foreach (Collider2D c in cols) 
			{
				if (!string.Equals (c.transform.parent.name, nameOfPreviousCol)) {
					object[] args = {damageValue, transform.localScale.x * pushbackValue, reelLength};
					c.SendMessageUpwards ("enemyTakeDamage", args);
					c.SendMessageUpwards ("resetCanAttack");
				}
				nameOfPreviousCol = c.transform.parent.name;
			}
		}
	}

	//method for punching
	void punch()
	{
		if(!attackHasAlreadyHit){
			attack (punchHitBox, punchDamageValue, punchPushbackValue, punchReelLength, impactSoundEffect);
		}
	}

	void playSoundEffect(AudioClip src) {
		audioPlayer.clip = src;
		audioPlayer.Play();
	}

	void jumpingPunch()
	{
		if (!attackHasAlreadyHit) {
			attack (jumpingPunchHitBox, jumpingPunchDamageValue, jumpingPunchPushbackValue, jumpingPunchReelLength, impactSoundEffect);
		}
	}

	void kick()
	{
		if (!attackHasAlreadyHit) {
			attack (kickHitBox, kickDamageValue, kickPushbackValue, kickReelLength, impactSoundEffect);
		}
	}

	void jumpingKick()
	{
		if (!attackHasAlreadyHit) {
			attack (jumpingKickHitBox, jumpingKickDamageValue, jumpingKickPushbackValue, jumpingKickReelLength, impactSoundEffect);
		}
	}

	void sweep()
	{
		if (!attackHasAlreadyHit) {
			attack (sweepHitBox, sweepDamageValue, sweepPushbackValue, 0, impactSoundEffect);
		}
	}

	void uppercut()
	{
		if (!attackHasAlreadyHit) {
			attack (uppercutHitBox, uppercutDamageValue, uppercutPushbackValue, uppercutReelLength, impactSoundEffect);
		}
	}

	void resetAttackHasAlreadyHit(){
		attackHasAlreadyHit = false;
	}

	//ground check
	void OnCollisionStay2D(Collision2D col2D)
	{
		if ((col2D.collider.bounds.extents.y + col2D.gameObject.transform.position.y + groundCollisionOffset < transform.position.y - lowerHitBox.bounds.extents.y) && col2D.gameObject.layer == LayerMask.NameToLayer ("Ledge")) {
			isLedgeVaulting = true;
		}
	}
}
