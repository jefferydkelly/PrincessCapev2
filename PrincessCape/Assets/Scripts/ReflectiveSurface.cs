using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectiveSurface : MonoBehaviour {

	GameObject lightSource;
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light") && collision.transform.parent != transform) {
            transform.GetChild(0).gameObject.SetActive(true);
			lightSource = collision.gameObject;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
		if (collision.CompareTag("Light") && collision.transform.parent != transform)
		{
			transform.GetChild(0).gameObject.SetActive(true);
			lightSource = collision.gameObject;
		}

	
    }

	private void Update()
	{
		if (lightSource && !lightSource.activeInHierarchy)
        {
            EndReflection();
        }
	}

	private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.CompareTag("Light") && collision.transform.parent != transform)
		{
			EndReflection();
		}
    }

	void EndReflection() {
		transform.GetChild(0).gameObject.SetActive(false);
	}
}
