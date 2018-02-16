using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class InteractiveObject : MapTile {
    static InteractiveObject selected;
    [SerializeField]
    string interaction = "Interact";
    [SerializeField]
    Color highlightColor = Color.red;
    protected SpriteRenderer myRenderer;

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

    public abstract void Interact();

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
                Selected = this;
            } else if (selected == this) {
                Selected = null;
            }
        }
    }

    public static InteractiveObject Selected {
        get {
            return selected;
        }

        private set {
            selected = value;
            EventManager.TriggerEvent("SetInteraction");
        }
    }

    public string Interaction {
        get {
            return interaction;
        }
    }

}
