using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : InteractiveObject
{
    protected Rigidbody2D myRigidbody;
    protected Collider2D myCollider;
    protected bool isHeld = false;
    //int hitMasks;
	Vector3 startPosition;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		startPosition = transform.position;
		Game.Instance.Player.OnRespawn.AddListener(Reset);

        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        //hitMasks = ~(1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("UI"));

        if (Game.Instance.IsInLevelEditor) {
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

            Game.Instance.OnGameStateChanged.AddListener((GameState gameState) =>
            {
                myRigidbody.constraints = (gameState == GameState.Playing) ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
            });
        }
	}

    /// <summary>
    /// Reset this instance to its start position
    /// </summary>
	private void Reset()
	{
		gameObject.SetActive(true);
		transform.position = startPosition;
	}

    /// <summary>
    /// Drops the held object
    /// </summary>
	public void Drop()
    {
        Game.Instance.Player.HeldItem = null;
        UIManager.Instance.SetInteractionText("");
        myRigidbody.gravityScale = 1;
        //transform.position += Game.Instance.Player.Forward * 0.1f;
        isHeld = false;

		if (!IsHeavy && Mathf.Abs(Game.Instance.Player.Velocity.x) >= 0.25f)
		{
			Throw();
		}

        EventManager.StopListening("Interact", Drop);

    }

    /// <summary>
    /// Throw the held item according to the player's forward.
    /// </summary>
    public void Throw() {
        myRigidbody.AddForce(Game.Instance.Player.Forward * 6.25f, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Picks up the HeldObject
    /// </summary>
    public override void Interact()
    {
        if (!isHeld)
        {
            Game.Instance.Player.HeldItem = this;
            myRigidbody.gravityScale = 0;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            isHeld = true;
            IsHighlighted = false;
            EventManager.StartListening("Interact", Drop);
        }   
    }

    /// <summary>
    /// Update the Interaction Text.
    /// </summary>
    public void Update()
    {
        if (isHeld)
        {
            if (!IsHeavy && Mathf.Abs(Game.Instance.Player.Velocity.x) >= 0.25f)
            {
                UIManager.Instance.SetInteractionText("Throw");
            }
            else
            {
                UIManager.Instance.SetInteractionText("Drop");
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:HeldItem"/> is heavy.
    /// </summary>
    /// <value><c>true</c> if is heavy; otherwise, <c>false</c>.</value>
    public bool IsHeavy {
        get {
            return myRigidbody.mass > 1.0f;
        }
    }

    /// <summary>
    /// Gets half the height of the Held Item.
    /// </summary>
    /// <value>The height of the half.</value>
    public float HalfHeight {
        get {
            return myRenderer.bounds.extents.y;
        }
    }

    /// <summary>
    /// Gets or sets the velocity.
    /// </summary>
    /// <value>The velocity.</value>
    public Vector3 Velocity {
        get {
            return myRigidbody.velocity;
        }

        set {
            myRigidbody.velocity = value;
        }
    }
}
