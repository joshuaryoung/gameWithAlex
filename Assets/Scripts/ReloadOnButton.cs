using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadOnButton : MonoBehaviour
{
	public KeyCode reloadKeyCode = new KeyCode();

    void Start() {
		reloadKeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Reload", "JoystickButton9"));
    }

    void Update ()
    {
        if(Input.GetKeyDown(reloadKeyCode))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        
    }
}
