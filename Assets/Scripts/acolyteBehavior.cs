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
  public float punchReelLength;
  public float punchBlockStunLength;
  public float punchPushbackOnHit;
  public float punchPushbackOnBlock;
  public Collider2D headbuttHitBox;
  public int headbuttDamageValue;
  public float headbuttReelLength;
  public float headbuttBlockStunLength;
  public float headbuttPushbackOnHit;
  public float headbuttPushbackOnBlock;
  public Collider2D attack3HitBox;
  public int attack3DamageValue;
  public float attack3ReelLength;
  public float attack3BlockStunLength;
  public float attack3PushbackOnHit;
  public float attack3PushbackOnBlock;
  public bool canAttack = true;
  public bool isBlocking;
  Animator acolyteAnim;
  public Animator hitSparkAnimator;
  public Animator blockSparkAnimator;
  public float currentReelLengthCooldown;
  public float pushBackDistance;
  public bool infiniteHealth;
  public footsies.range currentFootsiesRange;
  public bool isNotInAnimation;
  public int attackDecisionRNG;
  public byte attackDecisionRNGMin;
  public byte attackDecisionRNGMax;
  public byte lightPunchRNGMin;
  public byte lightPunchNearRNGMin;
  public byte lightPunchMidRNGMin;
  public byte lightPunchFarRNGMin;
  public byte lightPunchRNGMax;
  public byte lightPunchNearRNGMax;
  public byte lightPunchMidRNGMax;
  public byte lightPunchFarRNGMax;
  public byte headbuttPunchRNGMin;
  public byte headbuttPunchNearRNGMin;
  public byte headbuttPunchMidRNGMin;
  public byte headbuttPunchFarRNGMin;
  public byte headbuttPunchRNGMax;
  public byte headbuttPunchNearRNGMax;
  public byte headbuttPunchMidRNGMax;
  public byte headbuttPunchFarRNGMax;
  public byte attack3RNGMin;
  public byte attack3NearRNGMin;
  public byte attack3MidRNGMin;
  public byte attack3FarRNGMin;
  public byte attack3RNGMax;
  public byte attack3NearRNGMax;
  public byte attack3MidRNGMax;
  public byte attack3FarRNGMax;
  public byte blockRNGMin;
  public byte blockNearRNGMin;
  public byte blockMidRNGMin;
  public byte blockFarRNGMin;
  public byte blockRNGMax;
  public byte blockNearRNGMax;
  public byte blockMidRNGMax;
  public byte blockFarRNGMax;
  public byte bobAndWeaveRNGDecisionMin;
  public byte bobAndWeaveNearRNGDecisionMin;
  public byte bobAndWeaveMidRNGDecisionMin;
  public byte bobAndWeaveFarRNGDecisionMin;
  public byte bobAndWeaveRNGDecisionMax;
  public byte bobAndWeaveNearRNGDecisionMax;
  public byte bobAndWeaveMidRNGDecisionMax;
  public byte bobAndWeaveFarRNGDecisionMax;
  public float bobAndWeaveDistanceRNG;
  public float bobAndWeaveDistanceRNGMin;
  public float bobAndWeaveDistanceNearRNGMin;
  public float bobAndWeaveDistanceMidRNGMin;
  public float bobAndWeaveDistanceFarRNGMin;
  public float bobAndWeaveDistanceRNGMax;
  public float bobAndWeaveDistanceNearRNGMax;
  public float bobAndWeaveDistanceMidRNGMax;
  public float bobAndWeaveDistanceFarRNGMax;
  public float bobAndWeaveDeadZoneMin;
  public float bobAndWeaveDeadZoneNearMin;
  public float bobAndWeaveDeadZoneMidMin;
  public float bobAndWeaveDeadZoneFarMin;
  public float bobAndWeaveDeadZoneMax;
  public float bobAndWeaveDeadZoneNearMax;
  public float bobAndWeaveDeadZoneMidMax;
  public float bobAndWeaveDeadZoneFarMax;
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
  public bool isDead = false;
  public bool isDying = false;

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
    if (blockSparkAnimator == null) {
      Debug.LogError("blockSparkAnimator is null!");
      return;
    }
    if (CVO == null)
    {
        Debug.LogError("CVO script not assigned!");
        return;
    }
    if (isDead || isDying) {
      return;
    }
    isNotInAnimation = acolyteAnim.GetBool("isReeling") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isHeadbutting") == false && acolyteAnim.GetBool("isBlocking") == false && acolyteAnim.GetBool("isBlockingAnAttack") == false && !isBeingGrabbed;

    // Is Visible to camera?
    if (spriteR.isVisible) {
      CVO.addObject(gameObject);
    } else {
      CVO.removeObject(gameObject);
    }
    // Footsies Stuff
    if ((currentFootsiesRange != footsies.range.None || bobAndWeaveDistanceRNG != 0) && isNotInAnimation && !AIBS.isCollidingWithAIBlocker)
    {
      footsiesValsForCurrentRange();
      if (bobAndWeaveDistanceRNG == 0)
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
      else if (attackDecisionRNG >= headbuttPunchRNGMin && attackDecisionRNG <= headbuttPunchRNGMax && canAttack)
      {
        canAttack = false;
        acolyteAnim.SetBool("isHeadbutting", true);
        audioSrc.clip = punchSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      }
      else if (attackDecisionRNG >= attack3RNGMin && attackDecisionRNG <= attack3RNGMax && canAttack)
      {
        canAttack = false;
        acolyteAnim.SetBool("isAttack3ing", true);
        audioSrc.clip = punchSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      }
      else if (attackDecisionRNG >= blockRNGMin && attackDecisionRNG <= blockRNGMax && canAttack)
      {
        canAttack = false;
        isBlocking = true;
        acolyteAnim.SetBool("isBlocking", true);
      }
      else if (attackDecisionRNG >= bobAndWeaveRNGDecisionMin && attackDecisionRNG <= bobAndWeaveRNGDecisionMax && isNotInAnimation)
      {
        if (bobAndWeaveDistanceRNG == 0)
        {
          bobAndWeaveDistanceRNG = UnityEngine.Random.Range(bobAndWeaveDistanceRNGMin, bobAndWeaveDistanceRNGMax);
          if (bobAndWeaveDistanceRNG >= bobAndWeaveDeadZoneMin && bobAndWeaveDistanceRNG <= bobAndWeaveDeadZoneMax) {
            float distanceToMin = Math.Abs(bobAndWeaveDistanceRNG - bobAndWeaveDeadZoneMin);
            float distanceToMax = Math.Abs(bobAndWeaveDeadZoneMax - bobAndWeaveDistanceRNG);
            if (distanceToMin < distanceToMax) {
              bobAndWeaveDistanceRNG = UnityEngine.Random.Range(bobAndWeaveDistanceRNGMin, bobAndWeaveDeadZoneMin);
              // Debug.Log($"in dead zone. Closer to min. bobAndWeaveRng: {bobAndWeaveRNG} min: {bobAndWeaveDeadZoneMin} max: {bobAndWeaveDeadZoneMax} distanceToMin: {distanceToMin} distanceToMax: {distanceToMax}");
            } else {
              bobAndWeaveDistanceRNG = UnityEngine.Random.Range(bobAndWeaveDeadZoneMax, bobAndWeaveDistanceRNGMax);
              // Debug.Log($"in dead zone. Closer to max. bobAndWeaveRng: {bobAndWeaveDistanceRNG} min: {bobAndWeaveDeadZoneMin} max: {bobAndWeaveDeadZoneMax} distanceToMin: {distanceToMin} distanceToMax: {distanceToMax}");
            }
          }
        }
        actualMoveDistance = Mathf.Clamp(bobAndWeaveDistanceRNG, -1 * enemyHorizontalSpeed, enemyHorizontalSpeed);
        transform.position += new Vector3(actualMoveDistance * transform.localScale.x,
          0,
          0
        ) * Time.deltaTime;
        bobAndWeaveDistanceRNG -= actualMoveDistance;
      }
    }
    else if (currentFootsiesRange == footsies.range.None && isNotInAnimation)
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
        acolyteAnim.SetBool("isHeadbutting", false);
        acolyteAnim.SetBool("isAttack3ing", false);
        acolyteAnim.SetBool("isBlocking", false);
        acolyteAnim.SetBool("isBlockingAnAttack", false);
        isBlocking = false;
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

  void footsiesValsForCurrentRange() {
    switch (currentFootsiesRange)
    {
        case footsies.range.Near:
          lightPunchRNGMin = lightPunchNearRNGMin;
          lightPunchRNGMax = lightPunchNearRNGMax;
          headbuttPunchRNGMin = headbuttPunchNearRNGMin;
          headbuttPunchRNGMax = headbuttPunchNearRNGMax;
          attack3RNGMin = attack3NearRNGMin;
          attack3RNGMax = attack3NearRNGMax;
          blockRNGMin = blockNearRNGMin;
          blockRNGMax = blockNearRNGMax;
          bobAndWeaveRNGDecisionMin = bobAndWeaveNearRNGDecisionMin;
          bobAndWeaveRNGDecisionMax = bobAndWeaveNearRNGDecisionMax;
          bobAndWeaveDistanceRNGMin = bobAndWeaveDistanceNearRNGMin;
          bobAndWeaveDistanceRNGMax = bobAndWeaveDistanceNearRNGMax;
          bobAndWeaveDeadZoneMin = bobAndWeaveDeadZoneNearMin;
          bobAndWeaveDeadZoneMax = bobAndWeaveDeadZoneNearMax;
          break;

        case footsies.range.Mid:
          lightPunchRNGMin = lightPunchMidRNGMin;
          lightPunchRNGMax = lightPunchMidRNGMax;
          headbuttPunchRNGMin = headbuttPunchMidRNGMin;
          headbuttPunchRNGMax = headbuttPunchMidRNGMax;
          attack3RNGMin = attack3MidRNGMin;
          attack3RNGMax = attack3MidRNGMax;
          blockRNGMin = blockMidRNGMin;
          blockRNGMax = blockMidRNGMax;
          bobAndWeaveRNGDecisionMin = bobAndWeaveMidRNGDecisionMin;
          bobAndWeaveRNGDecisionMax = bobAndWeaveMidRNGDecisionMax;
          bobAndWeaveDistanceRNGMin = bobAndWeaveDistanceMidRNGMin;
          bobAndWeaveDistanceRNGMax = bobAndWeaveDistanceMidRNGMax;
          bobAndWeaveDeadZoneMin = bobAndWeaveDeadZoneMidMin;
          bobAndWeaveDeadZoneMax = bobAndWeaveDeadZoneMidMax;
          break;

        case footsies.range.Far:
          lightPunchRNGMin = lightPunchFarRNGMin;
          lightPunchRNGMax = lightPunchFarRNGMax;
          headbuttPunchRNGMin = headbuttPunchFarRNGMin;
          headbuttPunchRNGMax = headbuttPunchFarRNGMax;
          attack3RNGMin = attack3FarRNGMin;
          attack3RNGMax = attack3FarRNGMax;
          blockRNGMin = blockFarRNGMin;
          blockRNGMax = blockFarRNGMax;
          bobAndWeaveRNGDecisionMin = bobAndWeaveFarRNGDecisionMin;
          bobAndWeaveRNGDecisionMax = bobAndWeaveFarRNGDecisionMax;
          bobAndWeaveDistanceRNGMin = bobAndWeaveDistanceFarRNGMin;
          bobAndWeaveDistanceRNGMax = bobAndWeaveDistanceFarRNGMax;
          bobAndWeaveDeadZoneMin = bobAndWeaveDeadZoneFarMin;
          bobAndWeaveDeadZoneMax = bobAndWeaveDeadZoneFarMax;
          break;

        default:
          break;
    }
  }

  void disableIsPunching()
  {
    acolyteAnim.SetBool("isLightPunching", false);
    acolyteAnim.SetBool("isHeadbutting", false);
    acolyteAnim.SetBool("isAttack3ing", false);
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

  void attack(Collider2D hitBox, int damageValue, float pushbackOnBlock, float pushbackOnHit, float reelLength, float blockStunLength) {
    if (attackHasAlreadyHit) {
      return;
    }
    //hitbox stuff
    Collider2D[] cols = Physics2D.OverlapBoxAll(punchHitBox.bounds.center, punchHitBox.bounds.size, 0f, LayerMask.GetMask("PlayerHurtbox"));

    if (cols.Length > 0)
    {
      foreach (Collider2D c in cols)
      {
        attackHasAlreadyHit = true;
        object[] args = { damageValue, transform.localScale.x * (PIS.isBlocking ? pushbackOnBlock : pushbackOnHit), reelLength, blockStunLength };
        c.SendMessageUpwards("playerTakeDamage", args);
      }
    }
  }

  //method for punching
  public void punch()
  {
    attack(punchHitBox, punchDamageValue, punchPushbackOnBlock, punchPushbackOnHit, punchReelLength, punchBlockStunLength);
  }
  public void headbutt()
  {
    attack(headbuttHitBox, headbuttDamageValue, headbuttPushbackOnBlock, headbuttPushbackOnHit, headbuttReelLength, headbuttBlockStunLength);
  }
  public void attack3()
  {
    attack(attack3HitBox, attack3DamageValue, attack3PushbackOnBlock, attack3PushbackOnHit, attack3ReelLength, attack3BlockStunLength);
  }

  public void attack3Exit() {
    disableIsPunching();
    canAttack = true;
    attackHasAlreadyHit = false;
  }

  public void blockEnter() {
    canAttack = false;
  }

  public void blockExit() {
    isBlocking = false;
    acolyteAnim.SetBool("isBlocking", false);
    canAttack = true;
  }

  public void blockStunEnter() {
    canAttack = false;
    foreach (AnimatorControllerParameter parameter in acolyteAnim.parameters)
    {
      acolyteAnim.SetBool(parameter.name, false);
    }
    acolyteAnim.SetBool("isBlockingAnAttack", true);
  }

  public void enemyTakeDamage(object[] args)
  {
    attackHasAlreadyHit = false;
    int damage = (int)args[0];
    float pushBackDistance = (float)args[1];
    float reelLength = (float)args[2];
    float blockStunLength = (float)args[3];
    AudioClip hitSoundEffect = (AudioClip)args[4];
    AudioClip blockSoundEffect = (AudioClip)args[5];

    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0)
    {
      if (isBlocking) {
        currentReelLengthCooldown = blockStunLength;
        blockSparkAnimator.SetBool("isActive", true);
        acolyteAnim.SetBool("isBlockingAnAttack", true);
        pushBack(blockStunLength);
        audioSrc.clip = blockSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      } else {
        if (!infiniteHealth)
          currentHealth -= damage;
        hitSparkAnimator.SetBool("isActive", true);
        pushBack(pushBackDistance);
        audioSrc.clip = hitSoundEffect;
        // audioSrc.enabled = true;
        audioSrc.Play();
        reelStateEnter(reelLength);
        if (currentHealth <= 0)
          enemyDeath();
      }
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
    acolyteAnim.SetBool("isBlocking", false);
    acolyteAnim.SetBool("isBlockingAnAttack", false);
    isBlocking = false;
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
    CVO.removeObject(gameObject);
    isDying = true;
    acolyteAnim.SetBool("isDying", true);
    // gameObject.SetActive(false);
  }

  public void setDeathVars() {
    isDead = true;
    acolyteAnim.SetBool("isDead", true);
  }

  public void resetCanAttack()
  {
    canAttack = true;
  }
  public void setCanAttackFalse()
  {
    canAttack = false;
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
    isNotInAnimation = acolyteAnim.GetBool("isHeadbutting") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isReeling") == false;
    float collisionTop = col2D.transform.position.y + col2D.collider.bounds.extents.y;
    float characterBottom = transform.position.y - lowerHurtbox.bounds.extents.y;
    bool isWall = col2D.gameObject.layer == LayerMask.NameToLayer("Wall");
    bool isEnemy = col2D.gameObject.layer == LayerMask.NameToLayer("EnemyLayer");
    bool isPlayer = col2D.gameObject.tag == "PlayerCharacter";
    bool isNotOnIgnoreRaycastLayer = col2D.gameObject.layer != 2;
    bool isFacingTowardsWall = (col2D.gameObject.transform.localPosition.x - transform.localPosition.x) * transform.localScale.x > 0;
    bool facingOppositeDirections = col2D.transform.localScale.x * transform.localScale.x < 0;

    if (isNotInAnimation && isNotOnIgnoreRaycastLayer && flipCoolDown <= 0 && currentFootsiesRange == footsies.range.None)
    {
      if (isWall && isFacingTowardsWall) {
        flip();
      } else if (isEnemy && facingOppositeDirections) {
        flip();
      }
    }
  }
}
