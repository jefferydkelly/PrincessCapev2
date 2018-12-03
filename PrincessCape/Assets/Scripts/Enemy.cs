using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    int fwdX = 1;
    Rigidbody2D myRigidbody;
    int groundMask;
    float moveSpeed = 2.0f;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        groundMask = 1 << LayerMask.NameToLayer("Ground");

        if (Application.isPlaying) {

            IsFrozen = Game.Instance.IsInLevelEditor;

            if (IsFrozen) {
                Game.Instance.OnGameStateChanged.AddListener(OnStateChanged);
            }
        }
    }

    void OnStateChanged(GameState gameState) {
        IsFrozen = gameState != GameState.Playing;
    }
    bool IsFrozen {
        set {
            if (value) {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            } else {
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                myRigidbody.velocity = Vector2.right * fwdX * 2;
            }
        }

        get {
            return myRigidbody.constraints == RigidbodyConstraints2D.FreezeAll;
        }
    }
    // Update is called once per frame
    void Update () {
        if (Game.IsBeingPlayed)
        {
            if (!Physics2D.Raycast(transform.position + Vector3.right * fwdX / 2.0f, Vector2.down, 1.0f, groundMask))
            {
                fwdX *= -1;
            }

            myRigidbody.velocity = Vector2.right * fwdX * moveSpeed;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.IsOnLayer("Ground")) {
            if ((collision.transform.position - transform.position).x / fwdX > 0) {
                fwdX *= -1;
            }
        }
    }
}
