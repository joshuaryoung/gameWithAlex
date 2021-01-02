using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class witchAIBlockerScript : MonoBehaviour
{
  public Animator witchAnim;
  public Transform parentTransform;
  public WitchBehavior WB;
  public bool isCollidingWithAIBlocker;

  void Start() {
    if (witchAnim == null) {
      witchAnim = GetComponentInParent<Animator>();
    }
    if (parentTransform == null) {
      parentTransform = GetComponentInParent<Transform>();
    }
    if (WB == null) {
      WB = GetComponentInParent<WitchBehavior>();
    }
  }

  void flip() {
    Vector2 flipLocalScale = parentTransform.localScale;
    flipLocalScale.x *= -1;
    parentTransform.localScale = flipLocalScale;
  }

  void OnTriggerEnter2D(Collider2D col2D)
  {
    bool isNotInAnimation = witchAnim.GetBool("isHeavyPunching") == false && witchAnim.GetBool("isSlashing") == false && witchAnim.GetBool("isReeling") == false;
    bool isAIBlocker = col2D.gameObject.layer == LayerMask.NameToLayer("AIBlocker");
    bool isFacingTowardsWall = (col2D.gameObject.transform.localPosition.x - parentTransform.localPosition.x) * parentTransform.localScale.x > 0;

    if (isAIBlocker && isFacingTowardsWall)
    {
      isCollidingWithAIBlocker = true;
      flip();
    }
  }

  void OnTriggerExit2D(Collider2D col2D)
  {
    if (WB == null) {
      Debug.LogError("WB is null!");
    }
    bool isAIBlocker = col2D.gameObject.layer == LayerMask.NameToLayer("AIBlocker");
    
    if(isAIBlocker) {
      isCollidingWithAIBlocker = false;
      WB.flipCoolDown = WB.flipCoolDownMax;
    }
  }
}
