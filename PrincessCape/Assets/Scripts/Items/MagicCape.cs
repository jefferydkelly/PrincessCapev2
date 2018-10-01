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
		Game.Instance.Player.OnLand.AddListener(OnPlayerLanded);

        itemName = "Magic Cape";
        itemGetMessage = new List<string>() {
            "You got the Magic Cape!",
            "As you put it on, you feel yourself getting lighter",
            "Press and hold the [[[ItemOne]]] to float gently down"
        };
		itemDescritpion = "Press and hold the Item Key to float down gently.";
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
        if (Game.Instance.IsPlaying)
        {
            if (state == MagicItemState.Ready)
            {
                State = MagicItemState.Activated;
                Game.Instance.Player.IsFloating = true;
                Game.Instance.Player.Rigidbody.gravityScale = 0.15f;
                Game.Instance.Player.Rigidbody.velocity = Game.Instance.Player.Velocity.SetY(0);
            }
        }
    }

    /// <summary>
    /// Deactivates the cape resetting the players gravity scale.
    /// </summary>
    public override void Deactivate() {
        if (Game.Instance.IsPlaying)
        {
            if (state == MagicItemState.Activated)
            {
                Game.Instance.Player.Rigidbody.gravityScale = 1.0f;

                cooldownTimer.Start();
                State = MagicItemState.OnCooldown;
                Game.Instance.Player.IsFloating = false;
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
