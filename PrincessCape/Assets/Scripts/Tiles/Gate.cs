using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : ActivatedObject {
    /// <summary>
    /// Opens the gate.
    /// </summary>
    public override void Activate()
    {
        IsActivated = true;
    }

    /// <summary>
    /// Closes the gate
    /// </summary>
    public override void Deactivate()
    {
        IsActivated = false;
    }

    // Use this for initialization
    void Awake () {
        

        Init();
	}

    /// <summary>
    /// Initializes the gate
    /// </summary>
    public override void Init() {
        if (startActive) {
            Activate();
        }
    }

    /// <summary>
    /// Increases or decreases the height of the gate
    /// </summary>
    /// <param name="up">If set to <c>true</c> increase the height.  Decreases it otherwise.</param>
    public override void ScaleY(bool up)
    {
        if (up) {
            transform.localScale += Vector3.up / 2;
            //transform.localPosition += transform.up / 2;
        } else if (transform.localScale.y > 0.5f){
			transform.localScale -= Vector3.up / 2;
            //transform.localPosition -= transform.up / 2;
        }
    }

    /// <summary>
    /// Rotates the gate
    /// </summary>
    /// <param name="ang">Ang.</param>
    public override void Rotate(float ang)
    {
        base.Rotate(ang);
        if (transform.localScale.y == (int)transform.localScale.y) {
            transform.position += Vector3.up / 2;
            transform.position += Vector3.right / ((int)transform.localScale.y * 2);
        }
    }

    /// <summary>
    /// Gets the center of the gate.
    /// </summary>
    /// <value>The center.</value>
	public override Vector3 Center
	{
		get
		{
			return transform.position - transform.up * Bounds.y / 2;
		}
	}
}
