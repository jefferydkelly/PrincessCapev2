using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : ActivatedObject {

    /// <summary>
    /// Causes the spike to rise up
    /// </summary>
    public override void Activate()
    {
        if (Application.isPlaying)
        {
            IsActivated = true;
        }
    }

    /// <summary>
    /// Lowers the spike
    /// </summary>
    public override void Deactivate()
	{
        if (Application.isPlaying)
        {
            IsActivated = false;
        }
    }

    /// <summary>
    /// Handles the collision of the spike with the player
    /// </summary>
    /// <param name="collision">Collision.</param>
    public void OnTriggerEnter2D(Collider2D collision) {
       
        if (collision.CompareTag("Player")) {
            SoundManager.Instance.PlaySound("splat");
            Game.Instance.Player.TakeDamage(true);
        }
    }

    /// <summary>
    /// Handles the collision with any weighted object that would cause it to activate
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (!IsConnected && collision.rigidbody.mass > 0.5f)
		{
			IncrementActivator();
		}
    }

    /// <summary>
    /// Handles the end of the collision with a weighted object and checks if there is nothing on top
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
		if (!IsConnected && collision.rigidbody.mass > 0.5f)
		{
			DecrementActivator();
		}
    }

    /// <summary>
    /// Initializes the instance of Spike Platform
    /// </summary>
    public override void Init()
    {
        base.Init();
        initialized = true;

		if (startActive) {
			Activate();
			IsActivated = true;
		}
    }

    private void Awake()
    {
        Init();
    }
}
