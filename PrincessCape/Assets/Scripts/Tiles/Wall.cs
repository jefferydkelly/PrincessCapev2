using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MapTile {
    [SerializeField]
    GameObject regularWall;
    [SerializeField]
    Vector2 size = Vector2.one;
    BoxCollider2D myCollider;

    public override void Init()
    {
        base.Init();
        myCollider = GetComponent<BoxCollider2D>();
    }

    public override void ScaleX(bool right)
    {
        if (right) {
            Width += 1;
        } else {
            Width -= 1;
        }
    }

    public override void ScaleY(bool up)
    {
        if (up) {
            Height += 1;
        } else {
            Height -= 1;
        }
    }

    int Width {
        get {
            return (int)size.x;
        }

        set {
            size.x = Mathf.Max(value, 1);
            Resize();
        }
    }

    int Height {
        get {
            return (int)size.y;
        }

        set {
            size.y = Mathf.Max(value, 1);
            Resize();
        }
    }

    int NumTiles {
        get {
            return Width * Height;
        }
    }

    void Resize() {
        while(transform.childCount < NumTiles) {
            GameObject newTile = Instantiate(regularWall);
            newTile.transform.SetParent(transform);
        }

        while (transform.childCount > NumTiles) {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject, false);
        }

        for (int i = 0; i < Height; i++) {
            for (int j = 0; j < Width; j++) {
                transform.GetChild(Width * i + j).transform.localPosition = new Vector3(j, i, 0);
            }
        }
        myCollider.size = new Vector2(Width, Height);
        myCollider.offset = new Vector2(Width - 1, Height - 1) / 2;
    }

    public override Vector3 Center
    {
        get
        {
            return transform.position + new Vector3(Width - 1, Height - 1) / 2;
        }
    }
}
