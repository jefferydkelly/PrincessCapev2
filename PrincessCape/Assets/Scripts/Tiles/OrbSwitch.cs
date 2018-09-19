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
}
