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
  // Start is called before the first frame update
  void Start()
  {

  }

  public void OnClick()
  {
    settingsMenuGameObject.SetActive(!settingsMenuGameObject.activeSelf);
  }

  void Update() {
    if(PIS == null) {
      Debug.Log("PIS is empty!");
      return;
    }
    e = Event.current;
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
        default:
          break;
    }
  }
}
