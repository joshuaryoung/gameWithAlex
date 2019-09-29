﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public int startHealth;
	public int currentHealth;
	public int invincibilityCooldown;
	public int invincibilityCooldownFlash = 0;
	public bool isDead = false;
	public SpriteRenderer spriteR;
	public Color spriteColor;
	public Text deathScreen;
	private cameraFollow cf;
	private PlayerInputScript PIS;

	// Use this for initialization
	void Start ()
	{
		currentHealth = startHealth;
		spriteR = GetComponent<SpriteRenderer> ();
		spriteColor = spriteR.color;
		invincibilityCooldown = 0;
		cf = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<cameraFollow> ();
		PIS = gameObject.GetComponent<PlayerInputScript> ();
	}

	void Update()
	{
		if(invincibilityCooldown > 0)
			invincibilityCooldown--;
		//invincibility Animation
		if (invincibilityCooldown > 0) {
			spriteColor.a = invincibilityCooldownFlash;
			invincibilityCooldownFlash = (invincibilityCooldownFlash * -1) + 1;
			spriteR.color = spriteColor;
			invincibilityCooldown--;
			if (invincibilityCooldown == 0){
				spriteColor.a = (float)255;
				spriteR.color = spriteColor;
			}
		}
	}
	
	public void playerTakeDamage(int damage)
	{
		if (invincibilityCooldown == 0 && !PIS.blockPressed)
		{
			if (currentHealth - damage <= 0) {
				currentHealth -= damage;
				death ();
			}

			if (currentHealth - damage > 0)
			{
				currentHealth -= damage;
				invincibilityCooldown = 100;
				PIS.canAct = true;
			}
		}
	}

	void death()
	{
		isDead = true;
		deathScreen.enabled = true;
		Destroy (gameObject);
		cf.enabled = !cf.enabled;
	}
}
