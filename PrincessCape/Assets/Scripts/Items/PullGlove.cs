using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGlove : MagicItem
{
    Metal target;
    public override void Activate()
    {
        if (!target) {
            target = Metal.HighlightedBlock;
            if (target && !target.IsStatic) {
                target.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        if (target) {
            Game.Instance.Player.IsPulling = true;
            if (target.IsStatic) {
                Game.Instance.Player.Rigidbody.AddForce((target.transform.position - Game.Instance.Player.transform.position).normalized * 10);
            } else {

				target.Rigidbody.AddForce((Game.Instance.Player.transform.position - target.transform.position).normalized * 10);
            }
        }
    }

    public override void Deactivate()
    {
        if (target)
        {
            if (!target.IsStatic) {
                target.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            }
            target.Velocity = Vector2.zero;
            target = null;
        }
        Game.Instance.Player.IsPulling = false;
    }
}
