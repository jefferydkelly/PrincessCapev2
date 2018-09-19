using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : ActivatedObject
{
    BoxCollider2D theBelt;
    [SerializeField]
    Vector2 pushDir = Vector2.right;
    float pushForce = 5;
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Initializes the ConveyorBelt
    /// </summary>
    public override void Init() {
		theBelt = GetComponent<BoxCollider2D>();
        if (startActive)
        {
            Activate();
        }
        else
        {
            theBelt.enabled = false;
        }
    }

    /// <summary>
    /// Activates the Conveyor Belt
    /// </summary>
    public override void Activate()
    {
		theBelt.enabled = true;
		IsActivated = true;
    }

    /// <summary>
    /// Deactivates the Conveyor Belt
    /// </summary>
    public override void Deactivate()
    {
		theBelt.enabled = false;
		IsActivated = false;
    }

    /// <summary>
    /// Handles the start of collisions
    /// </summary>
    /// <param name="collision">Collision.</param>
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (Game.Instance && Game.Instance.IsPlaying)
        {
            collision.attachedRigidbody.AddForce(pushDir * pushForce);
        }
    }

    /// <summary>
    /// Flips the conveyor belt horizontally
    /// </summary>
    public override void FlipX()
    {
        base.FlipX();
        pushDir = pushDir.SetX(-pushDir.x);
    }

    /// <summary>
    /// Flips the belt vertically.
    /// </summary>
    public override void FlipY()
    {
        base.FlipY();
        pushDir = pushDir.SetY(-pushDir.y);
    }

    /// <summary>
    /// Rotates the conveyorbelt
    /// </summary>
    /// <param name="ang">Ang.</param>
    public override void Rotate(float ang)
    {
        base.Rotate(ang);
        pushDir = pushDir.RotateDeg(ang);
    }
}
