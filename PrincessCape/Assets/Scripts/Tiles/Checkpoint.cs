using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Checkpoint : MapTile
{
    static Checkpoint activeCheckpoint;
    static UnityEvent onCheckpointActivate = new UnityEvent();
    bool isActive = false;
    Animator myAnimator;
    [SerializeField]
    bool isFirstCheckpoint;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Initializes the checkpoint
    /// </summary>
    public override void Init() {
		myAnimator = GetComponent<Animator>();

		if (Application.isEditor && !Application.isPlaying)
		{
			if (FindObjectsOfType<Checkpoint>().Length == 1)
			{
				isFirstCheckpoint = true;
			}
		}
		else if (isFirstCheckpoint)
		{
			activeCheckpoint = this;

            if (Game.Instance.IsInLevelEditor) {
                Game.Instance.Player.OnDie.AddListener(() =>
                {
                    activeCheckpoint = this;
                });

                Game.Instance.OnEditorStop.AddListener(() =>
                {
                    activeCheckpoint = this;

                });
            }
		}
    }

    /// <summary>
    /// Removes the event when the checkpoint is disabled
    /// </summary>
    private void OnDisable()
    {
        onCheckpointActivate.RemoveListener(Deactivate);
    }

    /// <summary>
    /// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Activate();
        }
    }

    /// <summary>
    /// Set the Checkpoint as the ActiveCheckpoint when the player collides with it.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Activate();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Checkpoint"/> is active.
    /// </summary>
    /// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
    bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
            myAnimator.SetBool("IsActivated", value);
        }
    }

    /// <summary>
    /// Activate this instance.
    /// </summary>
    void Activate()
    {
        if (!IsActive)
        {
            activeCheckpoint = this;
            IsActive = true;
            onCheckpointActivate.Invoke();
            onCheckpointActivate.AddListener(Deactivate);
        }
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    void Deactivate()
    {
        IsActive = false;
        onCheckpointActivate.RemoveListener(Deactivate);
    }

    /// <summary>
    /// Gets the position of the Active Checkpoint
    /// </summary>
    /// <value>The reset position.</value>
    public static Vector3 ResetPosition
    {
        get
        {
            if (!activeCheckpoint)
            {
                return Vector3.zero;
            }
            return activeCheckpoint.transform.position.SetZ(0);
        }
    }

    /// <summary>
    /// Gets the active checkpoint's gameobject.
    /// </summary>
    /// <value>The gameobject of the active checkpoint.</value>
    public static GameObject Active
    {
        get
        {
            return activeCheckpoint.gameObject;
        }
    }

    /// <summary>
    /// Generates a save data string for the checkpoint
    /// </summary>
    /// <returns>The save data.</returns>
    protected override string GenerateSaveData()
    {
        string data =  base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Is First?", isFirstCheckpoint);
        return data;
    }

    /// <summary>
    /// Creates a Checkpoint from the TileStruct
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        isFirstCheckpoint = PCLParser.ParseBool(tile.NextLine);

        if (isFirstCheckpoint && Application.isPlaying) {
            activeCheckpoint = this;
        }
    }
}
