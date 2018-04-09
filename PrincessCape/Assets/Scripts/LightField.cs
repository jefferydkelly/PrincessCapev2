using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightField : MonoBehaviour {
    Timer expandTimer;
    float expandTime = 0.25f;

    private void Awake()
    {
        expandTimer = new Timer(expandTime, true);
        expandTimer.OnTick.AddListener(()=> {
            SetYScale((float)transform.localScale.y + 0.5f);
        });
    }

    private void OnEnable()
    {
        expandTimer.Start();
    }

    private void OnDisable()
    {
        expandTimer.Stop();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!CanPassThrough(collision.collider))
        {
            Stop(collision.gameObject);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanPassThrough(collision))
        {
            Stop(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanPassThrough(collision)) {
            expandTimer.Start();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!CanPassThrough(collision)) {
            SetYScale(collision.gameObject);
        }
    }

    bool CanPassThrough(Collider2D col) {
        ReflectiveSurface surf = col.gameObject.GetComponentInChildren<ReflectiveSurface>();
        return (surf != null && surf.transform == transform.parent) || col.CompareTag("Light");
    }

    void Stop(GameObject go) {
        if (expandTimer.IsRunning && !Game.isClosing)
        {
            expandTimer.Stop();
            SetYScale(go);
        }

    }

    void SetYScale(float yScale) {
        transform.localScale = transform.localScale.SetY(yScale);
    }

    void SetYScale(GameObject go) {
		Vector3 dif = go.transform.position - transform.parent.position;
		float dot = Vector3.Dot(dif, transform.up);
		SetYScale(dot - 0.625f);
    }
}
