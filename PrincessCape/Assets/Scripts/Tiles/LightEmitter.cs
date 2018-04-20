using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmitter : ActivatedObject
{
    Animator myAnimator;
    [SerializeField]
    LightField emission;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        myAnimator = GetComponent<Animator>();

        if (startActive) {
            Activate();
        } else {
            Deactivate();
        }

    }
    public override void Activate()
    {
        myAnimator.SetBool("IsActive", true);
        emission.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        myAnimator.SetBool("IsActive", false);
        emission.gameObject.SetActive(false);
    }
}
