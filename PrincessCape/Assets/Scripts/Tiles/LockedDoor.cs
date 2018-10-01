using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MapTile {

    Animator myAnimator;

    /// <summary>
    /// Initializes the Locked Door.
    /// </summary>
    public override void Init()
    {
        base.Init();
        myAnimator = GetComponent<Animator>();

    }

	private void Start()
	{
		Game.Instance.Player.OnRespawn.AddListener(() =>
        {
            myAnimator.SetTrigger("Close");
        });
	}

    /// <summary>
    /// Handles collisions with a key
    /// </summary>
    /// <param name="collision">Collision.</param>
	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.HasCompnent<HeldItem>() && collision.gameObject.name.Contains("Key")) {
            myAnimator.SetTrigger("Open");
			if (Game.Instance.IsInCutscene)
			{
                SoundManager.Instance.PlaySound("unlock");
				Destroy(collision.gameObject);
			} else {
				collision.gameObject.SetActive(false);
			}
        }
    }

    /// <summary>
    /// Increases or decreases the height of the door.
    /// </summary>
    /// <param name="up">If set to <c>true</c> up.</param>
	public override void ScaleY(bool up)
	{
		if (up)
        {
            transform.localScale += Vector3.up / 2.0f;
        }
        else if (transform.localScale.y > 1)
        {
            transform.localScale -= Vector3.up / 2.0f;
        }
	}

    /// <summary>
    /// Gets the center point of the Locked Door.
    /// </summary>
    /// <value>The center point of the Locked Door.</value>
	public override Vector3 Center
    {
        get
        {
            return transform.position + transform.up * Bounds.y / 2;
        }
    }
}
