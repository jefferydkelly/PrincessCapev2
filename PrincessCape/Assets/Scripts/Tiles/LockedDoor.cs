using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MapTile {

    Animator myAnimator;
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
	private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.GetComponent<HeldItem>() && collision.gameObject.name.Contains("Key")) {
            myAnimator.SetTrigger("Open");
			if (Game.Instance.IsInCutscene)
			{
				Destroy(collision.gameObject);
			} else {
				collision.gameObject.SetActive(false);
			}
        }
    }

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

	public override Vector3 Center
    {
        get
        {
            return transform.position + transform.up * Bounds.y / 2;
        }
    }
}
