using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int platformLayers = -1;
    int fwd = 1;
    Rigidbody2D myRigidbody;
    SpriteRenderer myRenderer;
    Timer resetTimer;
    bool isFrozen = false;
    float maxSpeed = 4.0f;

    bool onLadder = false;
    bool aboveLadder = false;
    Ladder theLadder;
    PlayerState state = PlayerState.Normal;

    Vector2 storedVelocity;

    List<MagicItem> inventory;
    ItemLevel items = ItemLevel.None;
    bool initialized = false;
    private void Awake()
    {
        inventory = new List<MagicItem>();
        EventManager.StartListening("LevelLoaded", Reset);
        EventManager.StartListening("ShowMessage", () => { state = PlayerState.ReadingMessage; });
        EventManager.StartListening("ShowDialog", () => { state = PlayerState.ReadingMessage; });
        EventManager.StartListening("EndOfMessage", () => { state = PlayerState.Normal; });
        DontDestroyOnLoad(gameObject);
   
    }

    public void Init() {
        if (!initialized)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            myRenderer = GetComponent<SpriteRenderer>();
            resetTimer = new Timer(1.0f);
            resetTimer.OnComplete.AddListener(Game.Instance.Reset);

            platformLayers = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Hazard"));
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.Target = gameObject;
            }

            initialized = true;
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDied", Die);
        EventManager.StartListening("Pause", Pause);
        EventManager.StartListening("StartPush", StartPush);
    }

	private void OnDisable()
	{
        EventManager.StopListening("PlayerDied", Die);
		EventManager.StopListening("Pause", Pause);
	}

    /// <summary>
    /// Pause this instance.
    /// </summary>
    void Pause() {
        if (myRigidbody.constraints == RigidbodyConstraints2D.FreezeAll)
        {
			if (state != PlayerState.ReadingMessage)
			{
				myRigidbody.velocity = storedVelocity;
				myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
        }
        else
        {
            storedVelocity = myRigidbody.velocity;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

	// Update is called once per frame
	void Update () {
        if (!Game.Instance.IsPaused && !isFrozen && !IsPulling)
        {
            float xForce = Controller.Instance.Horizontal * 5;
			myRigidbody.AddForce(new Vector2(xForce, 0));
			myRigidbody.ClampXVelocity(maxSpeed);

            if (myRigidbody.velocity.x / fwd < 0) {
                
                if (state != PlayerState.MovingBlock)
                {
                    fwd *= -1;
                    myRenderer.flipX = !myRenderer.flipX;
                }
            }
            bool onGround = IsOnGround;

            if (onLadder) {
                //myRigidbody.AddForce(new Vector2(0, Controller.Instance.Vertical * 5));

                bool jump = Controller.Instance.Jump;
           
                if (onGround && !(jump && aboveLadder) && Controller.Instance.Vertical > 0) {
                    myRigidbody.gravityScale = 0;
				}
                else if (jump)
				{
					onLadder = false;
					Jump();
                    return;
				}
				else if (aboveLadder && Controller.Instance.Vertical < 0) {
                    myRigidbody.gravityScale = 0;
                    myRigidbody.velocity = myRigidbody.velocity.SetX(0);
                    transform.position = transform.position.SetX(theLadder.transform.position.x);
                    transform.position += Vector3.down * 0.5f;
                }

                if (!aboveLadder)
				{
					myRigidbody.velocity = myRigidbody.velocity.SetY(Controller.Instance.Vertical * maxSpeed / 1.5f);
				}
            }
            else if (CanJump && Controller.Instance.Jump)
            {
                Jump();
            } 
        }
	}

    private void LateUpdate()
    {
        if (state == PlayerState.MovingBlock) {
            myRigidbody.velocity = Vector3.right * Controller.Instance.Horizontal * maxSpeed / 4.0f;
        }
    }

    /// <summary>
    /// Makes the Player jump.
    /// </summary>
    void Jump() {
        state = PlayerState.Jumping;
        myRigidbody.gravityScale = 1.0f;
        myRigidbody.velocity = myRigidbody.velocity.SetY(7.0f);
        //myRigidbody.AddForce(Vector2.up * 7.0f, ForceMode2D.Impulse);
    }
    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is on ground.
    /// </summary>
    /// <value><c>true</c> If the player is on ground; otherwise, <c>false</c>.</value>
    bool IsOnGround {
        get
        {
            return Physics2D.BoxCast(transform.position, new Vector2(1.0f, 0.1f), 0, Vector2.down, 1.0f, platformLayers);
        }
    }

    bool CanJump {
        get {
            return IsOnGround && !IsUsingMagneticGloves && state != PlayerState.MovingBlock;
        }
    }

    /// <summary>
    /// Kill the player and reset the game.
    /// </summary>
    public void Die() {
        isFrozen = true;
        if (state == PlayerState.MovingBlock) {
            InteractiveObject.Selected.Interact();
        }
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

			

            EventManager.TriggerEvent("PlayerLanded");
        }

		if (collision.collider.CompareTag("Ladder Top"))
		{
			aboveLadder = true;
            theLadder = collision.gameObject.GetComponentInParent<Ladder>();
        } else if (collision.collider.CompareTag("Projectile")) {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (IsUsingShield) {
                projectile.Fwd = Controller.Instance.DirectionalInput;
            } else {
                Die();
            }
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
        if (!IsUsingMagneticGloves)
        {
            if (collision.CompareTag("Ladder"))
            {
                onLadder = true;
                theLadder = collision.GetComponent<Ladder>();
            }
        }
    }

	public void OnTriggerExit2D(Collider2D collision)
	{
        if (!IsUsingMagneticGloves)
        {
            if (collision.CompareTag("Ladder"))
            {
                onLadder = false;
                myRigidbody.gravityScale = 1;
                if (!IsJumping)
                {
                    myRigidbody.velocity = myRigidbody.velocity.SetY(0);
                }
            }
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
    /// Gets the velocity.
    /// </summary>
    /// <value>The velocity.</value>
    public Vector2 Velocity {
        get {
            return myRigidbody.velocity;
        }
    }

    /// <summary>
    /// Gets the forward.
    /// </summary>
    /// <value>The forward.</value>
    public Vector3 Forward {
        get {
            return new Vector3(fwd, 0);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is dead.
    /// </summary>
    /// <value><c>true</c> if is dead; otherwise, <c>false</c>.</value>
    public bool IsDead {
        get {
            return state == PlayerState.Dead;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Player"/> is floating.
    /// </summary>
    /// <value><c>true</c> if is floating; otherwise, <c>false</c>.</value>
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

    public bool IsUsingShield {
        get {
            return state == PlayerState.UsingShield;
        }

        set {
            if (value && !(IsDead || isFrozen)) {
                state = PlayerState.UsingShield;
                isFrozen = true;
            } else if (!value && IsUsingShield) {
                state = PlayerState.Normal;
                isFrozen = false;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Player"/> is pulling.
    /// </summary>
    /// <value><c>true</c> if is pulling; otherwise, <c>false</c>.</value>
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
                state = PlayerState.Pulling;
			}
            else if (!value && IsPulling)
			{
				state = PlayerState.Normal;
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:Player"/> is pushing.
	/// </summary>
	/// <value><c>true</c> if is pushing; otherwise, <c>false</c>.</value>
	public bool IsPushing
	{
		get
		{
            return state == PlayerState.Pushing;
		}

		set
		{
			if (value && (!IsDead || isFrozen))
			{
				state = PlayerState.Pushing;
			}
			else if (!value && IsPushing)
			{
				state = PlayerState.Normal;
			}
		}
	}

    public bool IsUsingMagneticGloves {
        get {
            return state == PlayerState.Pulling || state == PlayerState.Pushing;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is jumping.
    /// </summary>
    /// <value><c>true</c> if is jumping; otherwise, <c>false</c>.</value>
    public bool IsJumping {
        get {
            return state == PlayerState.Jumping;
        }
    }

    /// <summary>
    /// Gets the inventory.
    /// </summary>
    /// <value>The inventory.</value>
    public List<MagicItem> Inventory {
        get {
            return inventory;
        }
    }

    /// <summary>
    /// Adds the item to the Player's inventory
    /// </summary>
    /// <param name="mi">Mi.</param>
    public void AddItem(MagicItem mi, bool showMessage = false) {
        inventory.Add(mi);

        if (Inventory.Count == 1) {
            mi.RegisterItemOne();
        } else if (Inventory.Count == 2) {
            if (inventory[0].Slot == MagicItemSlot.First) {
                mi.RegisterItemTwo();
            } else {
                mi.RegisterItemOne();
            }
        }
        items = (ItemLevel)System.Enum.Parse(typeof(ItemLevel), mi.ItemName.Replace(" ", string.Empty));
        if (showMessage)
        {
            MessageBox.SetMessage(mi.ItemGetMessage);
            EventManager.TriggerEvent("ShowMessage");
            EventManager.TriggerEvent("Pause");
            EventManager.StartListening("EndOfMessage", EndCutscene);
        }
    }

    /// <summary>
    /// Event Handler for the end of a cutscene
    /// </summary>
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
        EventManager.StartListening("StartPush", StartPush);
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public ItemLevel Items {
        get {
            return items;
        }
    }
}

public enum PlayerState {
    Normal,
    Dead,
    Jumping,
    Floating,
    Pushing,
    Pulling,
    MovingBlock,
    UsingShield,
    Frozen,
    ReadingMessage
}
