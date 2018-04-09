using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightField : MonoBehaviour {
    Timer expandTimer;
    float expandTime = 0.25f;
    BoxCollider2D myCollider;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!CanPassThrough(collision))
        {
            Stop();
            Debug.Log(FindClosestPoint(collision.gameObject));
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
        return (surf != null && surf.transform == transform.parent) || (col.transform == transform.parent)|| col.CompareTag("Light") || col.gameObject.IsOnLayer("UI");
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

}
