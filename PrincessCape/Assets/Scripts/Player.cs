using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int platformLayers = -1;
    Rigidbody2D myRigidbody;
    Timer resetTimer;
    bool isFrozen = false;
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        resetTimer = new Timer(Game.Instance.Reset, 1.0f);
    }
    // Use this for initialization
    void Start () {
        platformLayers = ~(1 << LayerMask.NameToLayer("Player"));
        CameraManager.Instance.Target = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isFrozen)
        {
            myRigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * 5, 0));
            myRigidbody.ClampXVelocity(2.0f);
            if (IsOnGround && Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector2.up * 7.5f, ForceMode2D.Impulse);
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
        transform.position = Checkpoint.ResetPosition;
        isFrozen = false;
        myRigidbody.velocity = Vector2.zero;
    }
}
