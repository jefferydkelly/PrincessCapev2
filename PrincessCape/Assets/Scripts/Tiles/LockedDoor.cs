using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MapTile {

    Animator myAnimator;
    public override void Init()
    {
        base.Init();
        myAnimator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Key") {
            myAnimator.SetTrigger("Open");
            Destroy(collision.gameObject);
        }
    }
}
