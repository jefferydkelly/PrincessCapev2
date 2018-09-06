using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MagneticField : MapTile{
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

	public override void Init()
	{
        myRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("Activated", Game.Instance.IsPlaying);

        Game.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);

        maxScale = (int)transform.localScale.y;

        expandTimer = new Timer(expandTime, (maxScale - 1) * 4);
        expandTimer.OnTick.AddListener(()=> {
            transform.localScale += Vector3.up / 4;
        });


	}

    void OnGameStateChanged(GameState state) {
        myAnimator.SetBool("Activated", Game.Instance.IsPlaying);
    }

    private void OnDestroy()
    {
        if (!Game.isClosing) {
            Game.Instance.OnGameStateChanged.RemoveListener(OnGameStateChanged);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Metal"))
		{
			collision.attachedRigidbody.AddForce((pull ? -transform.up : transform.up) * force);
		}
	}

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

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        maxScale = Mathf.RoundToInt(transform.localScale.y);
    }

    public override void ScaleY(bool up)
    {
        if (up) {
            transform.localScale += Vector3.up;
        } else if (transform.localScale.y > 1){
            transform.localScale -= Vector3.up;
        }
    }

	public override void ScaleY(float yscale)
	{
		transform.localScale = transform.localScale.SetY(yscale);
	}
}
