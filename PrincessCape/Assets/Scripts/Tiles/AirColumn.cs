using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirColumn : MapTile {

    Animator myAnimator;
    public override void Init()
    {
        base.Init();
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("Activated", Game.Instance.IsPlaying);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(transform.up * 15);
    }
}
