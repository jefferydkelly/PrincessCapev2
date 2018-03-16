using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MapTile{
    [SerializeField]
    TextAsset cutscene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && cutscene && Game.Instance.IsPlaying) {
            Cutscene.Instance.Load(cutscene);
            Cutscene.Instance.StartCutscene();
            gameObject.SetActive(false);
        }
    }

    public override void Init()
    {
        GetComponent<SpriteRenderer>().enabled = !Application.isPlaying;
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
