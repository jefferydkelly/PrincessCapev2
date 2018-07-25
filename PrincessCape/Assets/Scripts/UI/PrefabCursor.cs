using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCursor : MonoBehaviour {

    SpriteRenderer myRenderer;
    BoxCollider2D myCollider;
    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.color = myRenderer.color.SetAlpha(0.25f).SetRed(0.0f).SetGreen(0.0f);

        myCollider = GetComponent<BoxCollider2D>();
    }
  
	// Update is called once per frame
	void Update () {
        if (myRenderer.sprite) {
            transform.position = Controller.Instance.MousePosition;
            float blue = LevelEditor.Instance.IsSpawnPositionOpen(transform.position) ? 1 : 0;
            myRenderer.color = myRenderer.color.SetBlue(blue).SetRed(1 - blue);
        }
	}

    public Sprite Sprite {
        set {
            myRenderer.sprite = value;

            if (value) {
                myCollider.size = value.bounds.size;
                myRenderer.color = myRenderer.color.SetAlpha(0.25f);
            }
        }

        get {
            return myRenderer.sprite;
        }
    }
}
