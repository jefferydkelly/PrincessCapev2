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
    void Start () {
        myAnimator = GetComponent<Animator>();
	}
}
