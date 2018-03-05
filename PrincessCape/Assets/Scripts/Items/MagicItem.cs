using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicItem: ScriptableObject {

    protected Timer cooldownTimer;
    protected float cooldownTime = 0.25f;
    protected MagicItemState state = MagicItemState.Ready;
    protected MagicItemSlot slot = MagicItemSlot.None;
    protected string itemName;
    protected Sprite itemSprite;
    protected List<string> itemGetMessage;
    protected string itemDescritpion = "This is an item";
	// Use this for initialization
    public MagicItem () {
        cooldownTimer = new Timer(cooldownTime);
        cooldownTimer.OnComplete.AddListener(Reset);
	}

    /// <summary>
    /// Registers the item as the first item and starts listening for the associated events
    /// </summary>
    public void RegisterItemOne() {
        if (slot != MagicItemSlot.First)
        {
            EventManager.TriggerEvent("UnequipItemOne");
            slot = MagicItemSlot.First;
            StartListening();
            EventManager.TriggerEvent("ItemOneEquipped");
            EventManager.StartListening("SwapItems", SwapItems);

        }
    }

	/// <summary>
	/// Deregisters the item as the first item and stops listening for the associated events
	/// </summary>
	public void DeregisterItemOne() {
        if (slot == MagicItemSlot.First)
        {
            StopListening();
            EventManager.StopListening("SwapItems", SwapItems);
            slot = MagicItemSlot.None;
        }
    }

	/// <summary>
	/// Registers the item as the second item and starts listening for the associated events
	/// </summary>
	public void RegisterItemTwo()
	{
        if (slot != MagicItemSlot.Second)
        {
            EventManager.TriggerEvent("UnequipItemTwo");
            slot = MagicItemSlot.Second;
            StartListening();
            EventManager.TriggerEvent("ItemTwoEquipped");
        }
	}

	/// <summary>
	/// Deregisters the item as the second item and stops listening for the associated events
	/// </summary>
	public void DeregisterItemTwo()
	{
        if (slot == MagicItemSlot.Second)
        {
            StopListening();
            slot = MagicItemSlot.None;
        }
	}

    /// <summary>
    /// Swaps the items in the equipped slots.
    /// </summary>
    void SwapItems() {
        MagicItem other = null;

        foreach(MagicItem mi in Game.Instance.Player.Inventory) {
            if (mi.slot == MagicItemSlot.Second) {
                other = mi;
                break;
            }
        }
        slot = MagicItemSlot.None;
        StopListening();
        EventManager.StopListening("SwapItems", SwapItems);
        if (other != null) {
            other.StopListening();
            other.RegisterItemOne();
        }

        RegisterItemTwo();
    }

    /// <summary>
    /// Start listening to the events associated with Activation, Deactivation and removing items.
    /// </summary>
    void StartListening() {
        string itemSlot = "Item" + (slot == MagicItemSlot.First ? "One" : "Two");
       
        EventManager.StartListening(itemSlot + "Activated", Activate);
        EventManager.StartListening(itemSlot + "Held", Use);
        EventManager.StartListening(itemSlot + "Deactivated", Deactivate);
        if (slot == MagicItemSlot.First) {
            EventManager.StartListening("UnequipItemOne", DeregisterItemOne);
        } else {
            EventManager.StartListening("UnequipItemTwo", DeregisterItemTwo);
        }
    }

	/// <summary>
	/// Stop listening to the events associated with Activation, Deactivation and removing items.
	/// </summary>
	void StopListening() {
		string itemSlot = "Item" + (slot == MagicItemSlot.First ? "One" : "Two");
        EventManager.StopListening(itemSlot + "Activated", Activate);
        EventManager.StopListening(itemSlot + "Held", Use);
        EventManager.StopListening(itemSlot + "Deactivated", Deactivate);
		if (slot == MagicItemSlot.First)
		{
            EventManager.StopListening("UnequipItemOne", DeregisterItemOne);
		}
		else
		{
            EventManager.StopListening("UnequipItemTwo", DeregisterItemTwo);
		}
    }

    public abstract void Activate();
    public virtual void Use()
    {

    }
    public abstract void Deactivate();

    /// <summary>
    /// Reset this instance to be used again.
    /// </summary>
    public virtual void Reset() {
        if (slot == MagicItemSlot.First)
        {
            EventManager.TriggerEvent("ItemOneCooldown");
        }
        else if (slot == MagicItemSlot.Second)
        {
            EventManager.TriggerEvent("ItemTwoCooldown");
        }
		state = MagicItemState.Ready;
    }

    /// <summary>
    /// Gets the sprite.
    /// </summary>
    /// <value>The sprite.</value>
    public Sprite Sprite {
        get {
            return itemSprite;
        }
    }

    /// <summary>
    /// Gets the slot to which the item is assigned.
    /// </summary>
    /// <value>The slot to which the item is assigned.</value>
    public MagicItemSlot Slot {
        get {
            return slot;
        }
    }

    /// <summary>
    /// Gets the item message the player sees upon obtaining the item.
    /// </summary>
    /// <value>The item get message.</value>
    public List<string> ItemGetMessage {
        get {
            return itemGetMessage;
        }
    }

    /// <summary>
    /// Gets the description of the item.
    /// </summary>
    /// <value>The description.</value>
    public string Description {
        get {
            return itemDescritpion;
        }
    }

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    /// <value>The name of the item.</value>
    public string ItemName {
        get {
            return itemName;
        }
    }
}

public enum MagicItemState {
    Ready,
    Activated,
    OnCooldown
}

public enum MagicItemSlot {
    First,
    Second,
    None
}
