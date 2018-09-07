using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Elevator : ActivatedObject
{
	bool up = true;
	Timer moveTimer;
	[SerializeField]
	float moveTime = 1.0f;
	[SerializeField]
	float moveDistance = 3.0f;
    [SerializeField]
    GameObject rangeIndicator;
    [SerializeField]
    GameObject rangeLine;
	Vector3 startLocation;

	int ticksPerSec = 60;
	public override void Init()
	{
		base.Init();
        rangeIndicator.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
        rangeLine.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
		startLocation = transform.position;
		moveTimer = new Timer(1.0f / ticksPerSec, (int)(moveTime * ticksPerSec));

		moveTimer.OnTick.AddListener(() =>
		{
			if (up)
			{
				transform.position += Vector3.up * moveDistance / (moveTime * ticksPerSec);
				if (transform.position.y - startLocation.y >= moveDistance)
				{
					moveTimer.Stop();
					up = false;
					transform.position = transform.position.SetY(startLocation.y + moveDistance);
				}
			}
			else
			{
				transform.position += Vector3.up * moveDistance / ticksPerSec;

				if (transform.position.y <= startLocation.y)
				{
					transform.position = transform.position.SetY(startLocation.y);
				}
			}
		});

	}

    protected override void OnGameStateChanged(GameState state)
    {
        rangeIndicator.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
        rangeLine.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
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

	public override void ScaleY(bool up)
	{
		if (up) {
			moveDistance++;
		} else if (moveDistance > 1) {
			moveDistance--;
		}
        rangeIndicator.transform.localPosition = Vector3.up * (moveDistance + 0.5f);
        rangeLine.transform.localScale = Vector3.one + Vector3.up * (moveDistance - 1);
	}

#if UNITY_EDITOR
	public override void RenderInEditor()
	{
		base.RenderInEditor();
        
		Handles.color = Color.black;
		Handles.DrawLine(transform.position, transform.position + Vector3.up * moveDistance);
		Handles.DrawSolidArc(transform.position + Vector3.up * moveDistance, -Vector3.forward, Vector3.up, 360, 0.33f);
		Handles.color = Color.white;

	}
#endif
}
