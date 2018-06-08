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
		if (collision.collider.CompareTag("Player") && !Game.Instance.Player.IsHoldingItem)
		{
            
            Vector3 dif = collision.transform.position - transform.position;
            if (Vector3.Dot(Vector3.up, dif) <= 0.55f)
            {
                IsHighlighted = true;
            }
		}
	}

}
