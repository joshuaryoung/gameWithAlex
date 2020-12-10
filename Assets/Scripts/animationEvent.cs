using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class animationEvent : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator hitSparkAnimator;
    public PlayerInputScript PIS;

    void Start() {
        if (PIS == null) {
            PIS = FindObjectOfType<PlayerInputScript>();
        }
        hitSparkAnimator = GetComponent<Animator>();
    }
    public void animationEnded(string message) {
        hitSparkAnimator.SetBool("isActive", false);
    }

    public void setBlockingAttackFalse() {
        playerAnimator.SetBool("isBlockingAnAttack", false);
    }

    public void setCanCombo(int state) {
        if (PIS == null) {
            Debug.LogError("PIS is null!");
            return;
        }

        PIS.canCombo = Convert.ToBoolean(state);
    }

    public void setAttackHasAlreadyHit() {
        if (PIS == null) {
            Debug.LogError("PIS is null!");
            return;
        }

        PIS.attackHasAlreadyHit = true;
    }


    public void setCanActFalse() {
        if (PIS == null) {
            Debug.LogError("PIS is null!");
            return;
        }
        
        PIS.isAbleToAct = false;
    }
    public void setCanActTrue() {
        if (PIS == null) {
            Debug.LogError("PIS is null!");
            return;
        }
        
        PIS.isAbleToAct = true;
    }
}
