using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicItem:MonoBehaviour {

    protected Timer cooldownTimer;
    float cooldownTime = 1.0f;
	// Use this for initialization
    public MagicItem () {
        cooldownTimer = new Timer(Reset, cooldownTime);
	}

    public abstract void Use();

    public virtual void Reset() {
        
    }
}
