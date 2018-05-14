using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightActivatedObject : ActivatorObject {
    Animator myAnimator;
	GameObject lightSource;
    public override void Init()
    {
        base.Init();
        myAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light")) {
            Activate();
			lightSource = collision.gameObject;
            myAnimator.SetBool("IsActive", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.CompareTag("Light"))
		{
			Deactivate();
            myAnimator.SetBool("IsActive", false);
			lightSource = null;
		}
    }

	private void Update()
    {
        if (lightSource && !lightSource.activeInHierarchy)
        {
			Deactivate();
            myAnimator.SetBool("IsActive", false);
			lightSource = null;
        }
    }
}
