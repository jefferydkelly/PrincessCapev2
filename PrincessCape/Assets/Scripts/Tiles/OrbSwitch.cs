using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class OrbSwitch : ActivatorObject
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
			Destroy(collision.gameObject);
			IsActivated = !IsActivated;
            if (isActivated)
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
        SoundManager.Instance.PlaySound("switchactivate");
    }

    public override void Deactivate()
    {
        base.Deactivate();
        SoundManager.Instance.PlaySound("switchactivate");
    }
}
