using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : ActivatedObject {
    Animator myAnimator;
    public override void Activate()
    {
        isActivated = true;
        myAnimator.SetBool("isActivated", true);
    }

    public override void Deactivate()
    {
        isActivated = false;
		myAnimator.SetBool("isActivated", false);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            EventManager.TriggerEvent("PlayerDied");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Activate();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Deactivate();
    }

    public override void Init()
    {
        myAnimator = GetComponent<Animator>();
    }
}
