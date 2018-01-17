using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;

public class CheckPointActivatedEvent: UnityEvent<Checkpoint> {
    
}
public class Checkpoint : MapTile {
    static Checkpoint activeCheckpoint;
    bool isActive = false;
    Animator myAnimator;
    [SerializeField]
    bool isFirstCheckpoint;

    static CheckPointActivatedEvent OnCheckpointActivated = new CheckPointActivatedEvent();
    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        if (isFirstCheckpoint) {
            activeCheckpoint = this;
        }
     }

    private void OnEnable()
    {
        OnCheckpointActivated.AddListener(Activated);
    }

    private void OnDisable()
    {
        OnCheckpointActivated.RemoveListener(Activated);
    }

    void Activated(Checkpoint c) {
        IsActive = (c == this);
    }

    /// <summary>
    /// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            activeCheckpoint = this;
            OnCheckpointActivated.Invoke(this);
        }
    }

	/// <summary>
	/// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) {
            activeCheckpoint = this;
            OnCheckpointActivated.Invoke(this);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Checkpoint"/> is active.
    /// </summary>
    /// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
    bool IsActive {
        get {
            return isActive;
        }

        set {
            isActive = value;
            myAnimator.SetBool("isActive", value);
        }
    }

    /// <summary>
    /// Gets the position of the Active Checkpoint
    /// </summary>
    /// <value>The reset position.</value>
    public static Vector3 ResetPosition {
        get {
            return activeCheckpoint.transform.position;
        }
    }

    /// <summary>
    /// Gets the active checkpoint's gameobject.
    /// </summary>
    /// <value>The gameobject of the active checkpoint.</value>
    public static GameObject Active {
        get {
            return activeCheckpoint.gameObject;
        }
    }
}
