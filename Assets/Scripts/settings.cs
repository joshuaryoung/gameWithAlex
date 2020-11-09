using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class settings : MonoBehaviour
{
  public GameObject settingsMenuGameObject;
  public Event e;
  public string currentlyCapturingButtonName;
  public bool isCapturingInput = false;
  public PlayerInputScript PIS;
  public ReloadOnButton ROB;
  public KeyCode pauseKeyCode = new KeyCode();
  // Start is called before the first frame update
  void Start()
  {
    pauseKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause", "JoystickButton10"));
  }

  public void OnClick()
  {
    settingsMenuGameObject.SetActive(!settingsMenuGameObject.activeSelf);
  }

  void Update() {
    if(PIS == null || ROB == null) {
      Debug.Log("PIS or ROB is empty!");
      return;
    }
    if (isCapturingInput) {
      bool joystickButtonFound = false;
      int joystickButtonIndex = -1;
      string searchTerm;
      KeyCode joystickKeyCode = new KeyCode();
      for(int i = 0; i < 20; i++) {
        searchTerm = "JoystickButton" + i;
        joystickKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), searchTerm);
        joystickButtonFound = Input.GetKeyDown(joystickKeyCode);
        if(joystickButtonFound) {
          joystickButtonIndex = i;
          break;
        }
      }

      if(joystickButtonFound) {
        setPISButton(currentlyCapturingButtonName, joystickKeyCode);
        Debug.Log(currentlyCapturingButtonName + ": joystick button " + joystickButtonIndex);
        // PIS.$"{currentlyCapturingButtonName}" = joystickKeyCode;
        isCapturingInput = false;
      }
      // string keyCode = e.keyCode.ToString();
      // Debug.Log(currentlyCapturingButtonName + ": " + keyCode);
    }

    if (Input.GetKeyDown(pauseKeyCode)) {
      settingsMenuGameObject.SetActive(!settingsMenuGameObject.activeSelf);
    }
  }

  public void OnButtonPress(string text) {
    currentlyCapturingButtonName = text;
    isCapturingInput = true;
  }

  public void setPISButton(string buttonName, KeyCode keyCode) {
    PlayerPrefs.SetString(buttonName, keyCode.ToString());
    switch (buttonName)
    {
        case "Jump":
          PIS.jumpKeyCode = keyCode;
          break;
        case "Punch":
          PIS.punchKeyCode = keyCode;
          break;
        case "Kick":
          PIS.kickKeyCode = keyCode;
          break;
        case "Run":
          PIS.runKeyCode = keyCode;
          break;
        case "Block":
          PIS.blockKeyCode = keyCode;
          break;
        case "Up":
          PIS.upKeyCode = keyCode;
          break;
        case "Down":
          PIS.downKeyCode = keyCode;
          break;
        case "Left":
          PIS.leftKeyCode = keyCode;
          break;
        case "Right":
          PIS.rightKeyCode = keyCode;
          break;
        case "Reload":
          ROB.reloadKeyCode = keyCode;
          break;
        case "Pause":
          pauseKeyCode = keyCode;
          break;
        default:
          break;
    }
  }
}
