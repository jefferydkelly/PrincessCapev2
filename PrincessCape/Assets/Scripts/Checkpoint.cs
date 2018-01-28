using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;

public class Checkpoint : MapTile {
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


    private void OnDisable()
    {
        EventManager.StopListening("CheckpointActivated", Deactivate);
    }

    /// <summary>
    /// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Activate();
        }
    }

	/// <summary>
	/// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) {
            Activate();
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

    void Activate() {
        if (!IsActive)
        {
            activeCheckpoint = this;
            IsActive = true;
            EventManager.TriggerEvent("CheckpointActivated");
            EventManager.StartListening("CheckpointActivated", Deactivate);
        }
    }

    void Deactivate() {
        IsActive = false;
        EventManager.StopListening("CheckpointActivated", Deactivate);
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

    public override string SaveData()
    {
		string info = "{\n";
		info += string.Format("\"Name\": \"{0}\"", name.Split('(')[0]) + lineEnding;
		info += string.Format("\"ID\": \"{0}\"", ID) + lineEnding;
		info += string.Format("\"Position\": \"{0}\"", transform.position) + lineEnding;
		info += string.Format("\"Rotation\": \"{0}\"", transform.rotation) + lineEnding;
		info += string.Format("\"Scale\": \"{0}\"", transform.localScale) + lineEnding;
        info += string.Format("\"Is First?\": \"{0}\"", isFirstCheckpoint) + lineEnding;
		info += "}" + lineEnding;
		return info;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        isFirstCheckpoint = PCLParser.ParseBool(tile.info[3]);

    }
}
