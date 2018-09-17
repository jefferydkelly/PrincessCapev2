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

    /// <summary>
    /// Initializes the Light Emitter.
    /// </summary>
    public override void Init()
    {
        myAnimator = GetComponent<Animator>();

        if (startActive) {
            Activate();
        } else {
            Deactivate();
        }

    }

    /// <summary>
    /// Activate this light field.
    /// </summary>
    public override void Activate()
    {
        myAnimator.SetBool("IsActivated", true);
        emission.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate the light field.
    /// </summary>
    public override void Deactivate()
    {
        myAnimator.SetBool("IsActivated", false);
        emission.gameObject.SetActive(false);
    }
}
