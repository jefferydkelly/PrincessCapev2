using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGlove : MagneticGlove
{
    /// <summary>
    /// Pulls the Target towards the player or vice versa
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
                        Game.Instance.Player.Rigidbody.AddForce(-Direction * force);
                    }
                    else
                    {

                        target.Rigidbody.AddForce(Direction * force);
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
            ClearTarget();
            Game.Instance.Player.IsPulling = false;
            state = MagicItemState.OnCooldown;
            cooldownTimer.Start();
        }
    }
}
