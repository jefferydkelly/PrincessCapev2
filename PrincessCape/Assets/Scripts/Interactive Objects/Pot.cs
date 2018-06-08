using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : HeldItem {
  
    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.collider.CompareTag("Player") && !Game.Instance.Player.IsHoldingItem)
		{
			IsHighlighted = true;
        } else if (myRigidbody.velocity.sqrMagnitude > 25) {
            gameObject.SetActive(false);
        }
    }
}
