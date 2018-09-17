using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : SegmentedTile {
    /// <summary>
    /// Spawns a new segment or destroys a segment.
    /// </summary>
    /// <param name="right">If set to <c>true</c> spawns a new segment or destroys the last segment if there is one.</param>
	public override void ScaleX(bool right)
    {
        if (right) {
            SpawnSegment();
        } else if (NumSegments > 0) {
            DestroyImmediate(LastChild, false);
        }
    }

    /// <summary>
    /// Modifies the scale of the stairs by the given vector
    /// </summary>
    /// <param name="vec">Vec.</param>
    public override void Scale(Vector3 vec)
    {
        base.Scale(vec.SetX(0));
        Vector3 scale = LastTransform.localScale;
        scale += vec.SetY(0);
        if (vec.x > 0) {
            if (scale.x > 1) {
                LastTransform.localScale = LastTransform.localScale.SetX(1);
                SpawnSegment();
                float newX = scale.x - 1;
                LastTransform.localScale = LastTransform.localScale.SetX(newX);
                LastTransform.localPosition -= Vector3.right * (1 - newX) / 2;
            } else {
                LastTransform.localScale = scale;
                LastTransform.localPosition += vec.SetY(0) / 2;
            }
        } else if (vec.x < 0){
            if (scale.x < 0) {
                if (NumSegments > 0)
                {
                    DestroyImmediate(LastChild, false);
                    LastTransform.localScale = LastTransform.localScale - scale.SetY(0);
                }
            } else {
                LastTransform.localScale = scale;
                LastTransform.localPosition += vec.SetY(0) / 2;
            }
        }
    }

    /// <summary>
    /// Spawns a new segment.
    /// </summary>
    protected override void SpawnSegment()
    {
        base.SpawnSegment();
        Vector3 scale = Vector3.up * NumSegments / 4.0f;
        LastTransform.localScale = Vector3.one + scale ;
        LastTransform.position += scale / 2.0f;
    }

    /// <summary>
    /// Updates the segments.
    /// </summary>
    protected override void UpdateSegments()
    {
        for (int i = 0; i < NumSegments; i++)
        {
            Transform child = transform.GetChild(i + segmentStart);
            child.localPosition = SpawnDirection * (i + 1) + Vector3.up * (i + 1) / 8.0f;
        }

    }
}
