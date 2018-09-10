using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedTile : MapTile
{
    [SerializeField]
    GameObject segment;
    [SerializeField]
    Direction spawnDirection = Direction.Right;
    [SerializeField]
    protected int segmentStart = 0;

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
        return info;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        int numChildren = PCLParser.ParseInt(tile.NextLine);

        for (int i = 0; i < numChildren; i++)
        {
            SpawnSegment();
        }
    }

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

    void UpdateSegments() {
        for (int i = 0; i < transform.childCount - segmentStart; i++)
        {
            Transform child = transform.GetChild(i + segmentStart);
            child.localPosition = SpawnDirection * i;
        }
    }

    protected virtual void SpawnSegment()
    {
        GameObject child = Instantiate(segment);
        child.transform.SetParent(transform);
        child.transform.localPosition = SpawnDirection * ((transform.childCount - segmentStart));
    }

    protected int NumSegments {
        get {
            return transform.childCount - segmentStart;
        }
    }
    Vector3 SpawnDirection {
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
}
