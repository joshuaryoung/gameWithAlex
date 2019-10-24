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
	public int jumpingPunchDamageValue;
	public int kickDamageValue;
	public int jumpingKickDamageValue;
	public int sweepDamageValue;
	public int uppercutDamageValue;
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
	}

	// Update is called once per frame
	void Update ()
	{
		yVelo = RB2D.velocity.y;
		xVelo = RB2D.velocity.x;

		localScaleX = transform.localScale.x;

		// Poll controller
		punchPressed = Input.GetButtonDown ("Punch") && !isWallClimbing && !blockPressed;
		kickPressed = (Input.GetButtonDown ("Kick")  && !blockPressed && !isWallClimbing);
		sweepPressed = (Input.GetAxis ("Vertical") < 0 && isGrounded && kickPressed && !isWallClimbing);
		uppercutPressed = (Input.GetAxis ("Vertical") < 0 && isGrounded && punchPressed && !isWallClimbing);
		blockPressed = Input.GetButton ("Block");
		grabPressed = (punchPressed && blockPressed && isGrounded && !isCrouching && !isWallClimbing);
		runHeld = Input.GetButton("Run");
		isCrouching = (Input.GetAxis ("Vertical") < 0 && isGrounded && !isWallClimbing);
		controllerAxisX = Input.GetAxis ("Horizontal");
		jumpPressed = Input.GetButtonDown ( "Jump" );

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
			if (Input.GetButtonUp ( "Jump" ) ) {
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
		}

		if (canAct) {
			canCombo = false;

			if ((isGrounded || isWallClimbing) && jumpCoolDown == 0)
			{
				if(jumpPressed && !blockPressed)
					jump ();
			}
			anim.SetBool ("isBlocking", blockPressed);
			anim.SetBool ("isCrouching", isCrouching);
			anim.SetBool ("isPunching", (punchPressed && !uppercutPressed && !grabPressed));
			if(punchPressed && !uppercutPressed && !grabPressed) {
				playSoundEffect(punchSoundEffect);
			}
			anim.SetBool ("isKicking", (kickPressed && !sweepPressed  && !grabPressed));
			if(kickPressed && !sweepPressed  && !grabPressed) {
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
			if (punchPressed || kickPressed || sweepPressed || uppercutPressed) {
				foreach(AnimatorControllerParameter parameter in anim.parameters) {            
					anim.SetBool(parameter.name, false);            
				}
				attackHasAlreadyHit = false;
				anim.SetBool ("isPunching", (punchPressed && !uppercutPressed && !grabPressed));
				if(punchPressed && !uppercutPressed && !grabPressed) {
					playSoundEffect(punchSoundEffect);
				}
				anim.SetBool ("isKicking", (kickPressed && !sweepPressed && !grabPressed));
				if(kickPressed && !sweepPressed  && !grabPressed) {
					playSoundEffect(kickSoundEffect);
				}
				anim.SetBool ("isSweeping", sweepPressed);
				anim.SetBool ("isUppercutting", uppercutPressed);
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

	void attack(Collider2D hitBox, int damageValue, AudioClip soundEffect){
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
					c.SendMessageUpwards ("reelStateEnter");
					c.SendMessageUpwards ("enemyTakeDamage", damageValue);
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
			attack (punchHitBox, punchDamageValue, impactSoundEffect);
		}
	}

	void playSoundEffect(AudioClip src) {
		audioPlayer.clip = src;
		audioPlayer.Play();
	}

	void jumpingPunch()
	{
		if (!attackHasAlreadyHit) {
			attack (jumpingPunchHitBox, jumpingPunchDamageValue, impactSoundEffect);
		}
	}

	void kick()
	{
		if (!attackHasAlreadyHit) {
			attack (kickHitBox, kickDamageValue, impactSoundEffect);
		}
	}

	void jumpingKick()
	{
		if (!attackHasAlreadyHit) {
			attack (jumpingKickHitBox, jumpingKickDamageValue, impactSoundEffect);
		}
	}

	void sweep()
	{
		if (!attackHasAlreadyHit) {
			attack (sweepHitBox, sweepDamageValue, impactSoundEffect);
		}
	}

	void uppercut()
	{
		if (!attackHasAlreadyHit) {
			attack (uppercutHitBox, uppercutDamageValue, impactSoundEffect);
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
			Debug.Log ("ledgeJump as a result of collsion");
		}
	}
}
