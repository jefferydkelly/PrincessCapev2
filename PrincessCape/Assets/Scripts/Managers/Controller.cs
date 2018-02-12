using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller {
    static Controller instance;

    Controller() {
        instance = this;
    }

    public static Controller Instance {
        get {
            if (instance == null) {
                instance = new Controller();
            }
            return instance;
        }
    }
    public float Horizontal {
        get {
            return Input.GetAxis("Horizontal");
        }
    }

    public float Vertical {
        get {
            return Input.GetAxis("Vertical");
        }
    }

    bool IsItemOneDown {
        get {
            return Input.GetKeyDown(KeyCode.Mouse0);
        }
    }

    bool IsItemOneHeld {
        get {
            return Input.GetKey(KeyCode.Mouse0);
        }
    }

	bool IsItemOneUp
	{
		get
		{
			return Input.GetKeyUp(KeyCode.Mouse0);
		}
	}

    bool IsItemTwoDown {
        get {
            return Input.GetKeyDown(KeyCode.Mouse1);
        }
    }

	bool IsItemTwoHeld
	{
		get
		{
			return Input.GetKey(KeyCode.Mouse1);
		}
	}

	bool IsItemTwoUp
	{
		get
		{
			return Input.GetKeyUp(KeyCode.Mouse1);
		}
	}

    public bool Jump {
        get {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        }
    }

    public bool Pause {
        get {
            return Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);
        }
    }

    public void Update() {
		if (Pause)
		{
            EventManager.TriggerEvent(Game.Instance.IsPaused ? "Unpause" : "Pause");
		}

        if (!Game.Instance.IsPaused)
        {
            if (IsItemOneDown)
            {
                EventManager.TriggerEvent("ItemOneActivated");
            }
            else if (IsItemOneHeld)
            {
                EventManager.TriggerEvent("ItemOneHeld");
            }
            else if (IsItemOneUp)
            {
                EventManager.TriggerEvent("ItemOneDeactivated");
            }

            if (IsItemTwoDown)
            {
                EventManager.TriggerEvent("ItemTwoActivated");
            }
            else if (IsItemTwoHeld)
            {
                EventManager.TriggerEvent("ItemTwoHeld");
            }
            else if (IsItemTwoUp)
            {
                EventManager.TriggerEvent("ItemTwoDeactivated");
            }
        }
    }
}
