using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MapTile {
    [SerializeField]
    string nextScene;

    /// <summary>
    /// When the Player collidesr with the Door, load the scene connected with the door.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Game.Instance.LoadScene(nextScene);
        }
    }
}
