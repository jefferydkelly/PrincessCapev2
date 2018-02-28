using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : ActivatedObject
{
    [SerializeField]
    Projectile projectile;
    Timer launchTimer;
    float launchTime = 1.0f;
    Animator myAnimator;
    // Use this for initialization
    void Awake()
    {
        myAnimator = GetComponent<Animator>();
        launchTimer = new Timer(launchTime, true);
        launchTimer.OnTick.AddListener(Launch);
    }

    void Launch() {
        Projectile proj = Instantiate(projectile.gameObject).GetComponent<Projectile>();
        proj.transform.position = transform.position + transform.right;
        proj.Fwd = transform.right;
    }

    public override void Activate()
    {
        
        launchTimer.Start();
        myAnimator.SetTrigger("Activate");
    }

    public override void Deactivate()
    {
        launchTimer.Stop();
        myAnimator.SetTrigger("Deactivate");
    }
}
