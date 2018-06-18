using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MapTile {

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.GetClosestDirection() == Direction.Down && collision.rigidbody)
		{
			collision.rigidbody.AddForce(transform.up * 12.5f, ForceMode2D.Impulse);
		} else if (collision.collider.CompareTag("Player")) {
			
		}
	}
}
