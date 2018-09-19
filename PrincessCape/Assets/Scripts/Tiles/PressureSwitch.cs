using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PressureSwitch : ActivatorObject {

    /// <summary>
    /// When a Rigidbody with a great enough mass collides with the switch, increment ItemsOnTop.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.mass >= 0.1f) {
			IncrementActivator();
        }
    }

	/// <summary>
	/// When a Rigidbody with a great enough mass stops colliding with the switch, decrement ItemsOnTop.
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.attachedRigidbody.mass >= 0.1f)
		{
			DecrementActivator();
		}
	}
}
