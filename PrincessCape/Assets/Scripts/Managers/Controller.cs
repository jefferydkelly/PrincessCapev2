using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller:Manager {
    static Controller instance;

    KeyCode forward = KeyCode.D;
    KeyCode backward = KeyCode.A;
    KeyCode up = KeyCode.W;
    KeyCode down = KeyCode.S;
    KeyCode jump = KeyCode.Space;
    KeyCode interact = KeyCode.F;
    KeyCode itemOne = KeyCode.Mouse0;
    KeyCode itemTwo = KeyCode.Mouse1;
    KeyCode pause = KeyCode.P;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Controller"/> class.
    /// </summary>
    Controller() {
        instance = this;
    }

    public void SetKeys(Dictionary<string, KeyCode> keys) {
        forward = keys["Forward"];
        backward = keys["Backward"];
        up = keys["Up"];
        down = keys["Down"];
        jump = keys["Jump"];
        interact = keys["Interact"];
        itemOne = keys["First Item"];
        itemTwo = keys["Second Item"];
        pause = keys["Pause"];

    }
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static Controller Instance {
        get {
            if (!Game.isClosing)
            {
                if (instance == null)
                {
                    instance = new Controller();
                    Game.Instance.AddManager(instance);
                }
                return instance;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the horizontal input.
    /// </summary>
    /// <value>The horizontal input.</value>
    public float Horizontal {
        get {
            return (Input.GetKey(forward) ? 1 : 0) - (Input.GetKey(backward) ? 1 : 0);
            //return Input.GetAxis("Horizontal");
        }
    }

    /// <summary>
    /// Gets the vertical input.
    /// </summary>
    /// <value>The vertical input.</value>
    public float Vertical {
        get {
            return (Input.GetKey(up) ? 1 : 0) - (Input.GetKey(down) ? 1 : 0);
        }
    }

    /// <summary>
    /// Gets the directional input.
    /// </summary>
    /// <value>The directional input.</value>
    public Vector2 DirectionalInput {
        get {
            return new Vector2(Horizontal, Vertical);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the item one key has been pressed down on the last frame.
    /// </summary>
    /// <value><c>true</c> if the item one has been pressed in the last frame; otherwise, <c>false</c>.</value>
    bool IsItemOneDown {
        get {
            return Input.GetKeyDown(itemOne);
        }
    }

	/// <summary>
	/// Gets a value indicating whether the item one key is being held down.
	/// </summary>
	/// <value><c>true</c> if the item one is being held down; otherwise, <c>false</c>.</value>
	bool IsItemOneHeld {
        get {
            return Input.GetKey(itemOne);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the item one key was released in the last frame.
    /// </summary>
    /// <value><c>true</c> if the item one key was released in the last frame; otherwise, <c>false</c>.</value>
	bool IsItemOneUp
	{
		get
		{
            return Input.GetKeyUp(itemOne);
		}
	}

	/// <summary>
	/// Gets a value indicating whether the item two key has been pressed down on the last frame.
	/// </summary>
	/// <value><c>true</c> if the item two has been pressed in the last frame; otherwise, <c>false</c>.</value>
	bool IsItemTwoDown {
        get {
            return Input.GetKeyDown(itemTwo);
        }
    }

	/// <summary>
	/// Gets a value indicating whether the item two key is being held down.
	/// </summary>
	/// <value><c>true</c> if the item two is being held down; otherwise, <c>false</c>.</value>
	bool IsItemTwoHeld
	{
		get
		{
            return Input.GetKey(itemTwo);
		}
	}

	/// <summary>
	/// Gets a value indicating whether the item two key was released in the last frame.
	/// </summary>
	/// <value><c>true</c> if the item two key was released in the last frame; otherwise, <c>false</c>.</value>
	bool IsItemTwoUp
	{
		get
		{
            return Input.GetKeyUp(itemTwo);
		}
	}

    /// <summary>
    /// Gets a value indicating whether the Jump key has been pressed within the last frame.
    /// </summary>
    /// <value><c>true</c> if the Jump has been pressed; otherwise, <c>false</c>.</value>
    public bool Jump {
        get {
            return Input.GetKeyDown(jump) || Input.GetKeyDown(up);// || Input.GetKeyDown(KeyCode.UpArrow);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the Pause key has been pressed within the last frame.
    /// </summary>
    /// <value><c>true</c> if the Pause key has been pressed; otherwise, <c>false</c>.</value>
    public bool Pause {
        get {
            return Input.GetKeyDown(pause) || Input.GetKeyDown(KeyCode.Escape);
        }
    }

    /// <summary>
    /// Updates Input.
    /// </summary>
    /// <param name="dt">The time since the last update.</param>
    public void Update(float dt)
    {
		if (Pause)
		{
			EventManager.TriggerEvent(Game.Instance.IsPaused ? "Unpause" : "Pause");

			if (Game.Instance.IsPaused)
			{
				EventManager.TriggerEvent("ShowItemMenu");
			}
            else
			{
				EventManager.TriggerEvent("HideItemMenu");
			}
		}
        else if (IsItemOneDown)
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

    public string Info {
        get {
            string info = PCLParser.StructStart;
            info += PCLParser.CreateAttribute("Forward", forward);
            info += PCLParser.CreateAttribute("Backward", backward);
            info += PCLParser.CreateAttribute("Up", up);
            info += PCLParser.CreateAttribute("Down", down);
            info += PCLParser.CreateAttribute("Jump", jump);
            info += PCLParser.CreateAttribute("Interact", interact);
            info += PCLParser.CreateAttribute("First Item", itemOne);
            info += PCLParser.CreateAttribute("Second Item", itemTwo);
            info += PCLParser.CreateAttribute("Pause", pause);
            info += PCLParser.StructEnd;
            return info;
        }
    }

    public Dictionary<string, KeyCode> KeyDict {
        get {
            Dictionary<string, KeyCode> dict = new Dictionary<string, KeyCode>();
            dict.Add("Forward", forward);
			dict.Add("Backward", backward);
			dict.Add("Up", up);
			dict.Add("Down", down);
            dict.Add("Jump", jump);
			dict.Add("Interact", interact);
			dict.Add("First Item", itemOne);
			dict.Add("Second Item", itemTwo);
			dict.Add("Pause", pause);
            return dict;
        }
    }
}
