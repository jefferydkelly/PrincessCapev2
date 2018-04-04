using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTile : ActivatedObject {
    Animator myAnimator;
    BoxCollider2D magField;
    [SerializeField]
    bool pull = true;
    float force = 15f;
    public override void Activate()
    {
        myAnimator.SetTrigger("Activate");
        magField.enabled = true;
    }

    public override void Deactivate()
    {
        myAnimator.SetTrigger("Deactivate");
        magField.enabled = false;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, Vector2.one, 0, transform.up, magField.size.y);
        foreach(RaycastHit2D hit in hits) {
            if (hit.collider.GetComponent<Metal>()) {
                hit.rigidbody.AddForce((pull ? transform.up : -transform.up) * force / 10);
            }
        }
    }

    // Use this for initialization
    void Awake () {
        Init();
	}

    public override void Init() {
		myAnimator = GetComponent<Animator>();
		magField = GetComponent<BoxCollider2D>();
		magField.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Metal")) {
            collision.attachedRigidbody.AddForce((pull ? -transform.up : transform.up) * force);
        }
    }

    public override void ScaleY(bool up)
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
       
        if (up) {
            col.size += Vector2.up;
            col.offset += Vector2.up / 2;
        } else if (col.size.y > 1) {
            col.size -= Vector2.up;
            col.offset -= Vector2.up / 2;
        }
    }
}
