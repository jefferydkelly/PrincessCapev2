using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int platformLayers = -1;
    Rigidbody2D myRigidbody;
    Timer resetTimer;
    bool isFrozen = false;
    float maxSpeed = 3.0f;

    bool onLadder = false;
    bool aboveLadder = false;
    PlayerState state = PlayerState.Normal;

    Vector2 storedVelocity;

    List<MagicItem> inventory;
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        resetTimer = new Timer(1.0f);
        resetTimer.OnComplete.AddListener(Game.Instance.Reset);
        inventory = new List<MagicItem>();
   
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDied", Die);
        EventManager.StartListening("Pause", Pause);
        EventManager.StartListening("Unpause", Unpause);
        EventManager.StartListening("StartPush", StartPush);
    }

	private void OnDisable()
	{
        EventManager.StopListening("PlayerDied", Die);
		EventManager.StopListening("Pause", Pause);
        EventManager.StopListening("Unpause", Unpause);
	}

    void Pause() {
        storedVelocity = myRigidbody.velocity;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Unpause() {
        myRigidbody.velocity = storedVelocity;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    // Use this for initialization
    void Start () {
        platformLayers = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Hazard"));
        CameraManager.Instance.Target = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (!Game.Instance.IsPaused && !isFrozen && !IsPulling)
        {
            float xForce = Controller.Instance.Horizontal * 5;
            bool onGround = IsOnGround;

            if (onLadder) {
                myRigidbody.AddForce(new Vector2(0, Controller.Instance.Vertical * 5));

                if (onGround && Controller.Instance.Vertical > 0) {
                    myRigidbody.gravityScale = 0;
                } else if (aboveLadder && Controller.Instance.Vertical < 0) {
                    myRigidbody.gravityScale = 0;
                    transform.position += Vector3.down * 0.25f;
                }           
            } else if (state == PlayerState.MovingBlock) {
                myRigidbody.velocity = Vector3.right * Controller.Instance.Horizontal * maxSpeed / 4.0f;
                xForce = 0;

            }
            else if (onGround && Controller.Instance.Jump)
            {
                myRigidbody.AddForce(Vector2.up * 6.5f, ForceMode2D.Impulse);
            } 

            if (Input.GetKeyDown(KeyCode.F) && InteractiveObject.Selected) {
                InteractiveObject.Selected.Activate();
            }

            myRigidbody.AddForce(new Vector2(xForce, 0));
			myRigidbody.ClampXVelocity(maxSpeed);
        }
	}

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is on ground.
    /// </summary>
    /// <value><c>true</c> If the player is on ground; otherwise, <c>false</c>.</value>
    bool IsOnGround {
        get
        {
            return Physics2D.BoxCast(transform.position, new Vector2(1.0f, 0.1f), 0, Vector2.down, 0.5f, platformLayers);
        }
    }

    /// <summary>
    /// Kill the player and reset the game.
    /// </summary>
    public void Die() {
        isFrozen = true;
        resetTimer.Start();
    }

    /// <summary>
    /// Reset the player to the last checkpoint.
    /// </summary>
    public void Reset() {
        EventManager.TriggerEvent("PlayerRespawned");
        transform.position = Checkpoint.ResetPosition;
        isFrozen = false;
        myRigidbody.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0) {
            myRigidbody.gravityScale = 1;

			if (collision.collider.CompareTag("Ladder Top"))
			{
				aboveLadder = true;
			}

            EventManager.TriggerEvent("PlayerLanded");
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
		if (collision.collider.CompareTag("Ladder Top"))
		{
            aboveLadder = false;
		}
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) {
            onLadder = true;
        }
    }

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Ladder"))
		{
            onLadder = false;
            myRigidbody.gravityScale = 1;
		}
	}

    public Rigidbody2D Rigidbody {
        get {
            return myRigidbody;
        }
    }

    public bool IsDead {
        get {
            return state == PlayerState.Dead;
        }
    }

    public bool IsFloating {
        get {
            return state == PlayerState.Floating;
        }

        set {
            if (value && (!IsDead || isFrozen)) {
                state = PlayerState.Floating;
            } else if (!value && IsFloating) {
                state = PlayerState.Normal;
            }
        }
    }

	public bool IsPulling
	{
		get
		{
            return state == PlayerState.Pulling;
		}

		set
		{
			if (value && (!IsDead || isFrozen))
			{
				state = PlayerState.Floating;
			}
            else if (!value && IsPulling)
			{
				state = PlayerState.Normal;
			}
		}
	}

    public List<MagicItem> Inventory {
        get {
            return inventory;
        }
    }

    public void AddItem(MagicItem mi) {
        inventory.Add(mi);

        if (Inventory.Count == 1) {
            mi.RegisterItemOne();
        }
        MessageBox.SetMessage(mi.ItemGetMessage);
        EventManager.TriggerEvent("ShowMessage");
        EventManager.StartListening("EndOfMessage", EndCutscene);
    }

    void EndCutscene() {
        EventManager.StopListening("EndOfMessage", EndCutscene);
        EventManager.TriggerEvent("HideMessage");
        EventManager.TriggerEvent("Unpause");
    }

    void StartPush() {
        if (IsOnGround) {
            state = PlayerState.MovingBlock;
            EventManager.StopListening("StartPush", StartPush);
            EventManager.StartListening("StopPush", StopPush);
        }
    }

    void StopPush() {
        state = PlayerState.Normal;
        EventManager.StopListening("StopPush", StopPush);
        EventManager.StartListening("StartPush", Start);
    }
}

public enum PlayerState {
    Normal,
    Dead,
    Floating,
    Pushing,
    Pulling,
    MovingBlock,
    Frozen
}
