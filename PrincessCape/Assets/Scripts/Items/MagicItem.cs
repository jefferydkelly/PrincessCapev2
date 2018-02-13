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
	// Use this for initialization
    public MagicItem () {
        cooldownTimer = new Timer(Reset, cooldownTime);
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

    void SwapItems() {
        MagicItem other = null;

        foreach(MagicItem mi in Game.Instance.Player.Inventory) {
            if (mi.slot == MagicItemSlot.Second) {
                other = mi;
                break;
            }
        }
        StopListening();
        EventManager.StopListening("SwapItems", SwapItems);
        if (other != null) {
            other.StopListening();
            other.RegisterItemOne();
        }
        RegisterItemTwo();
    }

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

    public Sprite Sprite {
        get {
            return itemSprite;
        }
    }

    public MagicItemSlot Slot {
        get {
            return slot;
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
