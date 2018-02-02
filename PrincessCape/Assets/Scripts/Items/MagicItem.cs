using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicItem:MonoBehaviour {

    protected Timer cooldownTimer;
    float cooldownTime = 0.25f;
	// Use this for initialization
    public MagicItem () {
        cooldownTimer = new Timer(Reset, cooldownTime);
	}

    public abstract void Activate();
    public abstract void Deactivate();

    public virtual void Reset() {
        
    }
}
