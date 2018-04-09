using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : HeldItem {
    Vector3 startPosition;
    /*
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();

		startPosition = transform.position;
		EventManager.StartListening("Reset", () => {
			transform.position = startPosition;
            gameObject.SetActive(true);
		});
    }*/

    /*
    public override void Init()
    {
        base.Init();
        startPosition = transform.position;
        EventManager.StartListening("PlayerRespawned", ()=> {
            gameObject.SetActive(true);
            transform.position = startPosition;

        });
    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.collider.CompareTag("Player") && !Game.Instance.Player.IsHoldingItem)
		{
			IsHighlighted = true;
        } else if (myRigidbody.velocity.sqrMagnitude > 25) {
            gameObject.SetActive(false);
        }
    }
}
