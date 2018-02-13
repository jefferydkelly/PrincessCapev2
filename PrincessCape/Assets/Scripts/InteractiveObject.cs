using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : ActivatorObject {
    static InteractiveObject selected;
    [SerializeField]
    Color highlightColor = Color.red;
    SpriteRenderer myRenderer;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Highlight the object when it touches the player
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) {
            IsHighlighted = true;
        }
    }


    /// <summary>
    /// Remove the highlight when the player is not colliding with it.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
		if (collision.collider.CompareTag("Player"))
		{
            IsHighlighted = false;
		}
    }

	/// <summary>
	/// Highlight the object when it touches the player
	/// </summary>
	/// <param name="collision">Collision.</param>
	public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            IsHighlighted = true;
        }
    }

	/// <summary>
	/// Remove the highlight when the player is not colliding with it.
	/// </summary>
	/// <param name="collision">Collision.</param>
	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
            IsHighlighted = false;
		}
	}

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:InteractiveObject"/> is highlighted.
    /// </summary>
    /// <value><c>true</c> if is highlighted; otherwise, <c>false</c>.</value>
    public bool IsHighlighted {
        get {
            return myRenderer.color == highlightColor;
        }

        protected set {
            myRenderer.color = value ? highlightColor : Color.white;
            if (value) {
                selected = this;
            } else if (selected == this) {
                selected = null;
            }
        }
    }

    public static InteractiveObject Selected {
        get {
            return selected;
        }
    }
}
