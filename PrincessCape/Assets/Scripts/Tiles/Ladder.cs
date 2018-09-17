using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ladder : SegmentedTile
{
    BoxCollider2D myCollider;

    public void Awake()
    {
        Init();
    }

    /// <summary>
    /// Initializes this instance setting the collider, reveal timer and determining whether or not it appears at the start of the game.
    /// </summary>
    public override void Init()
    {
        base.Init();
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.size = new Vector2(1, NumSegments + 1);
    }

    /// <summary>
    /// Reveals all segments.
    /// </summary>
    protected override void RevealAllSegments()
    {
        base.RevealAllSegments();
        AdjustSize(NumSegments + 1);
    }

    /// <summary>
    /// Adjusts the size of the collider.
    /// </summary>
    /// <param name="numLinks">Number links.</param>
    void AdjustSize(int numLinks)
    {
        if (myCollider)
        {
            myCollider.size = myCollider.size.SetY(numLinks);

            myCollider.offset = new Vector2(0, -(numLinks - 1) / 2.0f);
        }
    }

    /// <summary>
    /// Scale the specified ladder by the given vector.
    /// </summary>
    /// <param name="vec">Vec.</param>
    public override void Scale(Vector3 vec)
    {
        base.Scale(vec.SetY(0));
        Vector3 scale = LastTransform.localScale;
        scale += vec.SetX(0);
        if (vec.y > 0)
        {
            if (scale.y > 1)
            {
                LastTransform.localScale = LastTransform.localScale.SetY(1);
                SpawnSegment();
                float newY = scale.y - 1;
                LastTransform.localScale = LastTransform.localScale.SetY(newY);
                LastTransform.localPosition += Vector3.up * (1 - newY) / 2;
            }
            else
            {

                LastTransform.localScale = scale;
                LastTransform.localPosition -= vec.SetX(0) / 2;
            }
        }
        else if (vec.y < 0)
        {
            if (scale.y < 0)
            {
                if (transform.childCount > 2)
                {
                    DestroyImmediate(LastChild, false);
                    LastTransform.localScale = LastTransform.localScale - scale.SetX(0);
                    AdjustSize(NumSegments + 1);
                }
            }
            else
            {
                LastTransform.localScale = scale;
                LastTransform.localPosition -= vec.SetX(0) / 2;
            }
        }

    }
    /// <summary>
    /// Scales the Ladder vertically.
    /// </summary>
    /// <param name="up">If set to <c>true</c> increases the scale.  Otherwisem decreases it.</param>
    public override void ScaleY(bool up)
    {
        if (up)
        {

            transform.position += Vector3.up;
            SpawnSegment();
        }
        else if (transform.childCount > segmentStart)
        {
            DestroyImmediate(LastChild, false);
            transform.position += Vector3.down;

        }

        AdjustSize(NumSegments + 1);


    }

    /// <summary>
    /// Gets the center of the ladder.
    /// </summary>
    /// <value>The center.</value>
	public override Vector3 Center
    {
        get
        {
            return transform.position + (Vector3)myCollider.offset;
        }
    }

    /// <summary>
    /// Gets the bounds of the ladder.
    /// </summary>
    /// <value>The bounds.</value>
	public override Vector2 Bounds
    {
        get
        {
            return new Vector2(1, NumSegments + 1);
        }
    }

    /// <summary>
    /// Sets the variables of the Ladder using the information in the Tile Struct
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        myCollider = GetComponent<BoxCollider2D>();
        AdjustSize(NumSegments);
    }

    /// <summary>
    /// Makes all sections of the ladder disappear
    /// </summary>
    public override void Deactivate()
    {
        base.Deactivate();
        AdjustSize(1);
    }
}
