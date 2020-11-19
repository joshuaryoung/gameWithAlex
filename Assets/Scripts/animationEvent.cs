using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationEvent : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator hitSparkAnimator;

    void Start() {
        hitSparkAnimator = GetComponent<Animator>();
    }
    public void animationEnded(string message) {
        hitSparkAnimator.SetBool("isActive", false);
    }

    public void setBlockingAttackFalse() {
        playerAnimator.SetBool("isBlockingAnAttack", false);
    }
}
