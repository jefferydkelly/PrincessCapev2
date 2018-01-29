using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int platformLayers = -1;
    Rigidbody2D myRigidbody;
    Timer resetTimer;
    bool isFrozen = false;
    Cape cape;
    bool onLadder = false;
    bool aboveLadder = false;
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        resetTimer = new Timer(Game.Instance.Reset, 1.0f);
        cape = gameObject.AddComponent<Cape>();
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
        if (!isFrozen)
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
                cape.Use();
            }
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
}
