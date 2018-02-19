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
            
            Game.Instance.LoadScene(nextScene);
        }
    }

    public string NextScene {
        set {
            if (value.Length > 0 && Application.isEditor && !Application.isPlaying)
            {
                nextScene = ParseScene(value);
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
        nextScene = ParseScene(PCLParser.ParseLine(tile.info[3]));
    }

    string ParseScene(string scenePath) {
		int rind = scenePath.IndexOf("Resources", System.StringComparison.Ordinal);
        return scenePath;
        /*
		if (rind > 0)
		{
			return scenePath.Substring(rind + 10);
		}
		else
		{
			return nextScene = scenePath;
		}*/
    }
}
