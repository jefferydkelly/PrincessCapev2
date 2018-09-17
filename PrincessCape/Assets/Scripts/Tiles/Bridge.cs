using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : SegmentedTile
{
    /// <summary>
    /// Deactivate this bridge and hides all segments.
    /// </summary>
    public override void Deactivate()
    {
        base.Deactivate();
        for (int i = 0; i < NumSegments; i++)
		{
            transform.GetChild(i + segmentStart).gameObject.SetActive(false);
		}
		isActivated = false;

    }

    /// <summary>
    /// Keeps the bridge from scaling upwards
    /// </summary>
    /// <param name="up">If set to <c>true</c> up.</param>
    public override void ScaleY(bool up)
    {

    }

    /// <summary>
    /// Adds or removes a segment to the bridge.  Doesn't remove the end segment.
    /// </summary>
    /// <param name="right">If set to <c>true</c>, adds a new segment.  Removes the last segment (if there is one) otherwise..</param>
    public override void ScaleX(bool right)
    {
        if (right)
        {
            SpawnSegment();
        }
        else if (NumSegments > 0)
        {
            DestroyImmediate(LastChild);
        }
    }

    /// <summary>
    /// Modifies the scale of the bridge by the given vector.
    /// </summary>
    /// <param name="vec">Vec.</param>
    public override void Scale(Vector3 vec)
    {
        base.Scale(vec.SetX(0));
        Vector3 scale = LastTransform.localScale;
        scale += vec.SetY(0);
        if (vec.x > 0)
        {
            if (scale.x > 1)
            {
                LastTransform.localScale = LastTransform.localScale.SetX(1);
                SpawnSegment();
                float newX = scale.x - 1;
                LastTransform.localScale = LastTransform.localScale.SetX(newX);
                LastTransform.localPosition -= Vector3.right * (1 - newX) / 2;
            }
            else
            {
                LastTransform.localScale = scale;
                LastTransform.localPosition += vec.SetY(0) / 2;
            }
        }
        else if (vec.x < 0)
        {
            if (scale.x < 0)
            {
                if (NumSegments > 0)
                {
                    DestroyImmediate(LastChild, false);
                    LastTransform.localScale = LastTransform.localScale - scale.SetY(0);
                }
            }
            else
            {
                LastTransform.localScale = scale;
                LastTransform.localPosition += vec.SetY(0) / 2;
            }
        }
    }

    /// <summary>
    /// Spawns a new segment of the bridge.
    /// </summary>
    protected override void SpawnSegment()
    {
        base.SpawnSegment();
        LastTransform.position += Vector3.down / 2.0f;
    }
}
