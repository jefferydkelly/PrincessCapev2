using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MapTile{
    [SerializeField]
    TextAsset cutscene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && cutscene) {
            Cutscene.Instance.Load(cutscene);
            Cutscene.Instance.StartCutscene();
        }
    }

    public override void Init()
    {
        GetComponent<SpriteRenderer>().enabled = !Application.isPlaying;
    }
}
