using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {
    float speed = 2.0f;
    Rigidbody2D myRigidbody;
    float lifeTime = 5.0f;
	// Use this for initialization
	void Awake () {
        myRigidbody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
	}
	
    public Vector2 Fwd {
        set {
            myRigidbody.velocity = value * speed;
        }

        get {
            return myRigidbody.velocity.normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
