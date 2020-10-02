/*
Bugs

Enemy punches when in reel state

butts

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acolyteAttackDistance : MonoBehaviour
{
  public GameObject playerGameObject;
  public GameObject enemyGameObject;
  public Collider2D collider2D;
  public acolyteBehavior AB;
  public PlayerInputScript PIS;
  public Animator anim;
  public bool isColidingWithSomething;
  // Use this for initialization
  void Start()
  {
    playerGameObject = GameObject.FindWithTag("PlayerCharacter");
    enemyGameObject = gameObject.transform.parent.gameObject;
  }

  // void OnTriggerEnter2D(Collider2D col2D)
  // {
  //   isColidingWithSomething = true;
  //   collider2D = col2D;
  //   if (col2D.gameObject.tag == "PlayerCharacter" && AB.invincibilityCooldownCurrent <= 0 && AB.canAttack && !PIS.isDead)
  //   {
  //     Debug.Log("Player Collision!");
  //     AB.isInFootsiesRange = true;
  //   }
  // }

  void OnTriggerStay2D (Collider2D col2D) {
    isColidingWithSomething = true;
    collider2D = col2D;
    if (col2D.gameObject.tag == "PlayerCharacter" && AB.invincibilityCooldownCurrent <= 0 && AB.canAttack && !PIS.isDead)
    {
      Debug.Log("Player Collision!");
      AB.isInFootsiesRange = true;
    }
  }

  void OnTriggerExit2D(Collider2D col2D)
  {
    isColidingWithSomething = false;
    collider2D = null;
    if (col2D.gameObject.tag == "PlayerCharacter")
    {
      AB.isInFootsiesRange = false;
      // anim.SetBool ("isPunching", false);
    }
  }
}
