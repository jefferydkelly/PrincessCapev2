using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadController : Controller {
    
    public GamepadController() {
        keys.Clear();
        keys["Jump"] = KeyCode.JoystickButton1;
        keys["Interact"] = KeyCode.JoystickButton4;
        keys["ItemOne"] = KeyCode.JoystickButton5;
        keys["ItemTwo"] = KeyCode.JoystickButton2;
        keys["Pause"] = KeyCode.JoystickButton7;
        keys["Inventory"] = KeyCode.JoystickButton8;
    }

    public override float Horizontal
    {
        get
        {
            return Input.GetAxis("Horizontal");
        }
    }

	public override float Vertical
	{
		get
		{
			return Input.GetAxis("Vertical");
		}
	}

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <returns>The key.</returns>
    /// <param name="key">Key.</param>
    public override string GetKey(string key)
    {
        KeyCode keycode = KeyCode.None;

        if (keys.TryGetValue(key, out keycode))
        {
            switch (keycode)
            {
                case KeyCode.JoystickButton1:
                    return "A";
                case KeyCode.JoystickButton2:
                    return "B";
                case KeyCode.JoystickButton4:
                    return "X";
                case KeyCode.JoystickButton5:
                    return "Y";
                default:
                    return keycode.ToString();
            }

        }
        return "";
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (Input.anyKeyDown) {
            for (int i = 0; i < 10; i++) {
                string keystring = string.Format("joystick button {0}", i);
                if (Input.GetKeyDown(keystring)) {
                    Debug.Log(keystring);
                }
            }
        }
    }


}
