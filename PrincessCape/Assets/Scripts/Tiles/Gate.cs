using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : ActivatedObject {
    Animator myAnimator;
    /// <summary>
    /// Opens the gate.
    /// </summary>
    public override void Activate()
    {
        IsActivated = true;
        myAnimator.SetBool("isOpen", true);
    }

    /// <summary>
    /// Closes the gate
    /// </summary>
    public override void Deactivate()
    {
        IsActivated = false;
        myAnimator.SetBool("isOpen", false);
    }

    // Use this for initialization
    void Awake () {
        myAnimator = GetComponent<Animator>();

        if (startActive) {
            EventManager.StartListening("LevelLoaded", Activate);
        }
	}

    public override void ScaleY(bool up)
    {
        if (up) {
            transform.localScale += Vector3.up / 2;
            transform.localPosition += transform.up / 2;
        } else if (transform.localScale.y > 0.5f){
			transform.localScale -= Vector3.up / 2;
            transform.localPosition -= transform.up / 2;
        }
    }

    public override void Rotate(float ang)
    {
        base.Rotate(ang);
        if (transform.localScale.y == (int)transform.localScale.y) {
            transform.position += Vector3.up / 2;
            transform.position += Vector3.right / ((int)transform.localScale.y * 2);
        }
    }
}
