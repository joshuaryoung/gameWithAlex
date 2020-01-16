using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadOnButton : MonoBehaviour
{
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        
    }
}
