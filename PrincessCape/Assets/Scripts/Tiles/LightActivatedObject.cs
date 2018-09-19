using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightActivatedObject : ActivatorObject {
	GameObject lightSource;

    /// <summary>
    /// Initializes the Light Activated Object
    /// </summary>
    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// Handles the start of collisions with light
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light")) {
            Activate();
            collision.GetComponent<LightField>().OnFade.AddListener(Deactivate);
        }
    }

    /// <summary>
    /// Handles the end of collisions with light
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.CompareTag("Light"))
		{
			Deactivate();
			lightSource = null;
            collision.GetComponent<LightField>().OnFade.RemoveListener(Deactivate);
		}
    }
}
