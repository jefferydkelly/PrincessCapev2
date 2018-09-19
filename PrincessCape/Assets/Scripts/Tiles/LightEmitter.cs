using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmitter : ActivatedObject
{
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
        IsActivated = true;
        emission.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate the light field.
    /// </summary>
    public override void Deactivate()
    {
        IsActivated = false;
        emission.gameObject.SetActive(false);
    }
}
