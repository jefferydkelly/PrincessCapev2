using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : HeldItem{

   
    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

	/// <summary>
	/// Highlight the object when it touches the player
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (Game.Instance.IsPlaying && collision.collider.CompareTag("Player") && !Game.Instance.Player.IsHoldingItem)
		{
			Direction closest = collision.GetClosestDirection();
			if (closest == Direction.Left || closest == Direction.Right)
            {
                IsHighlighted = true;
            }
		}
	}
}
