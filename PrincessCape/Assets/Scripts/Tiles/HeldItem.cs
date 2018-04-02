using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : InteractiveObject
{
    protected Rigidbody2D myRigidbody;
    protected bool isHeld = false;
    protected bool canBeThrown = true;
    public override void Init()
    {
        base.Init();
        myRigidbody = GetComponent<Rigidbody2D>();
    }
    public void Drop()
    {
        Game.Instance.Player.HeldItem = null;
        UIManager.Instance.SetInteractionText("");
        myRigidbody.gravityScale = 1;
        //transform.position += Game.Instance.Player.Forward * 0.1f;
        isHeld = false;

    }

    public void Throw() {
        myRigidbody.AddForce(Game.Instance.Player.Forward * 6.25f, ForceMode2D.Impulse);
    }

    public override void Interact()
    {
        if (!isHeld)
        {
            Game.Instance.Player.HeldItem = this;
            myRigidbody.gravityScale = 0;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            isHeld = true;
            IsHighlighted = false;
            EventManager.StartListening("Interact", Drop);
        } else {
            Drop();

            if (canBeThrown && Mathf.Abs(Game.Instance.Player.Velocity.x) >= 0.25f) {
                Throw();
            }
        }
    }

    public void Update()
    {
        if (isHeld)
        {
            if (canBeThrown && Mathf.Abs(Game.Instance.Player.Velocity.x) >= 0.25f)
            {
                UIManager.Instance.SetInteractionText("Throw");
            }
            else
            {
                UIManager.Instance.SetInteractionText("Drop");
            }
        }
    }

    public bool IsHeavy {
        get {
            return myRigidbody.mass > 1.0f;
        }
    }
}
