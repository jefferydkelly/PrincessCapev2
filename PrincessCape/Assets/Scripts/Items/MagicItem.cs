﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicItem {

    protected Timer cooldownTimer;
    protected float cooldownTime = 0.25f;
    protected MagicItemState state = MagicItemState.Ready;
    protected MagicItemSlot slot = MagicItemSlot.None;
	// Use this for initialization
    public MagicItem () {
        cooldownTimer = new Timer(Reset, cooldownTime);
	}

    /// <summary>
    /// Registers the item as the first item and starts listening for the associated events
    /// </summary>
    public void RegisterItemOne() {
        slot = MagicItemSlot.First;
        EventManager.StartListening("ItemOneActivated", Activate);
        EventManager.StartListening("ItemOneHeld", Use);
        EventManager.StartListening("ItemOneDeactivated", Deactivate);
    }

	/// <summary>
	/// Deregisters the item as the first item and stops listening for the associated events
	/// </summary>
	public void DegristerItemOne() {
        slot = MagicItemSlot.None;
        EventManager.StopListening("ItemOneActivated", Activate);
        EventManager.StopListening("ItemOneHeld", Use);
        EventManager.StopListening("ItemOneDeactivated", Deactivate);
    }

	/// <summary>
	/// Registers the item as the second item and starts listening for the associated events
	/// </summary>
	public void RegisterItemTwo()
	{
        slot = MagicItemSlot.Second;
		EventManager.StartListening("ItemTwoActivated", Activate);
        EventManager.StartListening("ItemTwoHeld", Use);
		EventManager.StartListening("ItemTwoDeactivated", Deactivate);
	}

	/// <summary>
	/// Deregisters the item as the second item and stops listening for the associated events
	/// </summary>
	public void DegristerItemTwo()
	{
        slot = MagicItemSlot.None;
		EventManager.StopListening("ItemTwoActivated", Activate);
        EventManager.StopListening("ItemTwoHeld", Use);
		EventManager.StopListening("ItemTwoDeactivated", Deactivate);
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
