using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBoyAIBlockerScript : MonoBehaviour
{
  public Animator bigBoyAnim;
  public Transform parentTransform;
  public BigBoyBehavior BBB;
  public bool isCollidingWithAIBlocker;

  void Start() {
    if (bigBoyAnim == null) {
      bigBoyAnim = GetComponentInParent<Animator>();
    }
    if (parentTransform == null) {
      parentTransform = GetComponentInParent<Transform>();
    }
    if (BBB == null) {
      BBB = GetComponentInParent<BigBoyBehavior>();
      Debug.Log("");
    }
  }

  void flip() {
    Vector2 flipLocalScale = parentTransform.localScale;
    flipLocalScale.x *= -1;
    parentTransform.localScale = flipLocalScale;
  }

  void OnTriggerEnter2D(Collider2D col2D)
  {
    bool isNotInAnimation = bigBoyAnim.GetBool("isHeavyPunching") == false && bigBoyAnim.GetBool("isLightPunching") == false && bigBoyAnim.GetBool("isReeling") == false;
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
    if (BBB == null) {
      Debug.LogError("BBB is null!");
    }
    bool isAIBlocker = col2D.gameObject.layer == LayerMask.NameToLayer("AIBlocker");
    
    if(isAIBlocker) {
      isCollidingWithAIBlocker = false;
      BBB.flipCoolDown = BBB.flipCoolDownMax;
    }
  }
}
