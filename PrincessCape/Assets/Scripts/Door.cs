using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MapTile {
    string nextScene = "";

    /// <summary>
    /// When the Player collidesr with the Door, load the scene connected with the door.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && nextScene.Length > 0) {
            Game.Instance.LoadScene(nextScene);
        }
    }

    public string NextScene {
        set {
            if (value.Length > 0 && Application.isEditor && !Application.isPlaying)
            {
                nextScene = value;
            }
        }

        get {
            return nextScene;
        }
    }
}
