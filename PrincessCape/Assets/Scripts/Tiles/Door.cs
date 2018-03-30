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
            EventManager.TriggerEvent("LevelOver");
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

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Next Level", nextScene);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        if (tile.info.Count > 3)
        {
            nextScene = PCLParser.ParseLine(tile.NextLine);
        }
    }
}
