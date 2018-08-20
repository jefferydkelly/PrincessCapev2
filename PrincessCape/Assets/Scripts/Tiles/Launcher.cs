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
    Animator myAnimator;
    // Use this for initialization
    void Awake()
    {
		Init();
    }

    public override void Init() {
		         
	    myAnimator = GetComponent<Animator>();
		if (launchTimer == null)
		{
			launchTimer = new Timer(launchTime, true);
			launchTimer.OnTick.AddListener(Launch);
		}
		base.Init();  
        initialized = true;
    }
    void Launch() {
        Projectile proj = Instantiate(projectile.gameObject).GetComponent<Projectile>();
        proj.transform.position = transform.position + transform.right;
        proj.Fwd = transform.right;
    }

    public override void Activate()
    {
        if (!Game.Instance.IsInLevelEditor || Game.Instance.IsPlaying)
        {
            launchTimer.Start();
        }
        myAnimator.SetTrigger("Activate");
    }

    public override void Deactivate()
    {
        launchTimer.Stop();
        myAnimator.SetTrigger("Deactivate");
    }
}
