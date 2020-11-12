using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBlockerScript : MonoBehaviour
{
  public Animator acolyteAnim;
  public Transform parentTransform;
  public acolyteBehavior AB;

  void Start() {
    if (acolyteAnim == null) {
      acolyteAnim = GetComponentInParent<Animator>();
    }
    if (parentTransform == null) {
      parentTransform = GetComponentInParent<Transform>();
    }
    if (AB == null) {
      AB = GetComponentInParent<acolyteBehavior>();
    }
  }

  void flip() {
    Vector2 flipLocalScale = parentTransform.localScale;
    flipLocalScale.x *= -1;
    parentTransform.localScale = flipLocalScale;
    AB.flipCoolDown = AB.flipCoolDownMax;
  }

  void OnTriggerEnter2D(Collider2D col2D)
  {
    bool isNotInAnimation = acolyteAnim.GetBool("isHeavyPunching") == false && acolyteAnim.GetBool("isLightPunching") == false && acolyteAnim.GetBool("isReeling") == false;
    bool isAIBlocker = col2D.gameObject.layer == LayerMask.NameToLayer("AIBlocker");
    bool isFacingTowardsWall = (col2D.gameObject.transform.localPosition.x - parentTransform.localPosition.x) * parentTransform.localScale.x > 0;

    if (isAIBlocker && isFacingTowardsWall && AB.flipCoolDown <= 0)
    {
      flip();
    }
  }
}
