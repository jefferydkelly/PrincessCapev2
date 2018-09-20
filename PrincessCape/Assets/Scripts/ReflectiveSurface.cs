using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectiveSurface : MonoBehaviour {

    [SerializeField]
    LightField reflection;
    private void Awake()
    {
        reflection.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light") && collision.transform.parent != transform) {
            reflection.gameObject.SetActive(true);
            reflection.Activate();
            collision.GetComponent<LightField>().OnFade.AddListener(EndReflection);

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
		if (collision.CompareTag("Light") && collision.transform.parent != transform)
		{
            reflection.gameObject.SetActive(true);
            reflection.Activate();
		}

	
    }

	private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.CompareTag("Light") && collision.transform.parent != transform)
		{
            collision.GetComponent<LightField>().OnFade.RemoveListener(EndReflection);
			EndReflection();
		}
    }

	void EndReflection() {
        reflection.Deactivate();
        reflection.gameObject.SetActive(false);
	}
}
