using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WitchBehavior : MonoBehaviour
{
  //Raycast class;
  Collider2D hit;
  Rigidbody2D RB2D;

  public GameObject playerObject;
  PlayerInputScript PIS;
  public GameObject upperCollisionBoxGameObj;
  public GameObject lowerCollisionBoxGameObj;
  public int startHealth;
  public int currentHealth;
  public int throwDamage;
  public float invincibilityCooldownPeriod;
  public float invincibilityCooldownCurrent;
  public int invincibilityCooldownFlash = 0;
  public float enemyHorizontalSpeed;
  public SpriteRenderer spriteR;
  public Color spriteColor;
  public Collider2D slashHitBox;
  public Collider2D heavyPunchHitBox;
  public int slashDamageValue;
  public float slashPushbackOnHit;
  public float slashPushbackOnBlock;
  public float slashReelLength;
  public float slashBlockStunLength;
  public int heavyPunchDamageValue;
  public float heavyPunchPushbackOnHit;
  public float heavyPunchPushbackOnBlock;
  public float heavyPunchReelLength;
  public float heavyPunchBlockStunLength;
  public bool canAttack;
  public bool isBlocking;
  Animator witchAnim;
  public Animator hitSparkAnimator;
  public Animator blockSparkAnimator;
  public float currentReelLengthCooldown;
  public bool infiniteHealth;
  public footsies.range currentFootsiesRange;
  public bool isNotInAnimation;
  public bool isGrounded;
  public bool wasGroundedLastFrame;
  public int attackDecisionRNG;
  public byte attackDecisionRNGMin;
  public byte attackDecisionRNGMax;
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
  public byte slashRNGMin;
  public byte slashNearRNGMin;
  public byte slashMidRNGMin;
  public byte slashFarRNGMin;
  public byte slashRNGMax;
  public byte slashNearRNGMax;
  public byte slashMidRNGMax;
  public byte slashFarRNGMax;
  public byte attack2RNGMin;
  public byte attack2NearRNGMin;
  public byte attack2MidRNGMin;
  public byte attack2FarRNGMin;
  public byte attack2RNGMax;
  public byte attack2NearRNGMax;
  public byte attack2MidRNGMax;
  public byte attack2FarRNGMax;
  public byte blockRNGMin;
  public byte blockNearRNGMin;
  public byte blockMidRNGMin;
  public byte blockFarRNGMin;
  public byte blockRNGMax;
  public byte blockNearRNGMax;
  public byte blockMidRNGMax;
  public byte blockFarRNGMax;
  public float actualMoveDistance;
  public AudioSource audioSrc;
  public AudioClip slashSoundEffect;
  public AudioClip thudSoundEffect;
  public float flipCoolDown = 0;
  public float flipCoolDownMax;
  public Collider2D lowerHurtbox;
  public witchAIBlockerScript WAIBS;
  public bool isBeingGrabbed = false;
  public bool isBeingThrown = false;
  public bool attackHasAlreadyHit = false;
  public CurrentlyVisableObjects CVO;
  public bool isDead = false;
  public bool isDying = false;
  public bool isKnockedDown = false;

  // Use this for initialization
  
  void Start()
  {
    PIS = GameObject.Find("PlayerCharacter").GetComponent<PlayerInputScript>();
    currentHealth = startHealth;
    spriteR = GetComponent<SpriteRenderer>();
    witchAnim = GetComponent<Animator>();
    spriteColor = spriteR.color;
    invincibilityCooldownCurrent = 0;
    RB2D = GetComponent<Rigidbody2D>();
    audioSrc = GetComponentInParent<AudioSource>();
    if (WAIBS == null) {
      WAIBS = GetComponentInChildren<witchAIBlockerScript>();
    }
    if (CVO == null) {
      CVO = FindObjectOfType<CurrentlyVisableObjects>();
    }
    canAttack = true;
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
    if (WAIBS == null)
    {
        Debug.LogError("WAIBS script not assigned!");
        return;
    }
    if (upperCollisionBoxGameObj == null) {
        Debug.LogError("upperCollisionBoxGameObj is null!");
        return;
    }
    if (lowerCollisionBoxGameObj == null) {
        Debug.LogError("lowerCollisionBoxGameObj is null!");
        return;
    }
    if (isDead || isDying) {
      return;
    }
    isNotInAnimation = isNotInAnimCheck();

    // Is Visible to camera?
    if (spriteR.isVisible) {
      CVO.addObject(gameObject);
    } else {
      CVO.removeObject(gameObject);
    }
    // Footsies Stuff
    if ((currentFootsiesRange != footsies.range.None || bobAndWeaveDistanceRNG != 0) && isNotInAnimation && !WAIBS.isCollidingWithAIBlocker)
    {
      if (bobAndWeaveDistanceRNG == 0)
      {
        canAttack = true;
        attackDecisionRNG = UnityEngine.Random.Range(attackDecisionRNGMin, attackDecisionRNGMax);
      }
      // See if col2D's x is within range of the enemy's

      if (attackDecisionRNG >= slashRNGMin && attackDecisionRNG <= slashRNGMax && canAttack)
      {
        canAttack = false;
        witchAnim.SetBool("isSlashing", true);
        audioSrc.clip = slashSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      }
      else if (attackDecisionRNG >= attack2RNGMin && attackDecisionRNG <= attack2RNGMax && canAttack)
      {
        canAttack = false;
        witchAnim.SetBool("isHeavyPunching", true);
        audioSrc.clip = slashSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      }
      else if (attackDecisionRNG >= blockRNGMin && attackDecisionRNG <= blockRNGMax && canAttack)
      {
        canAttack = false;
        isBlocking = true;
        witchAnim.SetBool("isBlocking", true);
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
              // Debug.Log($"in dead zone. Closer to min. bobAndWeaveDistanceRNG: {bobAndWeaveDistanceRNG} min: {bobAndWeaveDeadZoneMin} max: {bobAndWeaveDeadZoneMax} distanceToMin: {distanceToMin} distanceToMax: {distanceToMax}");
            } else {
              bobAndWeaveDistanceRNG = UnityEngine.Random.Range(bobAndWeaveDeadZoneMax, bobAndWeaveDistanceRNGMax);
              // Debug.Log($"in dead zone. Closer to max. bobAndWeaveDistanceRNG: {bobAndWeaveDistanceRNG} min: {bobAndWeaveDeadZoneMin} max: {bobAndWeaveDeadZoneMax} distanceToMin: {distanceToMin} distanceToMax: {distanceToMax}");
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
        witchAnim.SetBool("isSlashing", false);
        witchAnim.SetBool("isHeavyPunching", false);
        witchAnim.SetBool("isBlocking", false);
        witchAnim.SetBool("isBlockingAnAttack", false);
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
        canAttack = true;
        spriteColor.a = (float)255;
        spriteR.color = spriteColor;
      }
    }

    if (flipCoolDown > 0) {
      flipCoolDown -= Time.deltaTime;
    }

    if (!wasGroundedLastFrame && isGrounded && isBeingThrown) {
      throwStateExit();
    }
    wasGroundedLastFrame = isGrounded;
  }

  void disableIsPunching()
  {
    witchAnim.SetBool("isSlashing", false);
    witchAnim.SetBool("isHeavyPunching", false);
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
    Collider2D[] cols = Physics2D.OverlapBoxAll(hitBox.bounds.center, hitBox.bounds.size, 0f, LayerMask.GetMask("PlayerHurtbox"));

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

  public void footsiesValsForCurrentRange() {
    switch (currentFootsiesRange)
    {
        case footsies.range.Near:
          slashRNGMin = slashNearRNGMin;
          slashRNGMax = slashNearRNGMax;
          attack2RNGMin = attack2NearRNGMin;
          attack2RNGMax = attack2NearRNGMax;
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
          slashRNGMin = slashMidRNGMin;
          slashRNGMax = slashMidRNGMax;
          attack2RNGMin = attack2MidRNGMin;
          attack2RNGMax = attack2MidRNGMax;
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
          slashRNGMin = slashFarRNGMin;
          slashRNGMax = slashFarRNGMax;
          attack2RNGMin = attack2FarRNGMin;
          attack2RNGMax = attack2FarRNGMax;
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

  //method for punching
  public void slash()
  {
    attack(slashHitBox, slashDamageValue, slashPushbackOnBlock, slashPushbackOnHit, slashReelLength, slashBlockStunLength);
  }
  public void heavyPunch()
  {
    if (heavyPunchHitBox == null) {
      Debug.LogError("heavyPunchHitBox is null!");
      return;
    }
    attack(heavyPunchHitBox, heavyPunchDamageValue, heavyPunchPushbackOnBlock, heavyPunchPushbackOnHit, heavyPunchReelLength, heavyPunchBlockStunLength);
  }

  public void blockEnter() {
    canAttack = false;
  }

  public void blockExit() {
    isBlocking = false;
    witchAnim.SetBool("isBlocking", false);
    canAttack = true;
  }

  public void blockStunEnter() {
    canAttack = false;
    foreach (AnimatorControllerParameter parameter in witchAnim.parameters)
    {
      witchAnim.SetBool(parameter.name, false);
    }
    witchAnim.SetBool("isBlockingAnAttack", true);
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

    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0 && !isKnockedDown)
    {
      PIS.attackHasAlreadyHit = true;
      if (isBlocking) {
        currentReelLengthCooldown = blockStunLength;
        blockSparkAnimator.SetBool("isActive", true);
        witchAnim.SetBool("isBlockingAnAttack", true);
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

  public void enemyTakeGrabAttackDamage(object[] args)
  {
    attackHasAlreadyHit = false;
    int damage = (int)args[0];
    AudioClip hitSoundEffect = (AudioClip)args[1];

    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0 && !isKnockedDown)
    {
      PIS.attackHasAlreadyHit = true;
      if (!infiniteHealth)
        currentHealth -= damage;
      hitSparkAnimator.SetBool("isActive", true);
      audioSrc.clip = hitSoundEffect;
      // audioSrc.enabled = true;
      audioSrc.Play();
      if (currentHealth <= 0) {
        PIS.grabAttackStateExit();
        enemyDeath();
      }
    }
  }
  public void enemyGetGrabbed()
  {
    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0 && !isKnockedDown)
    {
      grabStateEnter();
    }
  }
  public void enemyGetThrown(object[] args)
  {
    if (args.Length < 2) {
      Debug.LogError($"{this.name}: args.Length is less than 2!");
      return;
    }

    throwDamage = (int)args[0];
    Vector2 throwForce = (Vector2)args[1];
    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0 && !isKnockedDown)
    {
      throwStateEnter(throwForce);
    }
  }

  public void enemyGetSweeped(object[] args)
  {
    attackHasAlreadyHit = false;
    int damage = (int)args[0];
    float pushBackDistance = (float)args[1];
    float reelLength = (float)args[2];
    float blockStunLength = (float)args[3];
    AudioClip hitSoundEffect = (AudioClip)args[4];
    AudioClip blockSoundEffect = (AudioClip)args[5];

    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0 && !isKnockedDown)
    {
      PIS.attackHasAlreadyHit = true;
      if (isBlocking) {
        currentReelLengthCooldown = blockStunLength;
        blockSparkAnimator.SetBool("isActive", true);
        witchAnim.SetBool("isBlockingAnAttack", true);
        pushBack(blockStunLength);
        audioSrc.clip = blockSoundEffect;
        audioSrc.enabled = true;
        audioSrc.Play();
      } else {
        if (!infiniteHealth)
          currentHealth -= damage;
        hitSparkAnimator.SetBool("isActive", true);
        audioSrc.clip = hitSoundEffect;
        audioSrc.Play();
        knockDownEnter();
        // reelStateEnter(reelLength);
        if (currentHealth <= 0)
          enemyDeath();
      }
    }
  }

  public void knockDownEnter() {
    setAllBoolAnimParametersToFalse();
    witchAnim.SetBool("isKnockedDown", true);
  }

  public void knockDownInvincibilityEnter() {
    isKnockedDown = true;
    upperCollisionBoxGameObj.layer = LayerMask.NameToLayer("OnlyInteractsWithWallsGround");
    lowerCollisionBoxGameObj.layer = LayerMask.NameToLayer("OnlyInteractsWithWallsGround");
  }

  public void knockDownExit() {
    isKnockedDown = false;
    canAttack = true;
    witchAnim.SetBool("isKnockedDown", false);
    upperCollisionBoxGameObj.layer = LayerMask.NameToLayer("EnemyLayer");
    lowerCollisionBoxGameObj.layer = LayerMask.NameToLayer("EnemyLayer");
  }

  //for the state that occurs right after receiving damage where acolyte is combo-able
  public void reelStateEnter(float reelLength)
  {
    currentReelLengthCooldown = reelLength;
    if (invincibilityCooldownCurrent <= 0)
    {
      foreach (AnimatorControllerParameter parameter in witchAnim.parameters)
      {
        string paramType = parameter.type.ToString();
        string boolType = AnimatorControllerParameterType.Bool.ToString();
        if (paramType == boolType) {
          witchAnim.SetBool(parameter.name, false);
        }
      }
      witchAnim.SetBool("isReeling", true);
      witchAnim.Play("reel", 0, 0.0f);
    }
  }

  public void reelStateExit()
  {
    witchAnim.SetBool("isReeling", false);
    witchAnim.SetBool("isBlocking", false);
    witchAnim.SetBool("isBlockingAnAttack", false);
    isBlocking = false;
    invincibilityCooldownCurrent = invincibilityCooldownPeriod;

    if (invincibilityCooldownCurrent == 0) {
      canAttack = true;
    }
  }
  public void grabStateEnter()
  {
      setAllBoolAnimParametersToFalse();
      isBlocking = false;
      isBeingGrabbed = true;
      canAttack = false;
      witchAnim.SetBool("isBeingGrabbed", true);
  }
  public void throwStateEnter(Vector2 throwForce)
  {
      setAllBoolAnimParametersToFalse();
      witchAnim.SetBool("isBeingThrown", true);
      isBeingGrabbed = false;
      isBeingThrown = true;
      canAttack = false;
      applyThrowForce(throwForce);
  }
  public void throwStateExit()
  {
    isBeingThrown = false;
    applyThrowDamage();

    if (!isDead && !isDying) {
      knockDownEnter();
    }
  }

  void applyThrowForce(Vector2 throwForce) {
    upperCollisionBoxGameObj.layer = LayerMask.NameToLayer("InteractsWithEverythingButPlayer");
    lowerCollisionBoxGameObj.layer = LayerMask.NameToLayer("InteractsWithEverythingButPlayer");
    RB2D.AddForce(new Vector2(throwForce.x * transform.localScale.x, throwForce.y), ForceMode2D.Impulse);
  }

  void applyThrowDamage() {
    if (thudSoundEffect == null) {
      Debug.LogError($"{this.name}: thudSoundEffect is null!");
      return;
    }
    if (currentHealth > 0 && invincibilityCooldownCurrent <= 0 && !isKnockedDown)
    {
      if (!infiniteHealth)
        currentHealth -= throwDamage;
      hitSparkAnimator.SetBool("isActive", true);
      audioSrc.clip = thudSoundEffect;
      // audioSrc.enabled = true;
      audioSrc.Play();
      if (currentHealth <= 0) {
        enemyDeath();
      }
    }
  }
  public void grabStateExit()
  {
    isBeingGrabbed = false;
    witchAnim.SetBool("isBeingGrabbed", false);
    invincibilityCooldownCurrent = invincibilityCooldownPeriod;
  }

  public void enemyDeath()
  {
    CVO.removeObject(gameObject);
    isDying = true;
    witchAnim.SetBool("isDying", true);
    // gameObject.SetActive(false);
  }

  public void setDeathVars() {
    isDead = true;
    witchAnim.SetBool("isDead", true);
    upperCollisionBoxGameObj.layer = LayerMask.NameToLayer("OnlyInteractsWithWallsGround");
    lowerCollisionBoxGameObj.layer = LayerMask.NameToLayer("OnlyInteractsWithWallsGround");
  }

  public void setCanAttackFalse()
  {
    canAttack = false;
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
    if (isDead || isDying) {
      return;
    }
    /*if (hit.GetComponent<Collider>().tag == "Player") {
			playerObject.GetComponent<PlayerHealth> ().playerTakeDamage (1);
		}*/
    isNotInAnimation = isNotInAnimCheck();
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

  void setAllBoolAnimParametersToFalse() {
    foreach (AnimatorControllerParameter parameter in witchAnim.parameters)
    {
      string paramType = parameter.type.ToString();
      string boolType = AnimatorControllerParameterType.Bool.ToString();
      if (paramType == boolType) {
        witchAnim.SetBool(parameter.name, false);
      }
    }
  }

  bool isNotInAnimCheck() {
    return witchAnim.GetBool("isReeling") == false && witchAnim.GetBool("isSlashing") == false && witchAnim.GetBool("isHeavyPunching") == false && witchAnim.GetBool("isBlocking") == false && witchAnim.GetBool("isBlockingAnAttack") == false && witchAnim.GetBool("isKnockedDown") == false && witchAnim.GetBool("isBeingThrown") == false && !isBeingGrabbed;
  }
}
