using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int platformLayers = -1;
    Rigidbody2D myRigidbody;
    Timer resetTimer;
    bool isFrozen = false;
    Cape cape;
    PullGlove glove;
    PushGlove otherGlove;
    bool onLadder = false;
    bool aboveLadder = false;
    PlayerState state = PlayerState.Normal;
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        resetTimer = new Timer(Game.Instance.Reset, 1.0f);
        cape = gameObject.AddComponent<Cape>();
        glove = gameObject.AddComponent<PullGlove>();
        otherGlove = gameObject.AddComponent<PushGlove>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDied", Die);
    }

	private void OnDisable()
	{
        EventManager.StopListening("PlayerDied", Die);
	}
    // Use this for initialization
    void Start () {
        platformLayers = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Hazard"));
        CameraManager.Instance.Target = gameObject;

	}
	
	// Update is called once per frame
	void Update () {
        if (!isFrozen && !IsPulling)
        {
            bool onGround = IsOnGround;
            myRigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * 5, 0));
            myRigidbody.ClampXVelocity(2.0f);
            if (onLadder) {
                myRigidbody.AddForce(new Vector2(0, Input.GetAxis("Vertical") * 5));

                if (onGround && Input.GetAxis("Vertical") > 0) {
                    myRigidbody.gravityScale = 0;
                } else if (aboveLadder && Input.GetAxis("Vertical") < 0) {
                    myRigidbody.gravityScale = 0;
                    transform.position += Vector3.down * 0.25f;
                }           
            }
            if (onGround && Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector2.up * 7.5f, ForceMode2D.Impulse);
            }

            if (!onGround && !onLadder && Input.GetKeyDown(KeyCode.Space)) {
                cape.Activate();
            } else if (Input.GetKeyUp(KeyCode.Space)) {
                cape.Deactivate();
            }
        }

		if (Input.GetKey(KeyCode.Mouse0))
		{
			glove.Activate();
		}
		else if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			glove.Deactivate();
		}

		if (Input.GetKey(KeyCode.Mouse1))
		{
            otherGlove.Activate();
		}
		else if (Input.GetKeyUp(KeyCode.Mouse1))
		{
            otherGlove.Deactivate();
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
}

public enum PlayerState {
    Normal,
    Dead,
    Floating,
    Pushing,
    Pulling,
    Frozen
}
