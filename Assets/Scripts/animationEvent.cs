using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationEvent : MonoBehaviour
{
    public void animationEnded(string message) {
        gameObject.SetActive(false);
    }
}
