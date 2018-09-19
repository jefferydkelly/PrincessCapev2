using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthPlatform : MovingPlatform {

    public override void Init()
    {
        base.Init();

        moveTimer = new Timer(travelTime, true);
        moveTimer.OnTick.AddListener(() =>
        {
            direction *= -1;
        });

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
