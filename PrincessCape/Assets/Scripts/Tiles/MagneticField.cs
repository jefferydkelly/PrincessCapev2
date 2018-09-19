using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MagneticField : MapTile {
    SpriteRenderer myRenderer;
    Animator myAnimator;
    [SerializeField]
    bool pull = true;
    float force = 15f;
    int maxScale = 1;
    Timer expandTimer;
    float expandTime = 0.0625f;

	void Awake()
	{
		Init();
	}

    /// <summary>
    /// Initializes the instance of the Magnetic Field
    /// </summary>
	public override void Init()
	{
        myRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("IsActivated", Game.Instance.IsPlaying);

        maxScale = (int)transform.localScale.y;

        expandTimer = new Timer(expandTime, (maxScale - 1) * 4);
        expandTimer.OnTick.AddListener(()=> {
            transform.localScale += Vector3.up / 4;
        });


	}

    /// <summary>
    /// Handles the changing of the game state.  Animating only when the game is playing
    /// </summary>
    /// <param name="state">State.</param>
    protected override void OnGameStateChanged(GameState state) {
        myAnimator.SetBool("IsActivated", Game.Instance.IsPlaying);
    }

    /// <summary>
    /// Handles the collisions with metal objects
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Metal"))
		{
			collision.attachedRigidbody.AddForce((pull ? -transform.up : transform.up) * force);
		}
	}

    /// <summary>
    /// Shrinks the field and starts expanding it when the field is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (!Game.Instance.IsInLevelEditor)
        {
            if (transform.localScale.y > maxScale)
            {
                maxScale = Mathf.RoundToInt(transform.localScale.y);
            }
            transform.localScale = Vector3.one;
            expandTimer.Start();
        }
    }
    /// <summary>
    /// Stops any expansion and removes all of the force from Metal objects in the field when it is disabled
    /// </summary>
    private void OnDisable()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position - (transform.up * myRenderer.bounds.extents.y), Vector2.one, 0, transform.up, myRenderer.bounds.size.y);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider.GetComponent<Metal>())
			{
                hit.rigidbody.velocity = Vector2.zero;
                hit.rigidbody.AddForce(Vector2.down * force / 5);
			}
		}
        expandTimer.Stop();
    }

    /// <summary>
    /// Creates a magnetic field from given tile data
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        maxScale = Mathf.RoundToInt(transform.localScale.y);
    }

    /// <summary>
    /// Scales the height of the field.
    /// </summary>
    /// <param name="up">If set to <c>true</c> up.</param>
    public override void ScaleY(bool up)
    {
        if (up) {
            transform.localScale += Vector3.up;
        } else if (transform.localScale.y > 1){
            transform.localScale -= Vector3.up;
        }
    }

    /// <summary>
    /// Sets the vertical scale of the field
    /// </summary>
    /// <param name="yscale">Yscale.</param>
	public override void ScaleY(float yscale)
	{
		transform.localScale = transform.localScale.SetY(yscale);
	}
}
