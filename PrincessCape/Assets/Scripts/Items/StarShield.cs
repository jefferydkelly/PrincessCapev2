using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarShield : MagicItem
{
    public StarShield() {
		itemName = "Star Shield";
		itemGetMessage = new List<string>() {
			"You got the Star Shield!",
			"Press and hold the item button to reflect projectiles away from you"
		};
    }

    private void OnEnable()
    {
        itemSprite = Resources.Load<Sprite>("Sprites/Shield");
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
                EventManager.TriggerEvent("ShowAim");
				state = MagicItemState.Activated;
                Game.Instance.Player.IsUsingShield = true;
			}
		}
	}

	/// <summary>
	/// Deactivates the cape resetting the players gravity scale.
	/// </summary>
	public override void Deactivate()
	{
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
                EventManager.TriggerEvent("HideAim");
                Game.Instance.Player.IsUsingShield = false;
				cooldownTimer.Start();
				state = MagicItemState.OnCooldown;
			}
		}
	}
}
