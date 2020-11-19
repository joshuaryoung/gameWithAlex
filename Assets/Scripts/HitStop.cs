using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    bool isWaiting = false;
    public float hitStopDuration;
    public void stop() {
        if (isWaiting) {
            return;
        }

        Time.timeScale = 0.0f;
        StartCoroutine(wait());
    }

    IEnumerator wait() {
        isWaiting = true;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1.0f;
        isWaiting = false;
    }
}
