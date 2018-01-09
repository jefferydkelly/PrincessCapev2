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
        myRigidbody.velocity = Vector2.right * fwdX * 2;
        groundMask = 1 << LayerMask.NameToLayer("Ground");
    }

    // Update is called once per frame
    void Update () {
        if (!Physics2D.Raycast(transform.position + Vector3.right * fwdX / 2.0f, Vector2.down, 1.0f, groundMask)) {
            fwdX *= -1;
        }

        myRigidbody.velocity = Vector2.right * fwdX * moveSpeed;
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
