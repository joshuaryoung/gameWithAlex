﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputScript : MonoBehaviour
{

    Animator anim;
    public bool isDead;
    public bool isAbleToAct;
    public bool canCombo;
    public bool isBeingGrabbed;
    public bool isInGrabAttackState;
    public bool isForwardDashing;
    public bool isBackDashing;
    public bool runHeld;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool dashReleased;
    public bool punchPressed;
    public bool kickPressed;
    public bool blockPressed;
    public bool isBlocking;
    public bool isGrabbing;
    public bool sweepPressed;
    public bool uppercutPressed;
    public bool grabPressed;
    public bool lockOnPressed;
    public bool isRunning;
    public bool isWalking;
    public bool isBackWalking;
    public bool isCrouching;
    public bool isGrounded;
    public bool isWallClimbing;
    public bool isWallSliding;
    public bool isWallJumping;
    public bool hasReleasedWall;
    public bool isLedgeVaulting;
    public bool wasGroundedPreviousFrame;
    public bool freeFallAvailable;  
    public bool attackHasAlreadyHit;  
    public bool dpadLeftPressed;
    public bool dpadRightPressed;
    public GameObject currentlyGrabbedEnemy = null;
    public AnimationClip punchAnimationClip;
    public AnimationClip kickAnimationClip;
    public PlayerHealth PH;
    public float jumpForce = 200.0f;
    //timer to delay another jump
    public float jumpCoolDownCurrent = 0;
    public float jumpCoolDownMax;
    RaycastHit2D hit;
    Rigidbody2D RB2D;
    public Collider2D upperHurtbox;
    public Collider2D lowerHurtbox;
    public Collider2D lowerHitBox;
    public Collider2D punchHitBox;
    public Collider2D punchAC2HitBox;
    public Collider2D punchAC3HitBox;
    public Collider2D jumpingPunchHitBox;
    public Collider2D kickHitBox;
    public Collider2D kickAC2HitBox;
    public Collider2D kickAC3HitBox;
    public Collider2D jumpingKickHitBox;
    public Collider2D sweepHitBox;
    public Collider2D uppercutHitBox;
    public Collider2D grabAttackStatePunchHitBox;
    public Collider2D grabAttackStateKickHitBox;
    public Collider2D grabAttackStateThrowHitBox;
    public Collider2D throwHitBox;
    public AudioClip punchSoundEffect;
    public AudioClip punchAC2SoundEffect;
    public AudioClip punchAC3SoundEffect;
    public AudioClip kickSoundEffect;
    public AudioClip kickAC2SoundEffect;
    public AudioClip kickAC3SoundEffect;
    public AudioClip impactSoundEffect;
    public AudioClip blockSoundEffect;
    public AudioClip throwSoundEffect;
    public AudioSource audioPlayer;
    public Collider2D collisionBoxCol2D;
    public int punchDamageValue;
    public int punchAC2DamageValue;
    public int punchAC3DamageValue;
    public float punchReelLength;
    public float punchAC2ReelLength;
    public float punchAC3ReelLength;
    public float punchBlockStunLength;
    public float punchAC2BlockStunLength;
    public float punchAC3BlockStunLength;
    public float punchPushbackValue;
    public float punchAC2PushbackValue;
    public float punchAC3PushbackValue;
    public int jumpingPunchDamageValue;
    public float jumpingPunchReelLength;
    public float jumpingPunchBlockStunLength;
    public float jumpingPunchPushbackValue;
    public Vector2 grabAttackStateThrowForce;
    public int kickDamageValue;
    public int kickAC2DamageValue;
    public int kickAC3DamageValue;
    public float kickReelLength;
    public float kickAC2ReelLength;
    public float kickAC3ReelLength;
    public float kickBlockStunLength;
    public float kickAC2BlockStunLength;
    public float kickAC3BlockStunLength;
    public float kickPushbackValue;
    public float kickAC2PushbackValue;
    public float kickAC3PushbackValue;
    public int jumpingKickDamageValue;
    public float jumpingKickReelLength;
    public float jumpingKickBlockStunLength;
    public float jumpingKickPushbackValue;
    public int sweepDamageValue;
    public float sweepBlockStunLength;
    public float sweepPushbackValue;
    public int uppercutDamageValue;
    public float uppercutReelLength;
    public float uppercutBlockStunLength;
    public float uppercutPushbackValue;
    public int grabAttackStatePunchDamageValue;
    public int grabAttackStateKickDamageValue;
    public int grabAttackStateThrowDamageValue;
    public float walkVelocity;
    public float runVelocity;
    public float crouchVelocity;
    public float backDashVelocity;
    public float forwardDashVelocity;
    public float wallJumpVelocity;
    public float wallStickDurationCurrent;
    public float wallStickDurationMax;
    public float airBrakeDeceleration;
    public float xVelo;
    public float yVelo;
    public float RBgravityScale;
    public float controllerAxisX;
    public float wallJumpMinXAxisCooldownMax;
    public float wallJumpMinXAxisCooldownCurrent;
    public float groundedCoolDown;
    public float localScaleX;    //ability to release the jump button and immediately start to fall
    public float groundCollisionOffset;  //has an attack already registered damage?
    public int currentAutoComboIndex = 0;
    public int currentGrabAttackIndex = 0;
    public KeyCode jumpKeyCode;
    public KeyCode punchKeyCode;
    public KeyCode kickKeyCode;
    public KeyCode runKeyCode;
    public KeyCode blockKeyCode;
    public KeyCode dashKeyCode;
    public KeyCode upKeyCode;
    public KeyCode downKeyCode;
    public KeyCode leftKeyCode;
    public KeyCode rightKeyCode;
    public KeyCode lockOnKeyCode;
    public SettingsScript settingsScript;
    public HitStop HS;
    public CurrentlyVisableObjects CVO;

    // Use this for initialization
    void Start()
    {
        audioPlayer = gameObject.GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        punchAnimationClip.wrapMode = WrapMode.Once;
        kickAnimationClip.wrapMode = WrapMode.Once;
        RB2D = GetComponent<Rigidbody2D>();
        isGrounded = true;
        groundedCoolDown = 0;
        isAbleToAct = true;
        attackHasAlreadyHit = false;
        wasGroundedPreviousFrame = true;
        freeFallAvailable = true;
        wallStickDurationCurrent = wallStickDurationMax;
        // wallJumpMinXAxisCooldownCurrent = wallJumpMinXAxisCooldownMax;
        jumpKeyCode = PlayerPrefs.GetString("Jump") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump")) : new KeyCode();
        punchKeyCode = PlayerPrefs.GetString("Punch") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Punch")) : new KeyCode();
        kickKeyCode = PlayerPrefs.GetString("Kick") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Kick")) : new KeyCode();
        runKeyCode = PlayerPrefs.GetString("Run") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Run")) : new KeyCode();
        blockKeyCode = PlayerPrefs.GetString("Block") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Block")) : new KeyCode();
        lockOnKeyCode = PlayerPrefs.GetString("LockOn") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LockOn")) : new KeyCode();
        upKeyCode = PlayerPrefs.GetString("Up") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up")) : new KeyCode();
        downKeyCode = PlayerPrefs.GetString("Down") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down")) : new KeyCode();
        leftKeyCode = PlayerPrefs.GetString("Left") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left")) : new KeyCode();
        rightKeyCode = PlayerPrefs.GetString("Right") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right")) : new KeyCode();
        dashKeyCode = PlayerPrefs.GetString("Dash") != "" ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Dash")) : new KeyCode();
        jumpReleased = true;
        dashReleased = true;
        if (HS == null) {
            HS = FindObjectOfType<HitStop>();
        }
        if (CVO == null) {
            CVO = FindObjectOfType<CurrentlyVisableObjects>();
        }
        hasReleasedWall = true;
        if (PH == null) {
            PH = GetComponent<PlayerHealth>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (settingsScript == null)
        {
            Debug.LogError("settings script not assigned!");
            return;
        }
        if (CVO == null)
        {
            Debug.LogError("CVO script not assigned!");
            return;
        }

        if (collisionBoxCol2D == null) {
            Debug.LogError("collisionBoxCol2D is null!");
            return;
        }

        if (lowerHurtbox == null) {
            Debug.LogError("lowerHurtbox is null!");
            return;
        }

        if (upperHurtbox == null) {
            Debug.LogError("upperHurtbox is null!");
            return;
        }

        if (PH == null) {
            Debug.LogError("PH is null!");
            return;
        }
        if (isDead)
        {
            return;
        }

        if (isBeingGrabbed) {
            return;
        }

        if (settingsScript.gamePaused)
        {
            return;
        }

        yVelo = RB2D.velocity.y;
        xVelo = RB2D.velocity.x;

        localScaleX = transform.localScale.x;

        // Poll controller
        punchPressed = Input.GetKeyDown(punchKeyCode);
        kickPressed = (Input.GetKeyDown(kickKeyCode) && !blockPressed && !isWallClimbing);
        sweepPressed = ((Input.GetAxis("Vertical") < 0 || Input.GetKey(downKeyCode)) && isGrounded && kickPressed && !isWallClimbing);
        uppercutPressed = ((Input.GetAxis("Vertical") < 0 || Input.GetKey(downKeyCode)) && isGrounded && punchPressed && !isWallClimbing);
        blockPressed = Input.GetKey(blockKeyCode);
        if (!dashReleased) {
            dashReleased = Input.GetKeyUp(dashKeyCode);
            if (dashReleased) {
                isForwardDashing = false;
                isBackDashing = false;
            }
        }
        grabPressed = punchPressed && blockPressed;
        runHeld = Input.GetKey(runKeyCode);
        lockOnPressed = Input.GetKeyDown(lockOnKeyCode);
        if (lockOnPressed) {
            CVO.initiateLockOn();
        }
        isCrouching = ((Input.GetAxis("Vertical") < 0 || Input.GetKey(downKeyCode)) && isGrounded && !isWallClimbing);
        controllerAxisX = Input.GetAxis("Horizontal");
        if (controllerAxisX == 0)
        {
            dpadLeftPressed = Input.GetKey(leftKeyCode);
            dpadRightPressed = Input.GetKey(rightKeyCode);
            if (dpadLeftPressed || dpadRightPressed)
            {
                controllerAxisX = dpadLeftPressed ? -1 : 1;
            }

            if(controllerAxisX == 0) {
                dashReleased = true;
                isForwardDashing = false;
                isBackDashing = false;
            }
        }
        jumpPressed = Input.GetKeyDown(jumpKeyCode);
        jumpReleased = Input.GetKeyUp(jumpKeyCode);
        isForwardDashing = isGrounded && !jumpPressed && dashReleased && Input.GetKey(dashKeyCode) && transform.localScale.x * controllerAxisX > 0;
        isBackDashing = isGrounded && !jumpPressed && dashReleased && Input.GetKey(dashKeyCode) && transform.localScale.x * controllerAxisX < 0;

        if (isWallClimbing)
        {
            isWallJumping = false;
            if (wallStickDurationCurrent > 0 && !jumpPressed)
            {
                wallStickDurationCurrent -= Time.deltaTime;
                //if( jumpCoolDownCurrent == 0)
                //RB2D.velocity = new Vector2 (RB2D.velocity.x, Vector2.zero.y);
                //RB2D.angularVelocity = Vector2.zero.magnitude;
                RB2D.gravityScale = 0;
            }
            else
            {
                RB2D.gravityScale = RBgravityScale;
            }
            anim.SetBool("isFalling", false);
            anim.SetBool("isJumping", false);
        }
        else
        {
            RB2D.gravityScale = RBgravityScale;
        }
        if (yVelo > 0f && !isWallClimbing)
        {
            isGrounded = false;
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
        }
        if (yVelo < 0f && !isWallClimbing)
        {
            isGrounded = false;
            anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
        }

        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallClimbing", isWallClimbing);

        if (freeFallAvailable && RB2D.velocity.y > 0 && wallJumpMinXAxisCooldownCurrent <= 0)
        {
            if (jumpReleased)
            {
                RB2D.velocity = new Vector2(RB2D.velocity.x, 0);
                freeFallAvailable = false;
            }
        }

        if (!wasGroundedPreviousFrame && isGrounded)
        {
            isAbleToAct = true;
            isWallJumping = false;
            upperHurtbox.offset.Set(upperHurtbox.offset.x, 0.5f);
            wallStickDurationCurrent = wallStickDurationMax;
            RB2D.gravityScale = RBgravityScale;
            attackHasAlreadyHit = false;
        }

        if (isAbleToAct)
        {
            if (controllerAxisX != 0 && !isInGrabAttackState)
            {
                movePlayer();
            }
            isGrabbing = grabPressed;
            isBlocking = blockPressed && !isGrabbing && !isForwardDashing && !isBackDashing && isGrounded;
            canCombo = false;
            attackHasAlreadyHit = false;
            currentAutoComboIndex = 0;

            anim.SetInteger("currentAutoComboIndex", currentAutoComboIndex);

            if ((isGrounded || isWallClimbing) && jumpCoolDownCurrent <= 0)
            {
                if (jumpPressed && !blockPressed && !isInGrabAttackState)
                    jump();
                if (isInGrabAttackState && jumpPressed) {
                    grabAttackStateExit();
                }
            }
            anim.SetBool("isBlocking", isBlocking);
            anim.SetBool("isBackDashing", isBackDashing);
            anim.SetBool("isForwardDashing", isForwardDashing);
            collisionBoxCol2D.gameObject.layer = isForwardDashing ? LayerMask.NameToLayer("OnlyInteractsWithWallsGround") : LayerMask.NameToLayer("PlayerHurtbox");
            // anim.SetBool ("isBlockWalking", blockPressed && xVelo != 0 && isGrounded);

            anim.SetBool("isCrouching", isCrouching);
            anim.SetBool("isPunching", (punchPressed && !uppercutPressed && !grabPressed));
            anim.SetBool("isKicking", (kickPressed && !sweepPressed && !grabPressed));
            anim.SetBool("isSweeping", sweepPressed);
            anim.SetBool("isUppercutting", uppercutPressed);
            anim.SetBool("isGrabbing", grabPressed);
            bool isFacingTheDirectionPressed = transform.localScale.x * controllerAxisX >= 0;
            anim.SetBool("isGrabAttackStateThrowing", isInGrabAttackState && punchPressed && !isFacingTheDirectionPressed);

            isWalking = (controllerAxisX != 0 && !runHeld && !blockPressed && !isInGrabAttackState);
            if (CVO.isLockedOn) {
                bool movementIsAwayFromEnemy = controllerAxisX * transform.localScale.x < 0;
                isBackWalking = isWalking && movementIsAwayFromEnemy; 
            } else {
                isBackWalking = false;
            }
            isRunning = (controllerAxisX != 0 && runHeld && !blockPressed && !isInGrabAttackState);
            if (CVO.isLockedOn && !isWallClimbing) {
                // Check: is player facing enemy?
                bool playerFacingEnemy = (gameObject.transform.localPosition.x - CVO.lockedOnEnemyObj.transform.localPosition.x) * gameObject.transform.localScale.x < 0;
                if (!playerFacingEnemy && !isRunning) {
                    flipPlayer();
                }
            }

            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isBackWalking", isBackWalking);
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isLedgeVaulting", isLedgeVaulting);

            if (isLedgeVaulting)
            {
                RB2D.gravityScale = RBgravityScale;
                isWallClimbing = false;
                hasReleasedWall = true;
                wallJumpMinXAxisCooldownCurrent = 0;
            }
            //Airbrake logic
            if (controllerAxisX == 0 && !isGrounded && wallJumpMinXAxisCooldownCurrent <= 0)
            {
                if (xVelo * transform.localScale.x > 0)
                {
                    RB2D.velocity = new Vector2(xVelo * airBrakeDeceleration, RB2D.velocity.y);
                }
                //if less than zero then reset to zero
                if (xVelo * transform.localScale.x < 0)
                {
                    RB2D.velocity = new Vector2(0f, RB2D.velocity.y);
                }
            }
        }

        if (canCombo)
        {
            if ((punchPressed || kickPressed || sweepPressed || uppercutPressed) && isGrounded && !grabPressed)
            {
                if (currentAutoComboIndex < 2)
                {
                    attackHasAlreadyHit = false;
                    currentAutoComboIndex++;
                    anim.SetInteger("currentAutoComboIndex", currentAutoComboIndex);

                    foreach (AnimatorControllerParameter parameter in anim.parameters)
                    {
                        switch (parameter.type.ToString())
                        {
                            case "Int":
                                break;
                            default:
                                anim.SetBool(parameter.name, false);
                                break;
                        }
                    }
                    anim.SetBool("isPunching", (punchPressed && !uppercutPressed && !grabPressed));
                    anim.SetBool("isKicking", (kickPressed && !sweepPressed && !grabPressed));
                    anim.SetBool("isSweeping", sweepPressed);
                    anim.SetBool("isUppercutting", uppercutPressed);
                    anim.SetBool("isGrounded", isGrounded);
                }

            }
        }

        if (jumpCoolDownCurrent > 0)
        {
            jumpCoolDownCurrent -= Time.deltaTime;
        }
        if (groundedCoolDown > 0)
        {
            groundedCoolDown -= Time.deltaTime;
        }
        if (wallJumpMinXAxisCooldownCurrent > 0)
        {
            wallJumpMinXAxisCooldownCurrent -= Time.deltaTime;
        }
        wasGroundedPreviousFrame = isGrounded;
    }

    void movePlayer()
    {
        bool isFacingTheDirectionPressed = transform.localScale.x * controllerAxisX > 0;
        if (isWallClimbing && !isFacingTheDirectionPressed) {
            isWallClimbing = false;
            anim.SetBool("isWallClimbing", false);
            anim.SetBool("isFalling", true);
        }
        if (isBackDashing && isGrounded) {
            dashReleased = false;
            RB2D.velocity = new Vector2(-transform.localScale.x * backDashVelocity, RB2D.velocity.y);
            return;
        } else if (isForwardDashing && isGrounded) {
            dashReleased = false;
            RB2D.velocity = new Vector2(transform.localScale.x * forwardDashVelocity, RB2D.velocity.y);
            return;
        }
        //If direction pushed == direction facing right, then move that way
        if ((isFacingTheDirectionPressed || CVO.isLockedOn) && !blockPressed)
        {
            if (isWalking && !isCrouching && wallJumpMinXAxisCooldownCurrent <= 0)
            {
                RB2D.velocity = new Vector2(controllerAxisX * walkVelocity, RB2D.velocity.y);
            }
            if (isRunning && !isCrouching && wallJumpMinXAxisCooldownCurrent <= 0)
            {
                RB2D.velocity = new Vector2(controllerAxisX * runVelocity, RB2D.velocity.y);
            }
            if (isCrouching && wallJumpMinXAxisCooldownCurrent <= 0)
            {
                RB2D.velocity = new Vector2(controllerAxisX * crouchVelocity, RB2D.velocity.y);
            }
            if (isGrounded && runHeld && !isFacingTheDirectionPressed) {
                flipPlayer();
            }
        }
        else
        {
            if (!blockPressed && !CVO.isLockedOn) {
                flipPlayer();
            }
        }

    }

    //Method for flipping the sprite on the horizontal axis
    void flipPlayer()
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
        if (!isWallClimbing)
        {
            RB2D.velocity = new Vector2(RB2D.velocity.x, jumpForce);
            isWallClimbing = false;
            hasReleasedWall = true;
            freeFallAvailable = true;
        }
        else
        {
            isWallJumping = true;
            wallJumpMinXAxisCooldownCurrent = wallJumpMinXAxisCooldownMax;
            freeFallAvailable = false;
            RB2D.velocity = new Vector2(wallJumpVelocity * (transform.localScale.x * -1), jumpForce);
            isWallClimbing = false;
            hasReleasedWall = true;
        }
        //set cooldown

        jumpCoolDownCurrent = jumpCoolDownMax;
    }

    void attack(Collider2D hitBox, int damageValue, float pushbackValue, float reelLength, float blockStunLength, AudioClip hitSoundEffect, AudioClip _blockSoundEffect)
    {
        string nameOfPreviousCol = "null";
        canCombo = true;
        //hitbox stuff
        Collider2D[] cols = Physics2D.OverlapBoxAll(hitBox.bounds.center, hitBox.bounds.size, 0f, LayerMask.GetMask("EnemyHurtbox"));

        if (cols.Length > 0)
        {
            // canCombo = true;
            // attackHasAlreadyHit = true;
            // playSoundEffect(soundEffect);
            // HS.stop();
            foreach (Collider2D c in cols)
            {
                if (!string.Equals(c.transform.parent.name, nameOfPreviousCol))
                {
                    object[] args = { damageValue, transform.localScale.x * pushbackValue, reelLength, blockStunLength, hitSoundEffect, _blockSoundEffect };
                    c.SendMessageUpwards("enemyTakeDamage", args);
                }
                nameOfPreviousCol = c.transform.parent.name;
            }
        }
    }

    void grabHitCheck(Collider2D hitBox, AudioClip soundEffect)
    {
        //hitbox stuff
        Collider2D[] cols = Physics2D.OverlapCircleAll(hitBox.bounds.center, hitBox.bounds.extents.x, LayerMask.GetMask("EnemyHurtbox"));

        if (cols.Length > 0)
        {
            Collider2D closestCol = cols[0];
            foreach(Collider2D el in cols) {
                float distanceToEl = Mathf.Abs(el.bounds.center.x - hitBox.bounds.center.x);
                float distanceToClosestCol = Mathf.Abs(closestCol.bounds.center.x - hitBox.bounds.center.x);
                if (distanceToEl < distanceToClosestCol) {
                    closestCol = el;
                }
            }
            currentlyGrabbedEnemy = closestCol.transform.parent.gameObject;
            if (currentlyGrabbedEnemy.GetComponent<Animator>().GetBool("isDying")) {
                currentlyGrabbedEnemy = null;
                return;
            }
            attackHasAlreadyHit = true;
            playSoundEffect(soundEffect);
            currentlyGrabbedEnemy.SendMessage("enemyGetGrabbed");
            anim.SetBool("isInGrabAttackState", true);
            isInGrabAttackState = true;
        }
    }

    void grabAttack(Collider2D hitBox, int damageValue, AudioClip hitSoundEffect)
    {
        if (currentlyGrabbedEnemy != null)
        {
            currentGrabAttackIndex++;
            object[] args = { damageValue, hitSoundEffect };
            currentlyGrabbedEnemy.SendMessageUpwards("enemyTakeGrabAttackDamage", args);
        }
    }

    void grabThrowAttack(Collider2D hitBox, int damageValue, Vector2 throwForce)
    {
        //hitbox stuff
        Collider2D[] cols = Physics2D.OverlapBoxAll(hitBox.bounds.center, hitBox.bounds.size, 0f, LayerMask.GetMask("EnemyHurtbox"));

        if (cols.Length > 0)
        {
            currentGrabAttackIndex++;
            object[] args = { damageValue, throwForce };
            cols[0].SendMessageUpwards("enemyGetThrown", args);
        }
    }

    void sweepAttack(Collider2D hitBox, int damageValue, float pushbackValue, float reelLength, float blockStunLength, AudioClip hitSoundEffect, AudioClip _blockSoundEffect)
    {
        string nameOfPreviousCol = "null";
        canCombo = true;
        //hitbox stuff
        Collider2D[] cols = Physics2D.OverlapBoxAll(hitBox.bounds.center, hitBox.bounds.size, 0f, LayerMask.GetMask("EnemyHurtbox"));

        if (cols.Length > 0)
        {
            // canCombo = true;
            // attackHasAlreadyHit = true;
            // playSoundEffect(soundEffect);
            // HS.stop();
            foreach (Collider2D c in cols)
            {
                if (!string.Equals(c.transform.parent.name, nameOfPreviousCol))
                {
                    object[] args = { damageValue, transform.localScale.x * pushbackValue, reelLength, blockStunLength, hitSoundEffect, _blockSoundEffect };
                    c.SendMessageUpwards("enemyGetSweeped", args);
                }
                nameOfPreviousCol = c.transform.parent.name;
            }
        }
    }

    //method for punching
    void punch()
    {
        if (!attackHasAlreadyHit)
        {
            attack(punchHitBox, punchDamageValue, punchPushbackValue, punchReelLength, punchBlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }
    void punchAC2()
    {
        if (!attackHasAlreadyHit)
        {
            attack(punchAC2HitBox, punchAC2DamageValue, punchAC2PushbackValue, punchAC2ReelLength, punchAC2BlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }
    void punchAC3()
    {
        if (!attackHasAlreadyHit)
        {
            attack(punchAC3HitBox, punchAC3DamageValue, punchAC3PushbackValue, punchAC3ReelLength, punchAC3BlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }
    

    void playSoundEffect(AudioClip src)
    {
        audioPlayer.clip = src;
        audioPlayer.Play();
    }

    void jumpingPunch()
    {
        if (!attackHasAlreadyHit)
        {
            attack(jumpingPunchHitBox, jumpingPunchDamageValue, jumpingPunchPushbackValue, jumpingPunchReelLength, jumpingPunchBlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }

    void kick()
    {
        if (!attackHasAlreadyHit)
        {
            attack(kickHitBox, kickDamageValue, kickPushbackValue, kickReelLength, kickBlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }
 void kickAC2()
    {
        if (!attackHasAlreadyHit)
        {
            attack(kickAC2HitBox, kickAC2DamageValue, kickAC2PushbackValue, kickAC2ReelLength, kickAC2BlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }
      void kickAC3()
    {
        if (!attackHasAlreadyHit)
        {
            attack(kickAC3HitBox, kickAC3DamageValue, kickAC3PushbackValue, kickAC3ReelLength, kickAC3BlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }
    void jumpingKick()
    {
        if (!attackHasAlreadyHit)
        {
            attack(jumpingKickHitBox, jumpingKickDamageValue, jumpingKickPushbackValue, jumpingKickReelLength, jumpingKickBlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }

    void sweep()
    {
        if (!attackHasAlreadyHit)
        {
            sweepAttack(sweepHitBox, sweepDamageValue, sweepPushbackValue, 0, sweepBlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }

    void uppercut()
    {
        if (uppercutHitBox == null) {
            Debug.Log($"{this.name}: uppercutHitBox is null!");
            return;
        }
        if (!attackHasAlreadyHit)
        {
            attack(uppercutHitBox, uppercutDamageValue, uppercutPushbackValue, uppercutReelLength, uppercutBlockStunLength, impactSoundEffect, blockSoundEffect);
        }
    }

    void grabAttackStatePunch()
    {
        if (grabAttackStatePunchHitBox == null) {
            Debug.LogError($"{this.name}: grabAttackStatePunchHitBox is null!");
            return;
        }
        if (!attackHasAlreadyHit)
        {
            grabAttack(grabAttackStatePunchHitBox, grabAttackStatePunchDamageValue, impactSoundEffect);
        }
    }

    void grabAttackStateKick()
    {
        if (grabAttackStateKickHitBox == null) {
            Debug.LogError($"{this.name}: grabAttackStateKickHitBox is null!");
            return;
        }
        if (!attackHasAlreadyHit)
        {
            grabAttack(grabAttackStateKickHitBox, grabAttackStateKickDamageValue, impactSoundEffect);
        }
    }

    void grabAttackStateThrow()
    {
        if (grabAttackStateThrowHitBox == null) {
            Debug.LogError($"{this.name}: grabAttackStateThrowHitBox is null!");
            return;
        }
        if (!attackHasAlreadyHit)
        {
            grabThrowAttack(grabAttackStateThrowHitBox, grabAttackStateThrowDamageValue, grabAttackStateThrowForce);
        }
    }

    void grab()
    {
        if (!attackHasAlreadyHit)
        {
            grabHitCheck(throwHitBox, impactSoundEffect);
        }
    }

    void resetAttackHasAlreadyHit()
    {
        attackHasAlreadyHit = false;
    }

    public void punchEnter() {
        attackHasAlreadyHit = false;
        isAbleToAct = false;
        playSoundEffect(punchSoundEffect);
    }

    public void punchExit() {
        isAbleToAct = true;
        canCombo = false;
        attackHasAlreadyHit = false;
    }

    public void punchAC3Enter() {
        attackHasAlreadyHit = false;
        playSoundEffect(punchSoundEffect);
    }

    public void jumpingPunchEnter() {
        attackHasAlreadyHit = false;
        isAbleToAct = false;
        playSoundEffect(punchSoundEffect);
    }

    public void uppercutEnter() {
        attackHasAlreadyHit = false;
        isAbleToAct = false;
        playSoundEffect(punchSoundEffect);
    }

    public void kickEnter() {
        attackHasAlreadyHit = false;
        isAbleToAct = false;
        playSoundEffect(kickSoundEffect);
    }

    public void kickExit() {
        isAbleToAct = true;
        canCombo = false;
        attackHasAlreadyHit = false;
    }

    public void kickACEnter() {
        attackHasAlreadyHit = false;
        playSoundEffect(kickSoundEffect);
    }

    public void jumpingKickEnter() {
        attackHasAlreadyHit = false;
        isAbleToAct = false;
        playSoundEffect(kickSoundEffect);
    }

    public void sweepEnter() {
        attackHasAlreadyHit = false;
        isAbleToAct = false;
        playSoundEffect(kickSoundEffect);
    }

    public void forwardDashExit() {
        isAbleToAct = true;
        isForwardDashing = false;
    }
    public void backDashExit() {
        isAbleToAct = true;
        isBackDashing = false;
    }

    public void forwardDashEnter() {
        isAbleToAct = false;
        invincibilityEnter();
    }

    public void backDashEnter() {
        isAbleToAct = false;
    }

    public void invincibilityEnter() {
        lowerHurtbox.gameObject.SetActive(false);
        upperHurtbox.gameObject.SetActive(false);
    }

    public void invincibilityExit() {
        lowerHurtbox.gameObject.SetActive(true);
        upperHurtbox.gameObject.SetActive(true);
    }

    public void knockDownEnter() {
        lowerHurtbox.gameObject.SetActive(false);
        upperHurtbox.gameObject.SetActive(false);
        isAbleToAct = false;
        isBeingGrabbed = false;
    }

    public void knockDownExit() {
        lowerHurtbox.gameObject.SetActive(true);
        upperHurtbox.gameObject.SetActive(true);
        isAbleToAct = true;
        anim.SetBool("isKnockedDown", false);
    }

    public void grabAttackStateEnter() {
        isAbleToAct = true;
    }

    public void grabAttackStateExit() {
        isInGrabAttackState = false;
        isAbleToAct = true;
        currentGrabAttackIndex = 0;
        anim.SetBool("isInGrabAttackState", false);
        currentlyGrabbedEnemy.SendMessage("grabStateExit");
        currentlyGrabbedEnemy = null;
    }

    public void grabAttackPunchEnter() {
        isAbleToAct = false;
        canCombo = false;
        playSoundEffect(punchSoundEffect);
    }

    public void grabAttackPunchExit() {
        isAbleToAct = true;
    }

    public void grabAttackKickEnter() {
        isAbleToAct = false;
        canCombo = false;
        playSoundEffect(kickSoundEffect);
    }

    public void grabAttackKickExit() {
        isAbleToAct = true;
    }

    public void grabAttackThrowEnter() {
        if (throwSoundEffect == null) {
            Debug.LogError($"{this.name}: throwSoundEffect is null!");
            return;
        }
        isAbleToAct = false;
        canCombo = false;
        playSoundEffect(throwSoundEffect);
    }

    public void grabAttackThrowExit() {
        setAllBoolAnimParametersToFalse(anim);
        isInGrabAttackState = false;
        isAbleToAct = true;
    }

  void setAllBoolAnimParametersToFalse(Animator _anim) {
    foreach (AnimatorControllerParameter parameter in _anim.parameters)
    {
      string paramType = parameter.type.ToString();
      string boolType = AnimatorControllerParameterType.Bool.ToString();
      if (paramType == boolType) {
        _anim.SetBool(parameter.name, false);
      }
    }
  }

    //ground check
    void OnCollisionStay2D(Collision2D col2D)
    {
        if ((col2D.collider.bounds.extents.y + col2D.gameObject.transform.position.y + groundCollisionOffset < transform.position.y - lowerHitBox.bounds.extents.y) && col2D.gameObject.layer == LayerMask.NameToLayer("Ledge"))
        {
            isLedgeVaulting = true;
        }
    }
}
