using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MapTile {
    [SerializeField]
    string nextScene = "";

    /// <summary>
    /// When the Player collidesr with the Door, load the scene connected with the door.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && nextScene.Length > 0) {
            if (Game.Instance.IsInLevelEditor)
            {
                Game.Instance.StopInEditor();
            } else {
                EventManager.TriggerEvent("LevelOver");
                Game.Instance.LoadScene(nextScene);
            }
        }
    }

    /// <summary>
    /// Gets the scene the door leads to.
    /// </summary>
    /// <value>The next scene.</value>
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

    /// <summary>
    /// Creates a save data string for the door
    /// </summary>
    /// <returns>The save data.</returns>
    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Next Level", nextScene);
        return data;
    }

    /// <summary>
    /// Creates a door from given tile.
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        if (tile.info.Count > 3)
        {
            nextScene = PCLParser.ParseLine(tile.NextLine);
        }
    }
}
