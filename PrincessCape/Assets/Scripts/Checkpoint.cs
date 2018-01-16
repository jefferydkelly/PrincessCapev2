using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class Checkpoint : MapTile {
    static List<Checkpoint> checkpoints;
    static Checkpoint activeCheckpoint;
    bool isActive = false;
    Animator myAnimator;
    [SerializeField]
    bool isFirstCheckpoint;

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

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
		if (checkpoints == null || !checkpoints.Contains(this))
		{
			checkpoints = FindObjectsOfType<Checkpoint>().ToList();
		}
    }

    /// <summary>
    /// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            IsActive = true;
        }
    }

	/// <summary>
	/// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) {
            IsActive = true;
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
            if (value) {
                foreach(Checkpoint c in checkpoints) {
                    c.IsActive = false;
                }
                activeCheckpoint = this;
            }
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
