using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : HeldItem{

    Vector3 startPosition;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
		startPosition = transform.position;
		EventManager.StartListening("PlayerRespawned", Reset);
    }

    private void Reset()
    {
        transform.position = startPosition;
    }

	/// <summary>
	/// Highlight the object when it touches the player
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.CompareTag("Player") && !Game.Instance.Player.IsHoldingItem)
		{
            Vector3 dif = collision.transform.position - transform.position;
            if (Vector3.Dot(Vector3.up, dif) <= 0.5f)
            {
                IsHighlighted = true;
            }
		}
	}

}
