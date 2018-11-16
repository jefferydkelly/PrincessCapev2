using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MapTile{
    [SerializeField]
    TextAsset cutscene;
    SpriteRenderer myRenderer;

    /// <summary>
    /// Starts a Cutscene when the player collides with the trigger
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && cutscene && Game.Instance.IsPlaying && Map.Instance.IsLoaded && !Game.Instance.IsInLevelEditor) {
            
            Cutscene.Instance.LoadTextFile(cutscene, true);
        }
    }

    /// <summary>
    /// Initializes the cutscene trigger
    /// </summary>
    public override void Init()
    {
        base.Init();
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.enabled = (Application.isPlaying && Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
    }

    /// <summary>
    /// Handles the changing of the game state
    /// </summary>
    /// <param name="state">State.</param>
    protected override void OnGameStateChanged(GameState state)
    {
        myRenderer.enabled = (Application.isPlaying && Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
    }

    /// <summary>
    /// Generates the save data for the trigger.
    /// </summary>
    /// <returns>The save data.</returns>
    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute<string>("Cutscene", cutscene.name);
        return data;
    }

    /// <summary>
    /// Sets the values of the cutscene trigger from the value in the tile struct.
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        string sceneName = PCLParser.ParseLine(tile.NextLine);
        cutscene = Resources.Load<TextAsset>("Cutscenes/" + sceneName);
    }
}
