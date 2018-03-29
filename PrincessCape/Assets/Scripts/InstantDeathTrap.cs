using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeathTrap : MapTile {

    /// <summary>
    /// Kill the Player when they collide with the spike.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Game.Instance.Player.TakeDamage(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) {
            Game.Instance.Player.TakeDamage(true);
        }
    }
}
