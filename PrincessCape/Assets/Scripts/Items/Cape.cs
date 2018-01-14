using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Cape : MagicItem
{
    private UnityAction listener;
    bool canUse = true;
    private void OnEnable()
    {
        listener = new UnityAction(OnPlayerLanded);
        EventManager.StartListening("PlayerLanded", OnPlayerLanded);

    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerLanded", listener);
    }
    public override void Use()
    {
        if (!cooldownTimer.IsRunning && canUse) {
            Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            canUse = false;
            cooldownTimer.Start();
        }
    }

    public override void Reset()
    {
        Game.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
    }

    void OnPlayerLanded() {
        canUse = true;
    }
}
