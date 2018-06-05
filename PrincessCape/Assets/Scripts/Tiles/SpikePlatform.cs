using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : ActivatedObject {
    Animator myAnimator;
    public override void Activate()
    {
        if (Application.isPlaying)
        {
            myAnimator.SetBool("isActivated", true);
        }
    }

    public override void Deactivate()
	{
        if (Application.isPlaying)
        {
            myAnimator.SetBool("isActivated", false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
       
        if (collision.CompareTag("Player")) {
            Game.Instance.Player.TakeDamage(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (!isConnected && collision.rigidbody.mass > 0.5f)
		{
			IncrementActivator();
		}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
		if (!isConnected && collision.rigidbody.mass > 0.5f)
		{
			DecrementActivator();
		}
    }

    public override void Init()
    {
        myAnimator = GetComponent<Animator>();
        initialized = true;

		if (startActive) {
			Activate();
			isActivated = true;
		}
    }

    private void Awake()
    {
        Init();
    }
}
