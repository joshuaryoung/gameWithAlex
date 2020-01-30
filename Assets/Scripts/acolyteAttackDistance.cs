/*
Bugs

Enemy punches when in reel state

butts

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acolyteAttackDistance : MonoBehaviour {
	public acolyteBehavior AB;
	public PlayerInputScript PIS;
	public Animator anim;
	public AudioSource audioSrc;
	public AudioClip punchSoundEffect;
	public int attackRNG;
	// Use this for initialization
	void Start () {
		audioSrc = GetComponentInParent<AudioSource>();
	}

	void OnTriggerStay2D(Collider2D col2D)
	{
		if (col2D.gameObject.tag == "PlayerCharacter" && AB.invincibilityCooldownCurrent <= 0 && AB.canAttack && !PIS.isDead) {
			AB.isWithinAttackDistance = true;
			attackRNG = Random.Range (1, 200);

			if (attackRNG == 5 && AB.canAttack) {
				AB.canAttack = false;
				anim.SetBool ("isLightPunching", true);
				audioSrc.clip = punchSoundEffect;
				audioSrc.enabled = true;
				audioSrc.Play();
			} else if(attackRNG == 1 && AB.canAttack) {
				AB.canAttack = false;
				anim.SetBool ("isHeavyPunching", true);
				audioSrc.clip = punchSoundEffect;
				audioSrc.enabled = true;
				audioSrc.Play();
			}
			
			else {
				if(AB.canAttack) {
					anim.SetBool ("isLightPunching", false);
					anim.SetBool ("isHeavyPunching", false);
				}
			}
		}
	}

	// void OnTriggerExit2D(Collider2D col2D)
	// {
	// 	if (col2D.gameObject.tag == "PlayerCharacter") {
	// 		AB.isWithinAttackDistance = false;
	// 		anim.SetBool ("isPunching", false);
	// 	}
	// }
}
