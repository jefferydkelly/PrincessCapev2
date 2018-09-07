using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthPlatform : MovingPlatform {
    Animator myAnimator;

    public override void Init()
    {
        base.Init();
        myAnimator = GetComponent<Animator>();

        moveTimer = new Timer(travelTime, true);
        moveTimer.OnTick.AddListener(() =>
        {
            direction *= -1;
        });

    }

    public override void Activate()
    {
        base.Activate();
        myAnimator.SetBool("IsActive", true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        myAnimator.SetBool("IsActive", false);
    }
    /// <summary>
    /// Update this instance.
    /// </summary>
    private void Update()
    {
        if (!Game.Instance.IsPaused && moveTimer.IsRunning)
        {
            transform.position += direction * travelDistance / travelTime * Time.deltaTime;
        }
    }

    /// <summary>
    /// Event for when a collision between this and another object continues
    /// </summary>
    /// <param name="collision">Collision.</param>
    void OnCollisionStay2D(Collision2D collision)
    {
        if (Game.Instance.IsPlaying && direction.y <= 0)
        {
            collision.transform.position += direction * travelDistance / travelTime * Time.deltaTime;
        }

    }
}
