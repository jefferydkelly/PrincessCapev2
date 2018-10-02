using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Player : MonoBehaviour
{

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
    ItemLevel itemState = ItemLevel.None;
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

    [SerializeField]
    LightField shieldField;

    UnityEvent onRespawn;
    UnityEvent onLand;
    UnityEvent onDie;

    private void Awake()
    {
        inventory = new List<MagicItem>();
        Game.Instance.Map.OnLevelLoaded.AddListener(Reset);
        Game.Instance.OnReady.AddListener(Init);
        //EventManager.StartListening("LevelLoaded", Reset);


        UIManager.Instance.OnAimStatusChange.AddListener((bool show) =>
        {
            if (show)
            {
                shieldField.transform.localScale = Vector3.one;
            }
            else
            {
                shieldField.gameObject.SetActive(false);
            }
        });
        Cutscene.Instance.OnEnd.AddListener(EndCutscene);

        shieldField.Init();
        shieldField.gameObject.SetActive(false);
        //DontDestroyOnLoad(gameObject);

        //Initialize events
        onRespawn = new UnityEvent();
        onLand = new UnityEvent();
        onDie = new UnityEvent();

        Game.Instance.OnEditorPlay.AddListener(HealToFull);


    }

    /// <summary>
    /// Initializes the player and the basic components
    /// </summary>
    public void Init()
    {
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
            //EventManager.TriggerEvent("IncreaseHealth");
            invincibilityTimer = new Timer(0.5f);
            invincibilityTimer.OnComplete.AddListener(() =>
            {
                isInvincible = false;
                myRenderer.color = myRenderer.color.SetAlpha(1.0f);
            });
            initialized = true;

            MaxHealth = 3;

            UIManager.Instance.OnMessageStart.AddListener(() =>
            {
                IsFrozen = true;
                state = PlayerState.ReadingMessage;
            });

            UIManager.Instance.OnMessageEnd.AddListener(() =>
            {

                if (!Game.Instance.IsInCutscene)
                {
                    IsFrozen = false;
                    state = PlayerState.Normal;
                }

            });

            if (Game.Instance.IsInLevelEditor)
            {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    private void OnEnable()
    {
        Controller.Instance.OnPause.AddListener(Pause);
        //transform.position = Checkpoint.Active.transform.position.SetZ(0);
        //EventManager.StartListening("Pause", Pause);
    }

    private void OnDisable()
    {
        if (!Game.isClosing)
        {
            Controller.Instance.OnPause.RemoveListener(Pause);
            Game.Instance.OnEditorPlay.RemoveListener(HealToFull);
        }
        //EventManager.StopListening("Pause", Pause);
    }

    /// <summary>
    /// Pause this instance.
    /// </summary>
    void Pause()
    {
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
    void Update()
    {
        if (initialized)
        {

            if (Game.Instance.IsPlaying && !isFrozen && !IsPulling)
            {
                if (!IsDashing)
                {
                    float xForce = Controller.Instance.Horizontal * 5;
                    myRigidbody.AddForce(new Vector2(xForce, 0));
                    myRigidbody.ClampXVelocity(maxSpeed);
                }

                if (myRigidbody.velocity.x / fwd < 0)
                {

                    if (!(IsHoldingItem && heldItem.IsHeavy))
                    {
                        Forward *= -1;
                    }
                }
                if (IsHoldingItem)
                {
                    if (heldItem.IsHeavy)
                    {
                        myRigidbody.ClampXVelocity(maxSpeed / 5.0f);
                        //heldItem.transform.position = heldItem.transform.position.SetY(transform.position.y - Height / 2 + heldItem.HalfHeight);
                    }
                    Vector3 dif = heldItem.transform.position - transform.position;

                    heldItem.Velocity = myRigidbody.velocity;

                    if (myRigidbody.velocity.x / dif.x < 0)
                    {
                        heldItem.Velocity *= 1.05f;
                    }
                }
                bool onGround = IsOnGround;

                if (onLadder && !IsDashing)
                {
                    //myRigidbody.AddForce(new Vector2(0, Controller.Instance.Vertical * 5));

                    bool jump = Controller.Instance.Jump;

                    if (onGround && !(jump && aboveLadder) && Controller.Instance.Vertical > 0)
                    {
                        myRigidbody.gravityScale = 0;
                    }
                    else if (jump)
                    {
                        onLadder = false;
                        Jump();
                        return;
                    }
                    else if (aboveLadder && Controller.Instance.Vertical < 0)
                    {
                        aboveLadder = false;
                        myRigidbody.gravityScale = 0;
                        myRigidbody.velocity = myRigidbody.velocity.SetX(0);
                        transform.position = transform.position.SetX(theLadder.transform.position.x);
                        transform.position += Vector3.down * 2;
                    }

                    if (!aboveLadder)
                    {

                        Vector2 input = Controller.Instance.DirectionalInput;
                        if (!onGround)
                        {
                            input = input.SetY(input.y * maxSpeed / 1.5f);
                            myRigidbody.velocity = input;

                        }
                        else
                        {
                            myRigidbody.velocity = myRigidbody.velocity.SetY(Controller.Instance.Vertical * maxSpeed / 1.5f);
                        }
                    }
                }
                else if (CanJump && Controller.Instance.Jump)
                {
                    Jump();
                }
            }
        }
    }

    /// <summary>
    /// Makes the Player jump.
    /// </summary>
    void Jump()
    {
        state = PlayerState.Jumping;
        myRigidbody.gravityScale = 1.0f;
        myRigidbody.velocity = myRigidbody.velocity.SetY(7.0f);
        SoundManager.Instance.PlaySound("jump");
        //myRigidbody.AddForce(Vector2.up * 7.0f, ForceMode2D.Impulse);
    }
    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is on ground.
    /// </summary>
    /// <value><c>true</c> If the player is on ground; otherwise, <c>false</c>.</value>
    public bool IsOnGround
    {
        get
        {
            return Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.1f), 0, Vector2.down, 1.0f, platformLayers);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> can jump.
    /// </summary>
    /// <value><c>true</c> if can jump; otherwise, <c>false</c>.</value>
    bool CanJump
    {
        get
        {
            return IsOnGround && !IsUsingMagneticGloves && !(IsHoldingItem && heldItem.IsHeavy);
        }
    }

    /// <summary>
    /// Kill the player and reset the game.
    /// </summary>
    public void Die()
    {

        OnDie.Invoke();
        if (!Game.Instance.IsInLevelEditor)
        {
            state = PlayerState.Dead;
            BeginReset();
        }
        else
        {
            Game.Instance.StopInEditor();
        }
    }

    /// <summary>
    /// Deals damage to the player and causes the player to die if the currentHealth == 0
    /// </summary>
    /// <param name="autoReset">If set to <c>true</c>, the player will reset to the last checkpoint automatically.</param>
    public void TakeDamage(bool autoReset = false)
    {
        if (!isInvincible)
        {
            CurrentHealth--;

            isInvincible = true;
            if (currentHealth == 0)
            {
                Die();
            }
            else if (autoReset)
            {
                BeginReset();
            }
            else
            {
                myRenderer.color = myRenderer.color.SetAlpha(0.5f);
                invincibilityTimer.Start(); ;
            }
        }
    }

    /// <summary>
    /// Starts resetting the player.
    /// </summary>
    void BeginReset()
    {
        isFrozen = true;
        EventManager.TriggerEvent("ItemOneDeactivated");
        EventManager.TriggerEvent("ItemTwoDeactivated");
        if (IsHoldingItem)
        {
            heldItem.Drop();
        }
        resetTimer.Start();
    }

    void HealToFull()
    {
        CurrentHealth = maxHealth;
    }

    /// <summary>
    /// Reset the player to the last checkpoint.
    /// </summary>
    public void Reset()
    {
        if (IsDead)
        {
            HealToFull();
        }

        OnRespawn.Invoke();
        transform.position = Checkpoint.ResetPosition + Vector3.up;
        isFrozen = false;
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        state = PlayerState.Normal;
        myRenderer.color = myRenderer.color.SetAlpha(0.5f);
        invincibilityTimer.Start(); ;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Direction colDir = collision.GetClosestDirection();
        if (/*collision.relativeVelocity.y > 0 && */colDir == Direction.Up)
        {
            
            myRigidbody.gravityScale = 1;


            if (collision.IsOnLayer("Hazard"))
            {
                state = PlayerState.Dead;

            }
            else
            {
                IsXFrozen = false;
                OnLand.Invoke();
            }

        }
        else if (IsDashing && (colDir == Direction.Left || colDir == Direction.Right)) {
            IsDashing = false;
            DashBoots boots = (UIManager.Instance.ItemOne.Item is DashBoots ? UIManager.Instance.ItemOne.Item : UIManager.Instance.ItemTwo.Item) as DashBoots;
            boots.Deactivate();
        }

		if (collision.collider.CompareTag("Ladder Top"))
		{
			OnLand.Invoke();
			aboveLadder = true;
            theLadder = collision.gameObject.GetComponentInParent<Ladder>();
        } else if (collision.collider.CompareTag("Projectile")) {
            if (IsUsingShield) {
                
                collision.gameObject.GetComponent<Projectile>().Fwd = (Controller.Instance.MousePosition - collision.transform.position).normalized;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Light") && collision.gameObject != shieldField.gameObject)
		{
			shieldField.gameObject.SetActive(IsUsingShield);
            if (IsUsingShield) {
                shieldField.Activate();
            }

            Vector2 dif = Controller.Instance.MousePosition - shieldField.transform.position;
            shieldField.transform.localRotation = Quaternion.AngleAxis(dif.Angle().ToDegrees() - 90, Vector3.forward);
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
		else if (collision.CompareTag("Light"))
		{
            if (collision.gameObject != shieldField.gameObject)
            {
                shieldField.Deactivate();
                shieldField.gameObject.SetActive(false);

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

        set {
            fwd = (int)value.x;
            myRenderer.flipX = fwd < 0;
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
    /// Gets or sets a value indicating whether this <see cref="T:Player"/> is frozen.
    /// </summary>
    /// <value><c>true</c> if is frozen; otherwise, <c>false</c>.</value>
	public bool IsFrozen {
        get {
            return isFrozen;
        }

        set {
            isFrozen = value;
            if (isFrozen) {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            } else {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    public bool IsXFrozen {
        get {
            return myRigidbody.constraints == RigidbodyConstraints2D.FreezePositionX;
        } 

        set {
            if (value) {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            } else {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Player"/> is floating.
    /// </summary>
    /// <value><c>true</c> if is floating; otherwise, <c>false</c>.</value>
    public bool IsFloating {
        get {
            return itemState == ItemLevel.MagicCape && !IsOnGround;
        }

        set {
            if (value && (!IsDead || IsFrozen)) {
                //state = PlayerState.Floating;
                itemState = ItemLevel.MagicCape;
            } else if (!value && IsFloating) {
                //state = PlayerState.Normal;
                itemState = ItemLevel.None;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Player"/> is using shield.
    /// </summary>
    /// <value><c>true</c> if is using shield; otherwise, <c>false</c>.</value>
    public bool IsUsingShield {
        get {
            //return state == PlayerState.UsingShield;
            return itemState == ItemLevel.StarShield;
        }

        set {
            if (value && !(IsDead || IsFrozen)) {
                //state = PlayerState.UsingShield;
                itemState = ItemLevel.StarShield;
                IsFrozen = true;
            } else if (!value && IsUsingShield) {
                //state = PlayerState.Normal;
                itemState = ItemLevel.None;
                IsFrozen = false;
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
            //return state == PlayerState.Pulling;
            return itemState == ItemLevel.PullGauntlet;
		}

		set
		{
			if (value && (!IsDead || IsFrozen))
			{
                //state = PlayerState.Pulling;
                itemState = ItemLevel.PullGauntlet;
				onLadder = false;
				aboveLadder = false;
			}
            else if (!value && IsPulling)
			{
                //state = PlayerState.Normal;
                itemState = ItemLevel.None;
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
            //return state == PlayerState.Pushing;
            return itemState == ItemLevel.PushGauntlet;
		}

		set
		{
			if (value && (!IsDead || IsFrozen))
			{
                //state = PlayerState.Pushing;
                itemState = ItemLevel.PushGauntlet;
                onLadder = false;
                aboveLadder = false;
			}
			else if (!value && IsPushing)
			{
                itemState = ItemLevel.None;
			}
		}
	}

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is using magnetic gloves.
    /// </summary>
    /// <value><c>true</c> if is using magnetic gloves; otherwise, <c>false</c>.</value>
    public bool IsUsingMagneticGloves {
        get {
            return IsPulling || IsPushing;
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

    public bool IsDashing {
        get {
            return itemState == ItemLevel.DashBoots;
            //return state == PlayerState.Dashing;
        }

        set {
            itemState = value ? ItemLevel.DashBoots : ItemLevel.None;

            //state = value ? PlayerState.Dashing : PlayerState.Normal;
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
            UIManager.Instance.ItemOne.Item = mi;
        } else if (Inventory.Count == 2) {
            if (inventory[0].Slot == MagicItemSlot.First) {
                UIManager.Instance.ItemTwo.Item = mi;
            } else {
                UIManager.Instance.ItemOne.Item = mi;
            }
        }
        items = (ItemLevel)System.Enum.Parse(typeof(ItemLevel), mi.ItemName.Replace(" ", string.Empty));
        if (showMessage)
        {
			UIManager.Instance.ShowMessage(mi.ItemGetMessage);

            state = PlayerState.ReadingMessage;
        }
    }

    public void ClearItems() {
        items = ItemLevel.None;
        inventory.Clear();
    }

    /// <summary>
    /// Event Handler for the end of a cutscene
    /// </summary>
    void EndCutscene() {

		IsFrozen = false;
		state = PlayerState.Normal;
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
            if (heldItem)
            {
                Vector3 dif = heldItem.transform.position - transform.position;

                if (dif.x / fwd < 0)
                {
                    Forward *= -1;
                }
            }
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

		private set {
			currentHealth = value;
			UIManager.Instance.HealthBar.CurrentHealth = value;
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

		set {
			maxHealth = value;
			UIManager.Instance.HealthBar.MaxHealth = value;
		}
    }

    /// <summary>
    /// Gets the height of the player.
    /// </summary>
    /// <value>The height.</value>
    float Height {
        get {
            return myRenderer.bounds.size.y;
        }
    }

    /// <summary>
    /// Gets the event for when the player respawns.
    /// </summary>
    /// <value>The onRespawnEvent.</value>
	public UnityEvent OnRespawn {
		get {
			return onRespawn;
		}
	}

    /// <summary>
    /// Gets the event for when the player dies.
    /// </summary>
    /// <value>The onDie event.</value>
	public UnityEvent OnDie {
		get {
			return onDie;
		}
	}

    /// <summary>
    /// Gets the event for when the player lands on the ground
    /// </summary>
    /// <value>The onLand event.</value>
	public UnityEvent OnLand {
		get {
			return onLand;
		}
	}

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Player"/> is on screen.
    /// </summary>
    /// <value><c>true</c> if is on screen; otherwise, <c>false</c>.</value>
	public bool IsOnScreen {
		get {
			return myRenderer.isVisible;
		}
	}
}

public enum PlayerState {
    Normal,
    Dead,
    Jumping,
    Frozen,
    ReadingMessage
}
