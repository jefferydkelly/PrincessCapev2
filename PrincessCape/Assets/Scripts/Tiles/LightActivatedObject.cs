using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightActivatedObject : ActivatorObject {
    Animator myAnimator;

    public override void Init()
    {
        base.Init();
        myAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light")) {
            Activate();
            myAnimator.SetBool("IsActive", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.CompareTag("Light"))
		{
			Deactivate();
            myAnimator.SetBool("IsActive", false);
		}
    }
}
