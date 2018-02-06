using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagneticGlove : MagicItem {
    protected float range = 10;
    protected float force = 10f;
    protected Metal target = null;
	protected void FindTarget()
	{
		target = Metal.HighlightedBlock;

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

    protected void ClearTarget() {
		if (target)
		{
			if (!target.IsStatic)
			{
				target.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
			}
			target.Velocity = Vector2.zero;
			target = null;
		}
    }

    public override void Activate()
    {
        if (state == MagicItemState.Ready)
        {
            state = MagicItemState.Activated;
            FindTarget();
        }
    }

    protected bool IsTargetInRange {
        get {
            return Vector2.Distance(target.transform.position, Game.Instance.Player.transform.position) <= range;
        }
    }
}
