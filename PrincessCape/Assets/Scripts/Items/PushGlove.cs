using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushGlove : MagneticGlove {
	public PushGlove()
	{
        itemName = "Push Gauntlet";
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
                    if (target.IsStatic)
                    {
                        Game.Instance.Player.Rigidbody.AddForce(Direction * force);
                    }
                    else
                    {
                        target.Rigidbody.AddForce(-Direction * force);
                    }
                } else {
                    ClearTarget();
                }
            }
        }
    }
    /// <summary>
    /// Ends the use of the glove
    /// </summary>
	public override void Deactivate()
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
