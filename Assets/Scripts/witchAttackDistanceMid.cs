/*
Bugs

Enemy punches when in reel state

butts

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class witchAttackDistanceMid : MonoBehaviour
{
  public GameObject playerGameObject;
  public GameObject enemyGameObject;
  public Collider2D _col2D;
  public WitchBehavior WB;
  public PlayerInputScript PIS;
  public Animator anim;
  public bool isColidingWithSomething;
  // Use this for initialization
  void Start()
  {
    playerGameObject = GameObject.FindWithTag("PlayerCharacter");
    enemyGameObject = gameObject.transform.parent.gameObject;
    if (PIS == null) {
      if (playerGameObject == null) {
        Debug.LogError("playerGameObject is null!");
        return;
      }

      PIS = playerGameObject.GetComponent<PlayerInputScript>();
    }
  }

  void OnTriggerStay2D (Collider2D col2D) {
    if (WB == null) {
      Debug.LogError("WB is null!");
      return;
    }
    if (PIS == null) {
      Debug.LogError("PIS is null!");
      return;
    }
    isColidingWithSomething = true;
    _col2D = col2D;

    bool isPlayerCharacter = col2D.gameObject.tag == "PlayerCharacter";
    bool isNotInInvincibilityAnim = WB.invincibilityCooldownCurrent <= 0;

    if (isPlayerCharacter && isNotInInvincibilityAnim && WB.canAttack && !PIS.isDead)
    {
      // Debug.Log("Player Collision!");
      WB.currentFootsiesRange = footsies.range.Mid;
    }
  }

  void OnTriggerExit2D(Collider2D col2D)
  {
    if (WB == null) {
      Debug.LogError("WB is null!");
      return;
    }
    isColidingWithSomething = false;
    _col2D = null;
    if (col2D.gameObject.tag == "PlayerCharacter")
    {
      WB.currentFootsiesRange = footsies.range.None;
    }
  }
}
