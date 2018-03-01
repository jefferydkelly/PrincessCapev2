using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagneticGlove : MagicItem {
    protected float range = 10;
    protected float force = 25f;
    protected Metal target = null;

    /// <summary>
    /// Finds the target.
    /// </summary>
	protected void FindTarget()
	{
        if (!target)
        {
            target = Metal.HighlightedBlock;
        }

        if (target)
		{
            if (!IsTargetInRange) {
                target = null;
            }
            if (!target.IsStatic)
            {
                target.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
		}
	}

    /// <summary>
    /// Clears the target.
    /// </summary>
    protected void ClearTarget() {
		if (target)
		{
			if (!target.IsStatic)
			{
				target.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
			}
			//target.Velocity = Vector2.zero;
            target.Clear();
			target = null;
		}
    }

    /// <summary>
    /// Activate this instance.
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
                FindTarget();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the selected Metal block in range.
    /// </summary>
    /// <value><c>true</c> if is target in range; otherwise, <c>false</c>.</value>
    protected bool IsTargetInRange {
        get {
            return Vector2.Distance(target.transform.position, Game.Instance.Player.transform.position) <= range;
        }
    }

    /// <summary>
    /// Returns the direction between the player and the target rounded to the nearest 45 degrees.
    /// </summary>
    /// <value>The direction between the player and the selected metal block.</value>
    protected Vector2 Direction {
        get {
            if (target) {
                Vector2 dif = Game.Instance.Player.transform.position - target.transform.position;
                float ang = dif.Angle();
                ang = ang.RoundToNearest(Mathf.PI / 8);
                return ang.FromRadianToVector();
            }
            return Vector2.zero;
        }
    }
}
