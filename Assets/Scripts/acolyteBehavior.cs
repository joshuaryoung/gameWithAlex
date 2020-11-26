using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class acolyteBehavior : MonoBehaviour
{
  //Raycast class;
  Collider2D hit;
  Rigidbody2D RB2D;

  public GameObject playerObject;
  PlayerInputScript PIS;
  public int startHealth;
  public int currentHealth;
  public float invincibilityCooldownPeriod;
  public float invincibilityCooldownCurrent;
  public int invincibilityCooldownFlash = 0;
  public float enemyHorizontalSpeed;
  public SpriteRenderer spriteR;
  public Color spriteColor;
  public Collider2D punchHitBox;
  public int punchDamageValue;
  public float punchPushbackOnHit;
  public float punchPushbackOnBlock;
  public bool canAttack;
  Animator acolyteAnim;
  public Animator hitSparkAnimator;
  public float currentReelLengthCooldown;
  public float pushBackDistance;
  public float punchReelLength;
  public bool infiniteHealth;
  public bool isInFootsiesRange;
  public bool isNotInAnimation;
  public int attackDecisionRNG;
  public float bobAndWeaveRNG;
  public float bobAndWeaveRNGMin;
  public float bobAndWeaveRNGMax;
  public float bobAndWeaveDeadZoneMin;
  public float bobAndWeaveDeadZoneMax;
  public byte attackDecisionRNGMin;
  public byte attackDecisionRNGMax;
  public byte lightPunchRNGMin;
  public byte lightPunchRNGMax;
  public byte heavyPunchRNGMin;
  public byte heavyPunchRNGMax;
  public byte bobAndWeaveRNGDecisionMin;
  public byte bobAndWeaveRNGDecisionMax;
  public float actualMoveDistance;
  public AudioSource audioSrc;
  public AudioClip punchSoundEffect;
  public float flipCoolDown = 0;
  public float flipCoolDownMax;
  public Collider2D lowerHurtbox;
  public AIBlockerScript AIBS;
  public bool isBeingGrabbed = false;
  public bool attackHasAlreadyHit = false;
  public CurrentlyVisableObjects CVO;

  // Use this for initialization
  void Start()
  {
    PIS = GameObject.Find("PlayerCharacter").GetComponent<PlayerInputScript>();
    currentHealth = startHealth;
    spriteR = GetComponent<SpriteRenderer>();
    acolyteAnim = GetComponent<Animator>();
    spriteColor = spriteR.color;
    invincibilityCooldownCurrent = 0;
    RB2D = GetComponent<Rigidbody2D>();
    audioSrc = GetComponentInParent<AudioSource>();
    if (AIBS == null) {
      AIBS = GetComponentInChildren<AIBlockerScript>();
    }
    if (CVO == null) {
      CVO = FindObjectOfType<CurrentlyVisableObjects>();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (lowerHurtbox == null) {
      Debug.LogError("lowerHurtbox is null!");
      return;
    }
    if (hitSparkAnimator == null) {
      Debug.LogError("hitSparkAnimator is null!");
      return;
    }
    if (CVO == null)
    {
        Debug.LogError("CVO script not assigned!");
        return;
    }
    isNotInAnimation = acolyteAnim.GetBool("isReeling") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isHeavyPunching") == false && !isBeingGrabbed;

    // Is Visible to camera?
    if (spriteR.isVisible) {
      CVO.addObject(gameObject);
    } else {
      CVO.removeObject(gameObject);
    }
    // Footsies Stuff
    if ((isInFootsiesRange || bobAndWeaveRNG != 0) && isNotInAnimation && !AIBS.isCollidingWithAIBlocker)
    {
      if (bobAndWeaveRNG == 0)
      {
        attackDecisionRNG = UnityEngine.Random.Range(attackDecisionRNGMin, attackDecisionRNGMax);
      }
      // See if col2D's x is within range of the enemy's

      if (attackDecisionRNG >= lightPunchRNGMin && attackDecisionRNG <= lightPunchRNGMax && canAttack)
      {
        canAttack = false;
        acolyteAnim.SetBool("isLightPunching", true);
        audioSrc.clip = punchSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      }
      else if (attackDecisionRNG >= heavyPunchRNGMin && attackDecisionRNG <= heavyPunchRNGMax && canAttack)
      {
        canAttack = false;
        acolyteAnim.SetBool("isHeavyPunching", true);
        audioSrc.clip = punchSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      }
      else if (attackDecisionRNG >= bobAndWeaveRNGDecisionMin && attackDecisionRNG <= bobAndWeaveRNGDecisionMax && isNotInAnimation)
      {
        if (bobAndWeaveRNG == 0)
        {
          bobAndWeaveRNG = UnityEngine.Random.Range(bobAndWeaveRNGMin, bobAndWeaveRNGMax);
          if (bobAndWeaveRNG >= bobAndWeaveDeadZoneMin && bobAndWeaveRNG <= bobAndWeaveDeadZoneMax) {
            float distanceToMin = Math.Abs(bobAndWeaveRNG - bobAndWeaveDeadZoneMin);
            float distanceToMax = Math.Abs(bobAndWeaveDeadZoneMax - bobAndWeaveRNG);
            if (distanceToMin < distanceToMax) {
              bobAndWeaveRNG = UnityEngine.Random.Range(bobAndWeaveRNGMin, bobAndWeaveDeadZoneMin);
              // Debug.Log($"in dead zone. Closer to min. bobAndWeaveRng: {bobAndWeaveRNG} min: {bobAndWeaveDeadZoneMin} max: {bobAndWeaveDeadZoneMax} distanceToMin: {distanceToMin} distanceToMax: {distanceToMax}");
            } else {
              bobAndWeaveRNG = UnityEngine.Random.Range(bobAndWeaveDeadZoneMax, bobAndWeaveRNGMax);
              // Debug.Log($"in dead zone. Closer to max. bobAndWeaveRng: {bobAndWeaveRNG} min: {bobAndWeaveDeadZoneMin} max: {bobAndWeaveDeadZoneMax} distanceToMin: {distanceToMin} distanceToMax: {distanceToMax}");
            }
          }
        }
        actualMoveDistance = Mathf.Clamp(bobAndWeaveRNG, -1 * enemyHorizontalSpeed, enemyHorizontalSpeed);
        transform.position += new Vector3(actualMoveDistance * transform.localScale.x,
          0,
          0
        ) * Time.deltaTime;
        bobAndWeaveRNG -= actualMoveDistance;
      }
    }
    else if (!isInFootsiesRange && isNotInAnimation)
    {
      transform.position += new Vector3(enemyHorizontalSpeed * transform.localScale.x,
        0,
        0
      ) * Time.deltaTime;
    }
    else
    {
      if (canAttack)
      {
        acolyteAnim.SetBool("isLightPunching", false);
        acolyteAnim.SetBool("isHeavyPunching", false);
      }
    }

    if (currentReelLengthCooldown > 0)
    {
      currentReelLengthCooldown -= Time.deltaTime;
      if (currentReelLengthCooldown <= 0)
      {
        reelStateExit();
      }
    }

    if (invincibilityCooldownCurrent > 0)
    {
      spriteColor.a = invincibilityCooldownFlash;
      invincibilityCooldownFlash = (invincibilityCooldownFlash * -1) + 1;
      spriteR.color = spriteColor;
      invincibilityCooldownCurrent -= Time.deltaTime;
      if (invincibilityCooldownCurrent <= 0)
      {
        spriteColor.a = (float)255;
        spriteR.color = spriteColor;
      }
    }

    if (flipCoolDown > 0) {
      flipCoolDown -= Time.deltaTime;
    }
  }

  void disableIsPunching()
  {
    acolyteAnim.SetBool("isLightPunching", false);
    acolyteAnim.SetBool("isHeavyPunching", false);
  }

  void resetAttackHasAlreadyHit() {
    attackHasAlreadyHit = false;
  }
  void flip()
  {
    Vector2 flipLocalScale = gameObject.transform.localScale;
    flipLocalScale.x *= -1;
    transform.localScale = flipLocalScale;
    flipCoolDown = flipCoolDownMax;
  }

  void pushBack(float pushBackValue)
  {
    RB2D.velocity = (new Vector2(RB2D.velocity.x + pushBackValue, RB2D.velocity.y));
  }

  //method for punching
  public void punch()
  {
    if (attackHasAlreadyHit) {
      return;
    }
    //hitbox stuff
    Collider2D[] cols = Physics2D.OverlapBoxAll(punchHitBox.bounds.center, punchHitBox.bounds.size, 0f, LayerMask.GetMask("Player"));

    if (cols.Length > 0)
    {
      foreach (Collider2D c in cols)
      {
        attackHasAlreadyHit = true;
        object[] args = { punchDamageValue, transform.localScale.x * (PIS.blockPressed ? punchPushbackOnBlock : punchPushbackOnHit), punchReelLength };
        c.SendMessageUpwards("playerTakeDamage", args);
      }
    }
  }

  public void enemyTakeDamage(object[] args)
  {
    attackHasAlreadyHit = false;
    int damage = (int)args[0];
    float pushBackDistance = (float)args[1];
    float reelLength = (float)args[2];

    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0)
    {
      if (!infiniteHealth)
        currentHealth -= damage;
      hitSparkAnimator.SetBool("isActive", true);
      pushBack(pushBackDistance);
      reelStateEnter(reelLength);
      if (currentHealth <= 0)
        enemyDeath();
    }
  }
  public void enemyGetGrabbed(object[] args)
  {
    int damage = (int)args[0];

    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0)
    {
      if (!infiniteHealth)
        currentHealth -= damage;
      grabStateEnter();
      if (currentHealth <= 0)
        enemyDeath();
    }
  }

  //for the state that occurs right after receiving damage where acolyte is combo-able
  public void reelStateEnter(float reelLength)
  {
    currentReelLengthCooldown = reelLength;
    if (invincibilityCooldownCurrent <= 0)
    {
      foreach (AnimatorControllerParameter parameter in acolyteAnim.parameters)
      {
        acolyteAnim.SetBool(parameter.name, false);
      }
      acolyteAnim.SetBool("isReeling", true);
      acolyteAnim.Play("reel", 0, 0.0f);
    }
  }
  public void grabStateEnter()
  {
    if (invincibilityCooldownCurrent <= 0)
    {
      foreach (AnimatorControllerParameter parameter in acolyteAnim.parameters)
      {
        acolyteAnim.SetBool(parameter.name, false);
      }
      isBeingGrabbed = true;
      acolyteAnim.SetBool("isBeingGrabbed", true);
      acolyteAnim.Play("BeingGrabbed", 0, 0.0f);
    }
  }

  public void reelStateExit()
  {
    acolyteAnim.SetBool("isReeling", false);
    invincibilityCooldownCurrent = invincibilityCooldownPeriod;
  }
  public void grabStateExit()
  {
    isBeingGrabbed = false;
    acolyteAnim.SetBool("isBeingGrabbed", false);
    invincibilityCooldownCurrent = invincibilityCooldownPeriod;
  }

  public void enemyDeath()
  {
    gameObject.SetActive(false);
  }

  public void resetCanAttack()
  {
    canAttack = true;
  }

  void OnCollisionStay2D(Collision2D col2D)
  {
    if (lowerHurtbox == null) {
      Debug.LogError("lowerHurtbox is null!");
      return;
    }
    /*if (hit.GetComponent<Collider>().tag == "Player") {
			playerObject.GetComponent<PlayerHealth> ().playerTakeDamage (1);
		}*/
    isNotInAnimation = acolyteAnim.GetBool("isHeavyPunching") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isReeling") == false;
    float collisionTop = col2D.transform.position.y + col2D.collider.bounds.extents.y;
    float characterBottom = transform.position.y - lowerHurtbox.bounds.extents.y;
    bool isWall = col2D.gameObject.layer == LayerMask.NameToLayer("Wall");
    bool isEnemy = col2D.gameObject.layer == LayerMask.NameToLayer("EnemyLayer");
    bool isPlayer = col2D.gameObject.tag == "PlayerCharacter";
    bool isNotOnIgnoreRaycastLayer = col2D.gameObject.layer != 2;
    bool isFacingTowardsWall = (col2D.gameObject.transform.localPosition.x - transform.localPosition.x) * transform.localScale.x > 0;
    bool facingOppositeDirections = col2D.transform.localScale.x * transform.localScale.x < 0;

    if (isNotInAnimation && isNotOnIgnoreRaycastLayer && flipCoolDown <= 0 && !isInFootsiesRange)
    {
      if (isWall && isFacingTowardsWall) {
        flip();
      } else if (isEnemy && facingOppositeDirections) {
        flip();
      }
    }
  }
}
