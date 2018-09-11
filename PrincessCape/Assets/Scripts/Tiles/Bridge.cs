using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : SegmentedTile
{
    private void Start()
    {
        Init();
    }

    public override void Deactivate()
    {
        base.Deactivate();
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		isActivated = false;

    }

    public override void ScaleY(bool up)
    {

    }

    public override void ScaleX(bool right)
    {
        if (right)
        {
            SpawnSegment();
        }
        else if (transform.childCount > 0)
        {
            DestroyImmediate(LastChild);
        }
    }

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
                if (transform.childCount > 0)
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

    protected override void SpawnSegment()
    {
        base.SpawnSegment();
        LastTransform.position += Vector3.down / 2.0f;
    }
}
