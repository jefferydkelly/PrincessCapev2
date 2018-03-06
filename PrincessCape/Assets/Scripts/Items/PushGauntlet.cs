using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushGauntlet : MagneticGlove {
    public PushGauntlet()
	{
        itemName = "Push Gauntlet";

		itemGetMessage = new List<string>() {
			"You got the Push Gaunlet!",
			"Slipping it on, you feel the power flowing through your hands",
			"Press and hold the item button to push metal blocks away from you",
			"But beware, just like the Push Gauntlet, if the block can't be moved.  You will be."
		};
	}

    private void OnEnable()
    {
        itemSprite = Resources.Load<Sprite>("Sprites/PushGauntlet");
    }
    /// <summary>
    /// Use pulls the Metal towards the Player or vice versa.
    /// </summary>
    public override void Use()
    {
        if (!Game.Instance.IsPaused)
        {
            if (state == MagicItemState.Activated)
            {
                if (!target)
                {
                    FindTarget();
                }

                if (target)
                {
                    if (IsTargetInRange)
                    {
                        Game.Instance.Player.IsPulling = true;
                        Vector2 gloveForce = Direction * gloveForceWeight;
                        Vector2 inputForce = Controller.Instance.DirectionalInput * inputForceWeight;
                        if (target.IsStatic)
                        {
                            Game.Instance.Player.Rigidbody.AddForce((gloveForce + inputForce).normalized * force);
							Game.Instance.Player.Rigidbody.ClampVelocity(maxSpeed);
                        }
                        else
                        {
                            target.Rigidbody.AddForce((inputForce - gloveForce).normalized * force);
                            target.Rigidbody.ClampVelocity(maxSpeed);
                        }
                    }
                    else
                    {
                        ClearTarget();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Ends the use of the glove
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
                ClearTarget();
                Game.Instance.Player.IsPulling = false;
                state = MagicItemState.OnCooldown;
                cooldownTimer.Start();
            }
        }
	}
}
