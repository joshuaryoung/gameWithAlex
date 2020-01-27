using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIscript : MonoBehaviour {

	public Image healthBar;
	public Sprite[] hBarSprite;
	public PlayerHealth pHealth;
	public bool isDeadLast = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDeadLast){
			healthBar.sprite = hBarSprite [pHealth.currentHealth <= hBarSprite.Length - 1 ? pHealth.currentHealth : hBarSprite.Length - 1];
			isDeadLast = pHealth.isDead;
		}
	}


}
