using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        

        Init();
	}

    public override void Init() {
        myAnimator = GetComponent<Animator>();
        if (startActive) {
            Activate();
        }
    }

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

    public override void Rotate(float ang)
    {
        base.Rotate(ang);
        if (transform.localScale.y == (int)transform.localScale.y) {
            transform.position += Vector3.up / 2;
            transform.position += Vector3.right / ((int)transform.localScale.y * 2);
        }
    }

	#if UNITY_EDITOR
    /// <summary>
    /// Draws indicators of the connections between the Activated Object and its connections as well as the activation status of the object
    /// </summary>
    public override void RenderInEditor()
    {
        foreach (ActivatedObject acto in Connections)
        {
            if (acto)
            {
                Handles.DrawDottedLine(transform.position, acto.transform.position, 8.0f);
            }
        }

        if (startActive)
        {
            Handles.color = Color.green;

        }
        else
        {
            Handles.color = Color.red;
        }
		Handles.DrawSolidArc(transform.position + (Vector3)Vector2.down.RotateDeg(transform.rotation.z) * transform.localScale.y, -Vector3.forward, Vector3.up, 360, 0.5f);
        Handles.color = Color.white;
    }

#endif
}
