using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class OrbSwitch : ActivatorObject
{

    Animator myAnimatior;

    public override void Init() {
		base.Init();
		myAnimatior = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            IsActivated = !IsActivated;
          
            if (IsActivated)
            {
                Activate();

            }
            else
            {
                Deactivate();

            }
        }
    }

    public override void Activate()
    {
        base.Activate();
        myAnimatior.SetTrigger("Activate");
    }

    public override void Deactivate()
    {
        base.Deactivate();
        myAnimatior.SetTrigger("Deactivate");
    }
}
