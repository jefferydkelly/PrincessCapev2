using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCursor : MonoBehaviour {

    SpriteRenderer myRenderer;
    SpriteRenderer circleRenderer;
    BoxCollider2D myCollider;
    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        circleRenderer = GetComponentsInChildren<SpriteRenderer>()[1];

        myRenderer.color = myRenderer.color.SetAlpha(0.25f);
        circleRenderer.color = new Color(0, 1, 0, 0.25f);
        circleRenderer.gameObject.SetActive(false);

        myCollider = GetComponent<BoxCollider2D>();
    }
  
	// Update is called once per frame
	void Update () {
        if (myRenderer.sprite) {
            transform.position = Controller.Instance.MousePosition;
            float green = LevelEditor.Instance.IsSpawnPositionOpen(transform.position) ? 1 : 0;
            circleRenderer.color = circleRenderer.color.SetGreen(green).SetRed(1 - green);
        }
	}

    /// <summary>
    /// Gets or sets the sprite of the prefab cursor.
    /// </summary>
    /// <value>The sprite.</value>
    public Sprite Sprite {
        set {
            myRenderer.sprite = value;
            circleRenderer.gameObject.SetActive(value);
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
