using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MagneticField : MapTile{
    BoxCollider2D myCollider;
    [SerializeField]
    bool pull = true;
    float force = 15f;
	void Awake()
	{
		Init();
	}

	public override void Init()
	{
        myCollider = GetComponent<BoxCollider2D>();
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
		RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, Vector2.one, 0, transform.up, myCollider.size.y);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider.GetComponent<Metal>())
			{
				hit.rigidbody.AddForce((pull ? transform.up : -transform.up) * force / 10);
			}
		}
    }

}
