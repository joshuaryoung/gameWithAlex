using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIscript : MonoBehaviour
{

  public GameObject healthBar;
  public PlayerHealth pHealth;
  public float scaleX;
  public int currentHealth;
  public int startHealth;
  public bool isDeadLast = false;

  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    if (!isDeadLast)
    {
      startHealth = pHealth.startHealth;
      currentHealth = pHealth.currentHealth;
      scaleX = (float)currentHealth / startHealth;
      healthBar.transform.localScale = new Vector3(scaleX, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
      // healthBar.sprite = hBarSprite [pHealth.currentHealth <= hBarSprite.Length - 1 ? pHealth.currentHealth : hBarSprite.Length - 1];
      // Make rectangle inside of a frame, the right coordinates moving left as a percentage of health
      isDeadLast = pHealth.isDead;
    }
  }


}
