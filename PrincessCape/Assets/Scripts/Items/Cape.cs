using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Cape : MagicItem
{
    private UnityAction listener;

    public Cape() {
		listener = new UnityAction(OnPlayerLanded);
		EventManager.StartListening("PlayerLanded", OnPlayerLanded);
    }
   
    public override void Activate()
    {
        if (state == MagicItemState.Ready) {
            state = MagicItemState.Activated;
            Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0.15f;//0.1f;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    public override void Deactivate() {

        if (state == MagicItemState.Activated)
        {
            Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1.0f;
            cooldownTimer.Start();
            state = MagicItemState.OnCooldown;
        }
    }

    void OnPlayerLanded() {
        if (state == MagicItemState.Activated)
        {
            Deactivate();
        }
    }
}
