using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightField : ActivatedObject {
    Timer expandTimer;
    float expandTime = 0.25f;
    float maxHeight = 1.0f;
    BoxCollider2D myCollider;
	UnityEvent onFade;

    /// <summary>
    /// Initializes this instance of Light Field
    /// </summary>
    public override void Init()
    {
        base.Init();

        expandTimer = new Timer(expandTime, true);
        expandTimer.OnTick.AddListener(() => {
            ScaleY(0.5f);
        });
        onFade = new UnityEvent();
    }
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public override void Activate()
    {
        if (!startActive && Game.Instance.IsPlaying)
        {
            expandTimer.Start();
        }
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
        if (!startActive && expandTimer.IsRunning && !Game.isClosing)
        {
            expandTimer.Stop();
            onFade.Invoke();
        }
    }

    /// <summary>
    /// Handles the changing of the game state
    /// </summary>
    /// <param name="state">State.</param>
    protected override void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Playing) {
            Activate();
        } else {
            Deactivate();
        }
    }

    private void OnDisable()
    {
        if (!Game.isClosing)
        {
            Deactivate();
			transform.localScale = Vector3.one;
        }
		onFade.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!CanPassThrough(collision))
        {
            //Deactivate();
            expandTimer.Stop();
            SetYScale(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanPassThrough(collision)) {
            Activate();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!CanPassThrough(collision)) {
            SetYScale(collision.gameObject);
        }
    }

    /// <summary>
    /// Determines if the light field can pass through the collider
    /// </summary>
    /// <returns><c>true</c>, If the light field can pass through the collider, <c>false</c> otherwise.</returns>
    /// <param name="col">Col.</param>
    bool CanPassThrough(Collider2D col) {
        ReflectiveSurface surf = col.gameObject.GetComponentInChildren<ReflectiveSurface>();
		return (surf != null && surf.transform == transform.parent) || (col.transform == transform.parent)|| col.CompareTag("Light") || col.gameObject.IsOnLayer("UI") || col.gameObject.IsOnLayer("Background");
    }

    /// <summary>
    /// Sets the vertical scale of the light field so that it touches the game object
    /// </summary>
    /// <param name="go">Go.</param>
    void SetYScale(GameObject go) {
        if (!startActive)
        {
            float distance = FindClosestPoint(go);
            if (distance > 0)
            {
                if (transform.parent.name == "Map") {
                    distance *= 2;
                }
                ScaleY(distance - transform.localScale.y);
            }
        }
    }

    /// <summary>
    /// Finds the closest point of the game object to this light field.
    /// </summary>
    /// <returns>The closest point.</returns>
    /// <param name="go">Go.</param>
    float FindClosestPoint(GameObject go) {
        
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, transform.up, transform.localScale.y)) {
            if (hit.collider.gameObject == go) {
                return hit.distance;
            }
        }
        return -1;
    }

    /// <summary>
    /// An event for responding to when the light goes out
    /// </summary>
    /// <value>The on fade.</value>
    public UnityEvent OnFade {
        get {
            return onFade;
        }
    }

    /// <summary>
    /// Scales the light field vertically
    /// </summary>
    /// <param name="yscale">The amount it will be sacled by.</param>
    public override void ScaleY(float yscale)
    {
        Vector3 addition = Vector3.up * yscale;
        transform.localScale += addition;
    }
}
