﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MapTile{
    [SerializeField]
    TextAsset cutscene;
    SpriteRenderer myRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag("Player") && cutscene && Game.Instance.IsPlaying && Map.Instance.IsLoaded) {
            
            Cutscene.Instance.Load(cutscene, true);
        }
    }

    public override void Init()
    {
        base.Init();
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.enabled = !(Application.isPlaying && Game.Instance.IsPlaying);
    }

    protected override void OnGameStateChanged(GameState state)
    {
        myRenderer.enabled = !(Application.isPlaying && Game.Instance.IsPlaying);
    }

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute<string>("Cutscene", cutscene.name);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        string sceneName = PCLParser.ParseLine(tile.NextLine);
        cutscene = Resources.Load<TextAsset>("Cutscenes/" + sceneName);
    }
}
