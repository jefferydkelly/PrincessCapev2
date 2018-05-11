using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : ActivatedObject
{
	bool up = true;
	Timer moveTimer;
	[SerializeField]
	float moveTime = 1.0f;
	[SerializeField]
	float moveDistance = 3.0f;
	Vector3 startLocation;

	int ticksPerSec = 60;
	public override void Init()
	{
		base.Init();
		startLocation = transform.position;
		moveTimer = new Timer(1.0f / ticksPerSec, (int)(moveTime * ticksPerSec));

		moveTimer.OnTick.AddListener(() =>
		{
			if (up)
			{
				transform.position += Vector3.up * moveDistance / (moveTime * ticksPerSec);
				if (transform.position.y - startLocation.y >= moveDistance) {
					moveTimer.Stop();
					up = false;
					transform.position = transform.position.SetY(startLocation.y + moveDistance);
				}
			} else {
				transform.position += Vector3.up * moveDistance / ticksPerSec;

				if (transform.position.y <= startLocation.y) {
					transform.position = transform.position.SetY(startLocation.y);
				}
			}
		});
        
	}
	public override void Activate()
	{
		moveTimer.Start();
	}

	public override void Deactivate()
	{
		moveTimer.Stop();
	}

	private void OnDestroy()
	{
		moveTimer.Stop();
	}
}
