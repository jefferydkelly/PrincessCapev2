using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller:Manager {
    static Controller instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Controller"/> class.
    /// </summary>
    Controller() {
        instance = this;
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static Controller Instance {
        get {
            if (instance == null) {
                instance = new Controller();
                Game.Instance.AddManager(instance);
            }
            return instance;
        }
    }

    /// <summary>
    /// Gets the horizontal input.
    /// </summary>
    /// <value>The horizontal input.</value>
    public float Horizontal {
        get {
            return Input.GetAxis("Horizontal");
        }
    }

    /// <summary>
    /// Gets the vertical input.
    /// </summary>
    /// <value>The vertical input.</value>
    public float Vertical {
        get {
            return Input.GetAxis("Vertical");
        }
    }

    /// <summary>
    /// Gets a value indicating whether the item one key has been pressed down on the last frame.
    /// </summary>
    /// <value><c>true</c> if the item one has been pressed in the last frame; otherwise, <c>false</c>.</value>
    bool IsItemOneDown {
        get {
            return Input.GetKeyDown(KeyCode.Mouse0);
        }
    }

	/// <summary>
	/// Gets a value indicating whether the item one key is being held down.
	/// </summary>
	/// <value><c>true</c> if the item one is being held down; otherwise, <c>false</c>.</value>
	bool IsItemOneHeld {
        get {
            return Input.GetKey(KeyCode.Mouse0);
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
			return Input.GetKeyUp(KeyCode.Mouse0);
		}
	}

	/// <summary>
	/// Gets a value indicating whether the item two key has been pressed down on the last frame.
	/// </summary>
	/// <value><c>true</c> if the item two has been pressed in the last frame; otherwise, <c>false</c>.</value>
	bool IsItemTwoDown {
        get {
            return Input.GetKeyDown(KeyCode.Mouse1);
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
			return Input.GetKey(KeyCode.Mouse1);
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
			return Input.GetKeyUp(KeyCode.Mouse1);
		}
	}

    /// <summary>
    /// Gets a value indicating whether the Jump key has been pressed within the last frame.
    /// </summary>
    /// <value><c>true</c> if the Jump has been pressed; otherwise, <c>false</c>.</value>
    public bool Jump {
        get {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the Pause key has been pressed within the last frame.
    /// </summary>
    /// <value><c>true</c> if the Pause key has been pressed; otherwise, <c>false</c>.</value>
    public bool Pause {
        get {
            return Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);
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
}
