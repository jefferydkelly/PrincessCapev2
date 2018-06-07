using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour{
    SpriteRenderer myRenderer;
    Rigidbody2D myRigidbody;
    Vector3 startPosition;
    static Metal highlighted;

	public void Start()
	{
		Init();
	}

	public void Init() {

		myRenderer = GetComponent<SpriteRenderer>();
		myRigidbody = GetComponent<Rigidbody2D>();
		if (!IsStatic)
		{
			startPosition = transform.position;
			Game.Instance.Player.OnRespawn.AddListener(Reset);
		}
    }

    /// <summary>
    /// Reset this instance to its starting position
    /// </summary>
    private void Reset()
    {
        transform.position = startPosition;
		Clear();
    }

    /// <summary>
    /// When the mouse moves over the Metal, highlight it.
    /// </summary>
    private void OnMouseEnter()
    {
        myRenderer.color = Color.red;
        highlighted = this;
    }

    private void OnMouseOver()
    {
        if (highlighted == null && myRenderer.color == Color.white) {
            myRenderer.color = Color.red;
            highlighted = this;
        }
    }

    /// <summary>
    /// When the mouse leaves the metal, remove the highlight
    /// </summary>
    private void OnMouseExit()
    {
        if (!Game.Instance.Player.IsUsingMagneticGloves)
        {
            Clear();
        }
    }

    private void OnMouseUp()
    {
        Clear();
    }

    public void Clear() {
		myRenderer.color = Color.white;
		if (highlighted == this)
		{
			highlighted = null;
		}
    }

    /// <summary>
    /// Gets the highlighted block.
    /// </summary>
    /// <value>The highlighted block.</value>
    public static Metal HighlightedBlock {
        get {
            return highlighted;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Metal"/> is static.
    /// </summary>
    /// <value><c>true</c> if is static; otherwise, <c>false</c>.</value>
    public bool IsStatic {
        get {
            return myRigidbody == null;
        }
    }

    /// <summary>
    /// Gets the rigidbody.
    /// </summary>
    /// <value>The rigidbody.</value>
    public Rigidbody2D Rigidbody {
        get {
            return myRigidbody;
        }
    }

    /// <summary>
    /// Gets or sets the velocity.
    /// </summary>
    /// <value>The velocity.</value>
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
