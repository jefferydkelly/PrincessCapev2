using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedTile : ActivatedObject
{
    [SerializeField]
    GameObject segment;
    [SerializeField]
    Direction spawnDirection = Direction.Right;
    [SerializeField]
    protected int segmentStart = 0;

    protected Timer revealTimer;
    protected float revealTime = 0.25f;

    public override void Init()
    {
        initialized = false;
        revealTimer = new Timer(revealTime, NumSegments);
        revealTimer.OnTick.AddListener(RevealSegment);

        base.Init();
        initialized = true;
    }

    public override void Activate()
    {
        if (Game.Instance.IsPlaying && !(startActive && !initialized)) {
            gameObject.SetActive(true);
            revealTimer.Start();
        }
        else
        {
            RevealAllSegments();
        }
    }

    public override void Deactivate()
    {
        gameObject.SetActive(false);
        revealTimer.Stop();

        for (int i = segmentStart; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    protected GameObject LastChild
    {
        get
        {
            if (transform.childCount > segmentStart)
            {
                return transform.GetChild(transform.childCount - 1).gameObject;
            }
            else
            {
                return gameObject;
            }
        }
    }

    protected Transform LastTransform
    {
        get
        {
            if (transform.childCount > segmentStart)
            {
                return transform.GetChild(transform.childCount - 1);
            }
            else
            {
                return transform;
            }
        }
    }

    protected override string GenerateSaveData()
    {
        string info = base.GenerateSaveData();
        info += PCLParser.CreateAttribute("Segments", transform.childCount - segmentStart);
        info += PCLParser.CreateAttribute("Final Segment Scale", LastTransform.localScale);
        return info;
    }

    /// <summary>
    /// Creates a ladder from the data given.
    /// </summary>
    /// <param name="tile">A struct containing the information for the ladder.</param>
    public override void FromData(TileStruct tile)
    {
        initialized = false;
        base.FromData(tile);

        int numLinks = PCLParser.ParseInt(tile.NextLine);
        for (int i = 0; i < numLinks; i++)
        {
            SpawnSegment();
        }

        if (!tile.FullyRead) {
            LastTransform.localScale = PCLParser.ParseVector3(tile.NextLine);
        }

        if (!startActive && Application.isPlaying)
        {
            Deactivate();
        }

        revealTimer = new Timer(revealTime, numLinks);
        revealTimer.OnTick.AddListener(RevealSegment);
        initialized = true;

    }

    /// <summary>
    /// Flips the tile horizontally
    /// </summary>
    public override void FlipX()
    {
        if (spawnDirection == Direction.Left) {
            spawnDirection = Direction.Right;
            UpdateSegments();
        } else if (spawnDirection == Direction.Right) {
            spawnDirection = Direction.Left;
            UpdateSegments();
        }
    }

    /// <summary>
    /// Flips the tile veritcally
    /// </summary>
    public override void FlipY()
    {
        if (spawnDirection == Direction.Up)
        {
            spawnDirection = Direction.Down;
            UpdateSegments();
        }
        else if (spawnDirection == Direction.Down)
        {
            spawnDirection = Direction.Up;
            UpdateSegments();
        }
    }

    /// <summary>
    /// Updates the positions of all the segments of the tile
    /// </summary>
    protected virtual void UpdateSegments() {
        for (int i = 0; i < NumSegments; i++)
        {
            Transform child = transform.GetChild(i + segmentStart);
            child.localPosition = SpawnDirection * (i + 1);
        }
    }

    /// <summary>
    /// Spawns a new segment of the tile
    /// </summary>
    protected virtual void SpawnSegment()
    {
        GameObject child = Instantiate(segment);
        child.transform.SetParent(transform);
        child.transform.localPosition = SpawnDirection * ((transform.childCount - segmentStart));
        SpriteRenderer spr = child.GetComponent<SpriteRenderer>();

        if (HighlightState == MapHighlightState.Primary)
        {
            spr.color = Color.blue;
        }

    }

    /// <summary>
    /// Reveals a segment of the tile
    /// </summary>
    protected virtual void RevealSegment()
    {
        if (revealTimer.IsRunning)
        {
            transform.GetChild(revealTimer.TicksCompleted + segmentStart).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Reveals all segments of the tile.
    /// </summary>
    protected virtual void RevealAllSegments() {
        gameObject.SetActive(true);
        for (int i = segmentStart; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Gets the number of segments.
    /// </summary>
    /// <value>The number of segments.</value>
    protected int NumSegments {
        get {
            return transform.childCount - segmentStart;
        }
    }

    /// <summary>
    /// Gets the direction in which new elements are spawned.
    /// </summary>
    /// <value>The direction in which new elements are spawned.</value>
    protected Vector3 SpawnDirection {
        get {
            switch(spawnDirection) {
                case Direction.Up:
                    return Vector3.up;
                case Direction.Right:
                    return Vector3.right;
                case Direction.Down:
                    return Vector3.down;
                case Direction.Left:
                    return Vector3.left;
                default:
                    return Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Gets or sets the state of the highlight.
    /// </summary>
    /// <value>The state of the highlight.</value>
    public override MapHighlightState HighlightState
    {
        get
        {
            return base.HighlightState;
        }
        set
        {
            highlightState = value;
            Color nextColor = Color.white;
            if (value == MapHighlightState.Primary)
            {
                nextColor = Color.blue;
            }
            else if (value == MapHighlightState.Secondary)
            {
                nextColor = Color.red;
            }
            else if (value == MapHighlightState.Backup)
            {
                nextColor = Color.cyan;
            }

            foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
            {
                spr.color = nextColor;
            }
        }
    }
}
