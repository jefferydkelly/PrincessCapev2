using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller:Manager {
    static Controller instance;
    protected Dictionary<string, KeyCode> keys;
    UnityEvent anyKey;
    UnityEvent onPause;
   
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Controller"/> class.
    /// </summary>
    protected Controller() {
        instance = this;
        keys = new Dictionary<string, KeyCode>();
        keys.Add("Forward", KeyCode.D);
        keys.Add("Backward", KeyCode.A);
        keys.Add("Up", KeyCode.W);
        keys.Add("Down", KeyCode.S);
        keys.Add("Jump", KeyCode.Space);
        keys.Add("ItemOne", KeyCode.Mouse0);
        keys.Add("ItemTwo", KeyCode.Mouse1);
        keys.Add("Interact", KeyCode.F);
        keys.Add("Pause", KeyCode.P);
        keys.Add("Inventory", KeyCode.I);
        anyKey = new UnityEvent();
        onPause = new UnityEvent();
    }

    public void SetKeys(Dictionary<string, KeyCode> keyDict) {
        keys = keyDict;
        Debug.Log(keys["ItemOne"]);
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
                    //UIManager.Instance.UpdateKeys();
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
    public virtual float Horizontal {
        get {
            return (Input.GetKey(keys["Forward"]) ? 1 : 0) - (Input.GetKey(keys["Backward"]) ? 1 : 0);
            //return Input.GetAxis("Horizontal");
        }
    }

    /// <summary>
    /// Gets the vertical input.
    /// </summary>
    /// <value>The vertical input.</value>
    public virtual float Vertical {
        get {
            return (Input.GetKey(keys["Up"]) ? 1 : 0) - (Input.GetKey(keys["Down"]) ? 1 : 0);
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
            return Input.GetKeyDown(keys["ItemOne"]);
        }
    }

	/// <summary>
	/// Gets a value indicating whether the item one key is being held down.
	/// </summary>
	/// <value><c>true</c> if the item one is being held down; otherwise, <c>false</c>.</value>
	bool IsItemOneHeld {
        get {
            return Input.GetKey(keys["ItemOne"]);
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
            return Input.GetKeyUp(keys["ItemOne"]);
		}
	}

	/// <summary>
	/// Gets a value indicating whether the item two key has been pressed down on the last frame.
	/// </summary>
	/// <value><c>true</c> if the item two has been pressed in the last frame; otherwise, <c>false</c>.</value>
	bool IsItemTwoDown {
        get {
            return Input.GetKeyDown(keys["ItemTwo"]);
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
            return Input.GetKey(keys["ItemTwo"]);
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
            return Input.GetKeyUp(keys["ItemTwo"]);
		}
	}

    /// <summary>
    /// Gets a value indicating whether the Jump key has been pressed within the last frame.
    /// </summary>
    /// <value><c>true</c> if the Jump has been pressed; otherwise, <c>false</c>.</value>
    public virtual bool Jump {
        get {
            return Input.GetKeyDown(keys["Jump"]) || (!Game.Instance.Player.IsOnLadder && Input.GetKeyDown(keys["Up"]));

        }
    }

    /// <summary>
    /// Gets a value indicating whether the Pause key has been pressed within the last frame.
    /// </summary>
    /// <value><c>true</c> if the Pause key has been pressed; otherwise, <c>false</c>.</value>
    public bool Pause {
        get {
            return Input.GetKeyDown(keys["Pause"]) || Input.GetKeyDown(KeyCode.Escape);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the interact key has been pressed since the last frame.
    /// </summary>
    /// <value><c>true</c> if interact; otherwise, <c>false</c>.</value>
    public bool Interact {
        get {
            return Input.GetKeyDown(keys["Interact"]);
        }
    }

    bool IsKeyDown(string keyName) {
        return Input.GetKeyDown(keys[keyName]);
    }

    bool IsKeyHeld (string keyName) {
        return Input.GetKey(keys[keyName]);
    }

    bool IsKeyReleased (string keyName) {
        return Input.GetKeyUp(keys[keyName]);
    }
    /// <summary>
    /// Updates Input.
    /// </summary>
    /// <param name="dt">The time since the last update.</param>
    public virtual void Update(float dt)
    {

        if (Input.anyKeyDown) {
            anyKey.Invoke();
        }
		if (Pause)
		{
            onPause.Invoke();
			//EventManager.TriggerEvent("Pause");
		}
        else if (IsKeyDown("ItemOne"))
		{
			EventManager.TriggerEvent("ItemOneActivated");
		}
        else if (IsKeyHeld("ItemOne"))
		{
			EventManager.TriggerEvent("ItemOneHeld");
		}
        else if (IsKeyReleased("ItemOne"))
		{
			EventManager.TriggerEvent("ItemOneDeactivated");
		}

		if (IsKeyDown("ItemTwo"))
		{
			EventManager.TriggerEvent("ItemTwoActivated");
		}
		else if (IsKeyHeld("ItemTwo"))
		{
			EventManager.TriggerEvent("ItemTwoHeld");
		}
		else if (IsKeyReleased("ItemTwo"))
		{
			EventManager.TriggerEvent("ItemTwoDeactivated");
		}

        if (Game.Instance.IsPlaying || Game.Instance.IsInInventory)
        {
            if (IsKeyDown("Inventory"))
            {
                EventManager.TriggerEvent("Inventory");
            }
        }

        if(Game.Instance.IsPlaying) {

            if (IsKeyDown("Interact"))
            {
                EventManager.TriggerEvent("Interact");
                //EventManager.TriggerEvent("ClearInteraction");
            }
        }


	}

    public string Info {
        get {
            string info = PCLParser.StructStart;
            foreach(KeyValuePair<string, KeyCode> kvp in keys) {
                info += PCLParser.CreateAttribute(kvp.Key, kvp.Value);
            }
            info += PCLParser.StructEnd;
            return info;
        }
    }

    public Dictionary<string, KeyCode> KeyDict {
        get {
            return keys;
        }
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <returns>The key.</returns>
    /// <param name="key">Key.</param>
    public virtual string GetKey(string key, bool fullName = false) {
        KeyCode keycode = KeyCode.None;

        if (keys.TryGetValue(key, out keycode)) {
            if (keycode == KeyCode.Mouse0) {
                return fullName ? "Left Mouse Button" : "LMB";
            } else if (keycode == KeyCode.Mouse1) {
                return fullName ? "Right Mouse Button" : "RMB";
            } else if (keycode == KeyCode.LeftArrow) {
                return fullName ? "Left Arrow Key" : "Left";
			} else if (keycode == KeyCode.RightArrow)
			{
				return fullName ? "Right Arrow Key" : "Right";
            } else if (keycode == KeyCode.UpArrow) {
                return fullName ? "Up Arrow Key" : "Up";
            } else if (keycode == KeyCode.DownArrow) {
                return fullName ? "Down Arrow Key" : "Down";
            }

            return keycode.ToString() + (fullName ? " Key" : "");
        }
        return "";
    }

    public UnityEvent AnyKey {
        get {
            return anyKey;
        }
    }

    public UnityEvent OnPause {
        get {
            return onPause;
        }
    }
}
