using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightField : MonoBehaviour {
    Timer expandTimer;
    float expandTime = 0.25f;
    BoxCollider2D myCollider;
	UnityEvent onFade;

    private void Awake()
    {
        expandTimer = new Timer(expandTime, true);
        expandTimer.OnTick.AddListener(()=> {
            SetYScale((float)transform.localScale.y + 0.5f);
        });
		onFade = new UnityEvent();

    }

    private void OnEnable()
    {
        expandTimer.Start();
    }

    private void OnDisable()
    {
        if (!Game.isClosing)
        {
            expandTimer.Stop();
			transform.localScale = Vector3.one;
        }
		onFade.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!CanPassThrough(collision))
        {
            Stop();
            SetYScale(collision.gameObject);
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
		return (surf != null && surf.transform == transform.parent) || (col.transform == transform.parent)|| col.CompareTag("Light") || col.gameObject.IsOnLayer("UI") || col.gameObject.IsOnLayer("Background");
    }

    void Stop() {
        if (expandTimer.IsRunning && !Game.isClosing)
        {
            expandTimer.Stop();
        }

    }

    void SetYScale(float yScale) {
        transform.localScale = transform.localScale.SetY(yScale);
    }

    void SetYScale(GameObject go) {
        float distance = FindClosestPoint(go);
        if (distance > 0)
        {
            SetYScale(distance);
        }
    }

    float FindClosestPoint(GameObject go) {
        
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, transform.up, transform.localScale.y)) {
            if (hit.collider.gameObject == go) {
                return hit.distance;
            }
        }
        return -1;
    }

    public UnityEvent OnFade {
        get {
            return onFade;
        }
    }
}
