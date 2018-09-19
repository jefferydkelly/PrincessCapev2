using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : ActivatedObject {

	[SerializeField]
	protected Vector3 direction = Vector3.right;
	[SerializeField]
	protected float travelTime = 1.0f;
	protected float travelDistance = 2.0f;
    protected float minimumDistance = 2.0f;
	protected Timer moveTimer;


    [SerializeField]
    protected GameObject rangeIndicator;
    [SerializeField]
    protected GameObject rangeLine;

	private void Awake()
	{
        Init();
	}

    /// <summary>
    /// Activate this instance.
    /// </summary>
	public override void Activate()
	{
        IsActivated = true;
		if (moveTimer.IsPaused) {
			moveTimer.Unpause();
		} else {
			moveTimer.Start();
		}

		
	}

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
	public override void Deactivate()
	{
		moveTimer.Pause();
        IsActivated = false;
	}

    /// <summary>
    /// Initializes the Moving Platform
    /// </summary>
	public override void Init()
	{
		if (!initialized)
		{
			travelTime = travelTime > 0.0f ? travelTime : 1.0f;
			

            rangeIndicator.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
            rangeLine.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);

			base.Init();
			initialized = true;
		}
	}

    /// <summary>
    /// Handles the changing of the game state.
    /// </summary>
    /// <param name="state">State.</param>
    protected override void OnGameStateChanged(GameState state)
    {
        rangeIndicator.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
        rangeLine.SetActive(Game.Instance.IsInLevelEditor && !Game.Instance.IsPlaying);
    }

    /// <summary>
    /// An event for when the game object is destroyed
    /// </summary>
	private void OnDestroy()
	{
		moveTimer.Stop();
	}
    /// <summary>
    /// Increases or decreases the distance this travels by one
    /// </summary>
    /// <param name="up">If set to <c>true</c> increases the the distance, otherwise dercreases it..</param>
	public override void ScaleY(bool up)
	{
		if (up) {
			TravelDistance++;
		} else {
			TravelDistance--;
		}
	}

	public override void ScaleX(bool right)
	{
		base.ScaleX(right);
		if (direction == Vector3.right || direction == Vector3.left)
		{
			if (right)
			{
				TravelDistance++;
			}
			else
			{
				TravelDistance--;
			}
		}
	}

    /// <summary>
    /// Gets or sets the travel distance.
    /// </summary>
    /// <value>The travel distance.</value>
    float TravelDistance {
        get {
            return travelDistance;
        }

        set {
            travelDistance = Mathf.Max(value, minimumDistance);
            UpdateIndicator();

        }
    }

    public Vector3 EndPoint {
        get {
            return rangeIndicator.transform.position;
        }

        set {
            Vector3 ep = (value - transform.position);
            direction = ep.normalized;
            TravelDistance = ep.magnitude;
            rangeLine.transform.rotation = Quaternion.AngleAxis(((Vector2)ep).Angle().ToDegrees(), Vector3.forward);
        }
    }

    protected virtual void UpdateIndicator() {
        rangeIndicator.transform.localPosition = direction * travelDistance;
        rangeLine.transform.localScale = Vector3.one + Vector3.right * (travelDistance - 1);
    }

    /// <summary>
    /// Rotates the travel direction by the given angle
    /// </summary>
    /// <param name="ang">Ang.</param>
	public override void Rotate(float ang)
	{
		direction = ((Vector2)(direction)).RotateDeg(ang);
        direction = new Vector3(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
		transform.GetChild(0).rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
        rangeLine.transform.rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
        rangeIndicator.transform.localPosition = direction * travelDistance;

        if (Equals(direction, Vector3.up)) {
            minimumDistance = 1.0f;
        } else {
            minimumDistance = 2.0f;
        }
	}

    /// <summary>
    /// Generates the save data for this tile/
    /// </summary>
    /// <returns>The save data.</returns>
	protected override string GenerateSaveData()
	{
		string data = base.GenerateSaveData();
		data += PCLParser.CreateAttribute<Vector3>("Direction", direction);
		data += PCLParser.CreateAttribute<float>("Travel Distance", travelDistance);
		data += PCLParser.CreateAttribute<float>("Travel Time", travelTime);
		return data;
	}

    /// <summary>
    /// Creates an instance of the tile from the given data
    /// </summary>
    /// <param name="tile">Tile.</param>
	public override void FromData(TileStruct tile)
	{
		base.FromData(tile);
		direction = PCLParser.ParseVector3(tile.NextLine);
		travelDistance = PCLParser.ParseFloat(tile.NextLine);
		travelTime = PCLParser.ParseFloat(tile.NextLine);
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
