using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTile : ActivatedObject {
    Animator myAnimator;
    MagneticField magField;

    public override void Activate()
    {
        myAnimator.SetTrigger("Activate");
        magField.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        myAnimator.SetTrigger("Deactivate");
        magField.gameObject.SetActive(false);
    }

    public override void Init() {
		myAnimator = GetComponent<Animator>();
        magField = GetComponentInChildren<MagneticField>();
        magField.gameObject.SetActive(false);
    }

    public override void ScaleY(bool up)
    {
        if (!magField) {
            magField = GetComponentInChildren<MagneticField>();
        }

        magField.ScaleY(up);
    }
}
