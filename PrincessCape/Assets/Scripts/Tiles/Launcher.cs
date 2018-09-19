using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : ActivatedObject
{
    [SerializeField]
    Projectile projectile;
    Timer launchTimer;
    float launchTime = 2.0f;
    // Use this for initialization
    void Awake()
    {
		Init();
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public override void Init() {
		         
		if (launchTimer == null)
		{
			launchTimer = new Timer(launchTime, true);
			launchTimer.OnTick.AddListener(Launch);
		}
		base.Init();  
        initialized = true;
    }

    /// <summary>
    /// Launches a projectile
    /// </summary>
    void Launch() {
        Projectile proj = Instantiate(projectile.gameObject).GetComponent<Projectile>();
        proj.transform.position = transform.position + transform.right;
        proj.Fwd = transform.right;
    }

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public override void Activate()
    {
        if (!Game.Instance.IsInLevelEditor || Game.Instance.IsPlaying)
        {
            launchTimer.Start();
        }
        IsActivated = true;
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
        launchTimer.Stop();
        IsActivated = false;
    }
}
