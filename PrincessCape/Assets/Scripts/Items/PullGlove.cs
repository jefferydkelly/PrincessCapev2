using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGlove : MagneticGlove
{
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
                        Game.Instance.Player.Rigidbody.AddForce((target.transform.position - Game.Instance.Player.transform.position).normalized * 10);
                    }
                    else
                    {

                        target.Rigidbody.AddForce((Game.Instance.Player.transform.position - target.transform.position).normalized * 10);
                    }
                } else {
                    ClearTarget();
                }
            }
        }
    }


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
