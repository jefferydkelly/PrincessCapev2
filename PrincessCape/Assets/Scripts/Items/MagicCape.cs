using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MagicCape : MagicItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Cape"/> class.
    /// </summary>
    public MagicCape() {
		EventManager.StartListening("PlayerLanded", OnPlayerLanded);

        itemName = "Magic Cape";
        itemGetMessage = new List<string>() {
            "You got the Magic Cape!",
            "As you put it on, you feel yourself getting lighter",
            "Press and hold the Left Mouse Button to float gently down"
        };
    }

    private void OnEnable()
    {
        itemSprite = Resources.Load<Sprite>("Sprites/Cape");
    }

    /// <summary>
    /// Activate this instance lessening the gravity scale and setting the Player's y-velocity to 0.
    /// </summary>
    public override void Activate()
    {
        if (!Game.Instance.IsPaused)
        {
            if (state == MagicItemState.Ready)
            {
                if (slot == MagicItemSlot.First)
                {
                    EventManager.TriggerEvent("ItemOneActivatedSuccessfully");
                }
                else
                {
                    EventManager.TriggerEvent("ItemTwoActivatedSuccessfully");
                }
                state = MagicItemState.Activated;
                Game.Instance.Player.Rigidbody.gravityScale = 0.15f;
                Game.Instance.Player.Rigidbody.velocity = Game.Instance.Player.Velocity.SetY(0);
            }
        }
    }

    /// <summary>
    /// Deactivates the cape resetting the players gravity scale.
    /// </summary>
    public override void Deactivate() {
        if (!Game.Instance.IsPaused)
        {
            if (state == MagicItemState.Activated)
            {
                if (slot == MagicItemSlot.First)
                {
                    EventManager.TriggerEvent("ItemOneDeactivatedSuccessfully");
                }
                else if (slot == MagicItemSlot.Second)
                {
                    EventManager.TriggerEvent("ItemTwoDeactivatedSuccessfully");
                }
                Game.Instance.Player.Rigidbody.gravityScale = 1.0f;

                cooldownTimer.Start();
                state = MagicItemState.OnCooldown;
            }
        }
    }

    /// <summary>
    /// Deactivates the cape when the Player land if it they are still using it.
    /// </summary>
    void OnPlayerLanded() {
        if (state == MagicItemState.Activated)
        {
            
            Deactivate();
        }
    }
}
