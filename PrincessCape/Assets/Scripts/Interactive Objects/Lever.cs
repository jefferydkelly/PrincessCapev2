using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractiveObject {
    ActivatedObject activatedObj;
    bool activated = false;
    Animator myAnimator;
	// Use this for initialization
	void Start () {
        activatedObj = GetComponent<ActivatedObject>();
        activated = activatedObj.StartsActive;
        myAnimator = GetComponent<Animator>();
	}

    /// <summary>
    /// Activates or deactivates the lever.
    /// </summary>
    public override void Interact()
    {
        activated = !activated;
        myAnimator.SetBool("IsActivated", activated);
        if (activated) {
            activatedObj.IncrementActivator();
        } else {
            activatedObj.DecrementActivator();
        }
    }
}
