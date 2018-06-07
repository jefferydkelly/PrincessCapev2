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

	public override void ScaleY(bool up)
	{
		if (up)
        {
            transform.localScale += Vector3.up / 2.0f;
        }
        else if (transform.localScale.y > 1)
        {
            transform.localScale -= Vector3.up / 2.0f;
        }
	}

	public override Vector3 Center
    {
        get
        {
            return transform.position + transform.up * Bounds.y / 2;
        }
    }
}
