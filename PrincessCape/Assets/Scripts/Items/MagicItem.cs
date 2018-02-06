using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicItem {

    protected Timer cooldownTimer;
    protected float cooldownTime = 0.25f;
    protected MagicItemState state = MagicItemState.Ready;
	// Use this for initialization
    public MagicItem () {
        cooldownTimer = new Timer(Reset, cooldownTime);
	}

    public void RegisterItemOne() {
        EventManager.StartListening("ItemOneActivated", Activate);
        EventManager.StartListening("ItemOneHeld", Use);
        EventManager.StartListening("ItemOneDeactivated", Deactivate);
    }

    public void DegristerItemOne() {
        EventManager.StopListening("ItemOneActivated", Activate);
        EventManager.StopListening("ItemOneHeld", Use);
        EventManager.StopListening("ItemOneDeactivated", Deactivate);
    }

	public void RegisterItemTwo()
	{
		EventManager.StartListening("ItemTwoActivated", Activate);
        EventManager.StartListening("ItemTwoHeld", Use);
		EventManager.StartListening("ItemTwoDeactivated", Deactivate);
	}

	public void DegristerItemTwo()
	{
		EventManager.StopListening("ItemTwoActivated", Activate);
        EventManager.StopListening("ItemTwoHeld", Use);
		EventManager.StopListening("ItemTwoDeactivated", Deactivate);
	}

    public abstract void Activate();
    public virtual void Use()
    {

    }
    public abstract void Deactivate();

    public virtual void Reset() {
		state = MagicItemState.Ready;
    }
}

public enum MagicItemState {
    Ready,
    Activated,
    OnCooldown
}
