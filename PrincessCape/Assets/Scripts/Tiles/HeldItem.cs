using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : InteractiveObject
{
    Rigidbody2D myRigidbody;
    bool isHeld = false;
    public override void Init()
    {
        base.Init();
        myRigidbody = GetComponent<Rigidbody2D>();
    }
    public void Drop()
    {
        Game.Instance.Player.HeldItem = null;
        myRigidbody.gravityScale = 1;
        //transform.position += Game.Instance.Player.Forward * 0.1f;
        if (Mathf.Abs(Game.Instance.Player.Velocity.x) >= 0.25f)
        {
            myRigidbody.velocity = Game.Instance.Player.Forward * 12.5f;
        }
        isHeld = false;
    }

    public override void Interact()
    {
        if (!isHeld)
        {
            Game.Instance.Player.HeldItem = this;
            myRigidbody.gravityScale = 0;
            isHeld = true;
            EventManager.StartListening("Interact", Drop);
        } else {
            Drop();
        }
    }

    public void Update()
    {
        if (isHeld)
        {
            if (Mathf.Abs(Game.Instance.Player.Velocity.x) >= 0.25f)
            {
                UIManager.Instance.SetInteractionText("Throw");
            }
            else
            {
                UIManager.Instance.SetInteractionText("Drop");
            }
        }
    }
}
