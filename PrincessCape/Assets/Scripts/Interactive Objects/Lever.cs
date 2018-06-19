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

    public override void Interact()
    {
        activated = !activated;
        myAnimator.SetBool("activated", activated);
        if (activated) {
            activatedObj.IncrementActivator();
        } else {
            activatedObj.DecrementActivator();
        }
    }
}
