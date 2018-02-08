using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : ActivatedObject
{
    Animator myAnimator;
    BoxCollider2D theBelt;
    Vector2 pushDir = Vector2.right;
    float pushForce = 5;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        theBelt = GetComponent<BoxCollider2D>();
        theBelt.enabled = false;

        if (GetComponent<SpriteRenderer>().flipX) {
            pushDir *= -1;
        }
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
}
