﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

  public int startHealth;
  public int currentHealth;
  public float invincibilityCooldownCurrent;
  public float invincibilityCooldownMax;
  public int invincibilityCooldownFlash = 0;
  public bool isDead = false;
  public bool isDying;
  public SpriteRenderer spriteR;
  public Color spriteColor;
  public Text deathScreen;
  public GameObject playerCharacter;
  public GameObject hitSparkObject;
  public Animator hitSparkAnimator;
  public GameObject blockSparkObject;
  public Animator blockSparkAnimator;
  public Animator anim;
  private cameraFollow cf;
  private PlayerInputScript PIS;
  public Rigidbody2D RB2D;
  public bool InfiniteHealth;
  public AudioSource audioSrc;
  public AudioClip impactSoundEffect;
  public AudioClip gettingGrabbedSoundEffect;
  public AudioClip blockSoundEffect;
  public AudioClip slamSoundEffect;
  public float currentReelLengthCooldown;

  // Use this for initialization
  void Start()
  {
    audioSrc = GetComponent<AudioSource>();
    currentHealth = startHealth;
    spriteR = GetComponent<SpriteRenderer>();
    spriteColor = spriteR.color;
    invincibilityCooldownCurrent = 0;
    cf = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<cameraFollow>();
    PIS = gameObject.GetComponent<PlayerInputScript>();
    RB2D = gameObject.GetComponent<Rigidbody2D>();
    if (!playerCharacter)
    {
      playerCharacter = gameObject;
    }
    if (hitSparkObject)
    {
      anim = GetComponent<Animator>();
    }
    if (blockSparkObject)
    {
      anim = GetComponent<Animator>();
    }
  }

  void Update()
  {
    if(hitSparkAnimator == null) {
      Debug.LogError("hitSparkAnimator is null!");
      return;
    }
    if(blockSparkAnimator == null) {
      Debug.LogError("blockSparkAnimator is null!");
      return;
    }
    if (currentReelLengthCooldown > 0)
    {
      currentReelLengthCooldown -= Time.deltaTime;
      if (currentReelLengthCooldown <= 0)
      {
        anim.SetBool("isReeling", false);
        anim.SetBool("isBlockingAnAttack", false);
        PIS.isAbleToAct = true;
      }
    }

    //invincibility Animation
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
  }

  public void playerTakeDamage(object[] args)
  {
    if (impactSoundEffect == null) {
      Debug.LogError($"{this.name}: impactSoundEffect is null");
      return;
    }
    int damage = (int)args[0];
    float pushBackValue = (float)args[1];
    float attackReelValue = (float)args[2];
    float attackBlockStunValue = (float)args[3];

    PIS.attackHasAlreadyHit = false;
    PIS.isInGrabAttackState = false;
    
    PIS.isBeingGrabbed = false;

    pushBack(pushBackValue);
    if (invincibilityCooldownCurrent <= 0 && (!PIS.isBlocking || !PIS.isGrounded || PIS.isInGrabAttackState))
    {
      audioSrc.clip = impactSoundEffect;
      audioSrc.Play();
      if (currentHealth - damage <= 0)
      {
        currentHealth = 0;
        dying();
      }

      if (currentHealth - damage > 0)
      {
        currentReelLengthCooldown = attackReelValue;
        if (!InfiniteHealth)
          currentHealth -= damage;
        invincibilityCooldownCurrent = invincibilityCooldownMax;
        PIS.isAbleToAct = true;
        PIS.isForwardDashing = false;
        PIS.isBackDashing = false;
        PIS.hasReleasedWall = true;
        PIS.currentGrabAttackIndex = 0;
        setAllBoolAnimParametersToFalse(anim);
        hitSparkAnimator.SetBool("isActive", true);
        anim.SetBool("isReeling", true);
        if (PIS.currentlyGrabbedEnemy != null) {
          PIS.grabAttackStateExit();
        }
      }
    }
    else if (invincibilityCooldownCurrent <= 0 && PIS.isBlocking && PIS.isGrounded)
    {
      currentReelLengthCooldown = attackBlockStunValue;
      audioSrc.clip = blockSoundEffect;
      audioSrc.Play();
      blockSparkAnimator.SetBool("isActive", true);
      setAllBoolAnimParametersToFalse(anim);
      PIS.isForwardDashing = false;
      PIS.isBackDashing = false;
      // PIS.isAbleToAct = true;
    }
  }

  public void playerTakeGrabDamage(object[] args) {
    if (slamSoundEffect == null) {
      Debug.LogError($"{this.name}: slamSoundEffect is null!");
      return;
    }

    int damage = (int)args[0];

    audioSrc.clip = slamSoundEffect;
    audioSrc.Play();
    if (currentHealth - damage <= 0)
    {
      currentHealth = 0;
      dying();
    }

    if (currentHealth - damage > 0)
    {
      if (!InfiniteHealth)
        currentHealth -= damage;
      invincibilityCooldownCurrent = invincibilityCooldownMax;
      playerGrabStateExit();
    }
  }

  public void playerGrabStateExit() {
    PIS.isForwardDashing = false;
    PIS.isBackDashing = false;
    PIS.hasReleasedWall = true;
    PIS.currentGrabAttackIndex = 0;
    setAllBoolAnimParametersToFalse(anim);
    anim.SetBool("isBeingGrabbed", false);
    anim.SetBool("isKnockedDown", true);
  }

  public void playerGetGrabbed()
  {
    if (gettingGrabbedSoundEffect == null) {
      Debug.LogError($"{this.name}: gettingGrabbedSoundEffect is null");
      return;
    }

    PIS.attackHasAlreadyHit = false;

    if (invincibilityCooldownCurrent <= 0 && PIS.isGrounded)
    {
      audioSrc.clip = gettingGrabbedSoundEffect;
      audioSrc.Play();
      PIS.isBeingGrabbed = true;
      PIS.isAbleToAct = false;
      PIS.isForwardDashing = false;
      PIS.isBackDashing = false;
      PIS.hasReleasedWall = true;
      setAllBoolAnimParametersToFalse(anim);
      anim.SetBool("isBeingGrabbed", true);
    }
  }

  void pushBack(float pushBackValue)
  {
    RB2D.velocity = (new Vector2(RB2D.velocity.x + pushBackValue, RB2D.velocity.y));
  }

  void dying()
  {
    PIS.isDead = true;
    isDying = true;
    anim.SetBool("isDying", isDying);

  }
  void death()
  {
    if (deathScreen == null) {
      Debug.LogError("deathScreen is null!");
      return;
    }
    isDying = false;
    anim.SetBool("isDying", isDying);
    RB2D.simulated = false;

    isDead = true;
    deathScreen.enabled = true;
    anim.SetBool("isDead", isDead);
    cf.enabled = !cf.enabled;
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
}
