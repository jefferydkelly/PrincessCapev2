using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MagicItem: ScriptableObject {

    protected Timer cooldownTimer;
    protected float cooldownTime = 0.25f;
    protected MagicItemState state = MagicItemState.Ready;
    protected MagicItemSlot slot = MagicItemSlot.None;
    protected string itemName;
    protected Sprite itemSprite;
    protected List<string> itemGetMessage;
    protected string itemDescritpion = "This is an item";
    protected MagicItemEvent onActivationStateChange;
    // Use this for initialization
    public MagicItem()
    {
        cooldownTimer = new Timer(cooldownTime);
        cooldownTimer.OnComplete.AddListener(Reset);
        Game.Instance.Player.OnDie.AddListener(() =>
        {
            Deactivate();
            cooldownTimer.Stop();
            Reset();
        });

        onActivationStateChange = new MagicItemEvent();
        Game.Instance.OnEditorStop.AddListener(() =>
        {

            onActivationStateChange.RemoveAllListeners();
        });
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
		State = MagicItemState.Ready;
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

        set {
            slot = value;
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

    public MagicItemEvent OnActivationStateChange {
        get {
            return onActivationStateChange;
        }
    }

    protected MagicItemState State {
        get {
            return state;
        }

        set {
            if (state != value)
            {
                state = value;
                onActivationStateChange.Invoke(state);
            }
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

public class MagicItemEvent : UnityEvent<MagicItemState>
{

}
