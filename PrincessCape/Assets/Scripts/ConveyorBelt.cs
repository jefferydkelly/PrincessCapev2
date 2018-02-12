using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : ActivatedObject
{
    Animator myAnimator;
    BoxCollider2D theBelt;
    [SerializeField]
    Vector2 pushDir = Vector2.right;
    float pushForce = 5;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        theBelt = GetComponent<BoxCollider2D>();
        theBelt.enabled = false;
    }
    public override void Activate()
    {
        myAnimator.SetTrigger("Activate");
        theBelt.enabled = true;
    }

    public override void Deactivate()
    {
        myAnimator.SetTrigger("Deactivate");
        theBelt.enabled = false;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(pushDir * pushForce);
    }

#if UNITY_EDITOR
    public override void FlipX()
    {
        base.FlipX();
        pushDir = pushDir.SetX(-pushDir.x);
    }

    public override void FlipY()
    {
        base.FlipY();
        pushDir = pushDir.SetY(-pushDir.y);
    }

    public override void Rotate(float ang)
    {
        base.Rotate(ang);
        pushDir = pushDir.RotateDeg(ang);
    }
#endif
}
