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
    public override void Activate()
    {
        if (!cooldownTimer.IsRunning && canUse) {
            Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0.2f;//0.1f;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    public override void Deactivate() {
		Rigidbody2D rb = Game.Instance.Player.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.0f;
        cooldownTimer.Start();
        canUse = false;
    }

    public override void Reset()
    {
        Game.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        canUse = true;
    }

    void OnPlayerLanded() {
        Deactivate();
    }
}
