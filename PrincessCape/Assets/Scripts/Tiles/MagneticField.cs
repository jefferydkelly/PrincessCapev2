using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MagneticField : MapTile{
    SpriteRenderer myRenderer;
    [SerializeField]
    bool pull = true;
    float force = 15f;
	void Awake()
	{
		Init();
	}

	public override void Init()
	{
        myRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Metal"))
		{
			collision.attachedRigidbody.AddForce((pull ? -transform.up : transform.up) * force);
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
    }

}
