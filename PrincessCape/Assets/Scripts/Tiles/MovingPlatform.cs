using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : ActivatedObject {

	[SerializeField]
	Vector3 direction = Vector3.right;
	[SerializeField]
	float travelTime = 1.0f;
	float travelDistance = 2.0f;
	Timer moveTimer;
	Animator myAnimator;

	private void Awake()
	{
		
	}

	private void Update()
	{
		if (!Game.Instance.IsPaused && moveTimer.IsRunning) {
			transform.position += direction * travelDistance / travelTime * Time.deltaTime;
		}
	}
	public override void Activate()
	{
		if (moveTimer.IsPaused) {
			moveTimer.Unpause();
		} else {
			moveTimer.Start();
		}

		myAnimator.SetBool("IsActive", true);
	}

	public override void Deactivate()
	{
		moveTimer.Pause();
		myAnimator.SetBool("IsActive", false);
	}
    
	public override void Init()
	{
		if (!initialized)
		{
			myAnimator = GetComponentInChildren<Animator>();
			travelTime = travelTime > 0.0f ? travelTime : 1.0f;
			moveTimer = new Timer(travelTime, true);
			moveTimer.OnTick.AddListener(() =>
			{
				direction *= -1;
			});
            
			base.Init();
			initialized = true;
		}
	}

	private void OnDestroy()
	{
		moveTimer.Stop();
	}
	public override void ScaleY(bool up)
	{
		if (up) {
			travelDistance++;
		} else if (travelDistance > transform.localScale.x + 1){
			travelDistance--;
		}
	}

	public override void ScaleX(bool right)
	{
		base.ScaleX(right);
		if (direction == Vector3.right || direction == Vector3.left)
		{
			if (right)
			{
				travelDistance++;
			}
			else
			{
				travelDistance--;
			}
		}
	}

	public override void Rotate(float ang)
	{
		direction = ((Vector2)(direction)).RotateDeg(ang);
		transform.GetChild(0).rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
	}

	protected override string GenerateSaveData()
	{
		string data = base.GenerateSaveData();
		data += PCLParser.CreateAttribute<Vector3>("Direction", direction);
		data += PCLParser.CreateAttribute<float>("Travel Distance", travelDistance);
		data += PCLParser.CreateAttribute<float>("Travel Time", travelTime);
		return data;
	}

	public override void FromData(TileStruct tile)
	{
		base.FromData(tile);
		direction = PCLParser.ParseVector3(tile.NextLine);
		travelDistance = PCLParser.ParseFloat(tile.NextLine);
		travelTime = PCLParser.ParseFloat(tile.NextLine);
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		collision.transform.position += direction * travelDistance / travelTime * Time.deltaTime;
		//collision.rigidbody.AddForce(direction * 8.875f);
	}
    
#if UNITY_EDITOR
	public override void RenderInEditor()
    {
        base.RenderInEditor();

        Handles.color = Color.black;
		Handles.DrawLine(transform.position, transform.position + direction * travelDistance);
		Handles.DrawSolidArc(transform.position + direction * travelDistance, -Vector3.forward, Vector3.up, 360, 0.33f);
        Handles.color = Color.white;

    }
    #endif
}
