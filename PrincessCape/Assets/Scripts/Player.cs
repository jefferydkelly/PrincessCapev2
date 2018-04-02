﻿using System.Collections;
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
    HeldItem heldItem;

    //Health
    int currentHealth = 3;
    int maxHealth = 3;
    bool isInvincible = false;
    Timer invincibilityTimer;

    private void Awake()
    {
        inventory = new List<MagicItem>();
        EventManager.StartListening("LevelLoaded", Reset);
        EventManager.StartListening("ShowMessage", () => {
            isFrozen = true;
            state = PlayerState.ReadingMessage; 
        });
             
        EventManager.StartListening("ShowDialog", () => {
            isFrozen = true;
            state = PlayerState.ReadingMessage; 
        });

        EventManager.StartListening("EndOfMessage", ()=> {
			
            if (!Game.Instance.IsInCutscene)
            {
				isFrozen = false;
				state = PlayerState.Normal;
                EventManager.TriggerEvent("HideMessage");
            }

        });
        EventManager.StartListening("EndCutscene", EndCutscene);

        DontDestroyOnLoad(gameObject);
   
    }

    /// <summary>
    /// Initializes the player and the basic components
    /// </summary>
    public void Init() {
        if (!initialized)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            myRenderer = GetComponent<SpriteRenderer>();
            resetTimer = new Timer(1.0f);
            resetTimer.OnComplete.AddListener(Game.Instance.Reset);

            //Exlcude these Player, Background and Hazard layers so the player can't jump off any of them
            platformLayers = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Hazard"));
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.Target = gameObject;
            }
            //Updates the health bar in the UI
            EventManager.TriggerEvent("IncreaseHealth");
            invincibilityTimer = new Timer(0.5f);
            invincibilityTimer.OnComplete.AddListener(()=> {
                isInvincible = false;
                myRenderer.color = myRenderer.color.SetAlpha(1.0f);
            });
            initialized = true;
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("Pause", Pause);
    }

	private void OnDisable()
	{
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
        //If the player isn't visible on the screen, call PlayerOffscreen to recenter the camera
        if (!myRenderer.isVisible) {
            EventManager.TriggerEvent("PlayerOffscreen");
        }

        if (Game.Instance.IsPlaying && !isFrozen && !IsPulling)
        {
            float xForce = Controller.Instance.Horizontal * 5;
			myRigidbody.AddForce(new Vector2(xForce, 0));
			myRigidbody.ClampXVelocity(maxSpeed);

            if (myRigidbody.velocity.x / fwd < 0) {
                
                if (!(IsHoldingItem && heldItem.IsHeavy))
                {
                    fwd *= -1;
                    myRenderer.flipX = !myRenderer.flipX;
                }
            }
            if (IsHoldingItem) {
                heldItem.transform.position = transform.position + Vector3.right * fwd;
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
                    aboveLadder = false;
                    myRigidbody.gravityScale = 0;
                    myRigidbody.velocity = myRigidbody.velocity.SetX(0);
                    transform.position = transform.position.SetX(theLadder.transform.position.x);
                    transform.position += Vector3.down * 2;
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
            return Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.1f), 0, Vector2.down, 1.0f, platformLayers);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> can jump.
    /// </summary>
    /// <value><c>true</c> if can jump; otherwise, <c>false</c>.</value>
    bool CanJump {
        get {
            return IsOnGround && !IsUsingMagneticGloves;
        }
    }

    /// <summary>
    /// Kill the player and reset the game.
    /// </summary>
    public void Die() {
        state = PlayerState.Dead;
        BeginReset();
    }

    /// <summary>
    /// Deals damage to the player and causes the player to die if the currentHealth == 0
    /// </summary>
    /// <param name="autoReset">If set to <c>true</c>, the player will reset to the last checkpoint automatically.</param>
    public void TakeDamage(bool autoReset = false) {
        if (!isInvincible)
        {
            currentHealth--;
            EventManager.TriggerEvent("TakeDamage");
            isInvincible = true;
            if (currentHealth == 0)
            {
                Die();
            }
            else if (autoReset)
            {
                BeginReset();
            } else {
                myRenderer.color = myRenderer.color.SetAlpha(0.5f);
                invincibilityTimer.Start();;
            }
        }
    }

    /// <summary>
    /// Starts resetting the player.
    /// </summary>
    void BeginReset() {
		isFrozen = true;
		EventManager.TriggerEvent("ItemOneDeactivated");
		EventManager.TriggerEvent("ItemTwoDeactivated");
        if (IsHoldingItem) {
            heldItem.Drop();
        }
        resetTimer.Start();
    }

    /// <summary>
    /// Reset the player to the last checkpoint.
    /// </summary>
    public void Reset() {
        if (IsDead)
        {
            currentHealth = maxHealth;
            EventManager.TriggerEvent("PlayerRespawned");
        }
        transform.position = Checkpoint.ResetPosition + Vector3.up;
        isFrozen = false;
        myRigidbody.velocity = Vector2.zero;
        state = PlayerState.Normal;
		myRenderer.color = myRenderer.color.SetAlpha(0.5f);
		invincibilityTimer.Start(); ;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0) {
            Vector2 dif = ((Vector3)collision.contacts[0].point - transform.position);

            if (Vector2.Dot(dif.normalized, Vector2.up) < 0)
            {
                myRigidbody.gravityScale = 1;


                if (collision.IsOnLayer("Hazard"))
                {
                    state = PlayerState.Dead;

                } else {
                    EventManager.TriggerEvent("PlayerLanded");
                }
            }
        }

		if (collision.collider.CompareTag("Ladder Top"))
		{
            EventManager.TriggerEvent("PlayerLanded");
			aboveLadder = true;
            theLadder = collision.gameObject.GetComponentInParent<Ladder>();
        } else if (collision.collider.CompareTag("Projectile")) {
            if (IsUsingShield) {
                collision.gameObject.GetComponent<Projectile>().Fwd = Controller.Instance.DirectionalInput;
            } else {
                TakeDamage();
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

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Player"/> is using shield.
    /// </summary>
    /// <value><c>true</c> if is using shield; otherwise, <c>false</c>.</value>
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
    /// Gets a value indicating whether this <see cref="T:Player"/> is on ladder.
    /// </summary>
    /// <value><c>true</c> if is on ladder; otherwise, <c>false</c>.</value>
    public bool IsOnLadder {
        get {
            return onLadder && !aboveLadder && !IsOnGround;
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

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is using magnetic gloves.
    /// </summary>
    /// <value><c>true</c> if is using magnetic gloves; otherwise, <c>false</c>.</value>
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
            UIManager.Instance.SetMainText(mi.ItemGetMessage);
            EventManager.TriggerEvent("ShowMessage");
            state = PlayerState.ReadingMessage;
        }
    }

    /// <summary>
    /// Event Handler for the end of a cutscene
    /// </summary>
    void EndCutscene() {

		isFrozen = false;
		state = PlayerState.Normal;
        EventManager.TriggerEvent("HideMessage");
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

    /// <summary>
    /// Gets or sets the held item.
    /// </summary>
    /// <value>The held item.</value>
    public HeldItem HeldItem {
        get {
            return heldItem;
        }

        set {
            heldItem = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is holding item.
    /// </summary>
    /// <value><c>true</c> if is holding item; otherwise, <c>false</c>.</value>
    public bool IsHoldingItem {
        get {
            return heldItem != null;
        }
    }
    /// <summary>
    /// Gets the current health.
    /// </summary>
    /// <value>The current health.</value>
    public int CurrentHealth {
        get {
            return currentHealth;
        }
    }

    /// <summary>
    /// Gets the max health.
    /// </summary>
    /// <value>The max health.</value>
    public int MaxHealth {
        get {
            return maxHealth;
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
    UsingShield,
    Frozen,
    ReadingMessage
}
