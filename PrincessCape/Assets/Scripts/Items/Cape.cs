using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Cape : MagicItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Cape"/> class.
    /// </summary>
    public Cape() {
		EventManager.StartListening("PlayerLanded", OnPlayerLanded);
        itemSprite = Resources.Load<Sprite>("Sprites/Cape");
        itemName = "Magic Cape";
    }
   
    /// <summary>
    /// Activate this instance lessening the gravity scale and setting the Player's y-velocity to 0.
    /// </summary>
    public override void Activate()
    {
        if (state == MagicItemState.Ready) {
            if (slot == MagicItemSlot.First)
            {
                EventManager.TriggerEvent("ItemOneActivatedSuccessfully");
            } else {
                EventManager.TriggerEvent("ItemTwoActivatedSuccessfully");
            }
            state = MagicItemState.Activated;
            Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0.15f;//0.1f;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    /// <summary>
    /// Deactivates the cape resetting the players gravity scale.
    /// </summary>
    public override void Deactivate() {

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
            Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1.0f;
            cooldownTimer.Start();
            state = MagicItemState.OnCooldown;
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
