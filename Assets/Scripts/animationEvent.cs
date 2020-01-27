using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationEvent : MonoBehaviour
{
    public Animator playerAnimator;
    public void animationEnded(string message) {
        gameObject.SetActive(false);
    }

    public void setBlockingAttackFalse() {
        playerAnimator.SetBool("isBlockingAnAttack", false);
    }
}
