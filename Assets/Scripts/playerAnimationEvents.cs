using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationEvents : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if(!animator) {
            animator = GetComponent<Animator>();
        }
    }

    void eventHandler(string message) {
        animator.SetBool("isReeling", false);
    }
}
