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
}
