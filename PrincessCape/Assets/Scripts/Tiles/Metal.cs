using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MapTile {
    SpriteRenderer myRenderer;
    Rigidbody2D myRigidbody;
    static Metal highlighted;
    public void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnMouseEnter()
    {
        myRenderer.color = Color.red;
        highlighted = this;
    }

    private void OnMouseExit()
    {
        myRenderer.color = Color.white;
        if (highlighted == this) {
            highlighted = null;
        }
    }

    public static Metal HighlightedBlock {
        get {
            return highlighted;
        }
    }

    public bool IsStatic {
        get {
            return myRigidbody == null;
        }
    }

    public Rigidbody2D Rigidbody {
        get {
            return myRigidbody;
        }
    }

    public Vector2 Velocity {
        set {
            if (myRigidbody) {
                myRigidbody.velocity = value;
            }
        }

        get {
            return myRigidbody ? myRigidbody.velocity : Vector2.zero;
        }
    }
}
