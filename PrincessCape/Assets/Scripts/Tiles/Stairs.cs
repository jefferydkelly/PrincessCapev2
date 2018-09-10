using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MapTile {
    [SerializeField]
    GameObject stair;
    [SerializeField]
    int dir = 1;
	public override void ScaleX(bool right)
    {
        if (right) {
            SpawnChild();
        } else if (transform.childCount > 0) {
            DestroyImmediate(LastChild, false);
        }
    }

    public override void Scale(Vector3 vec)
    {
        base.Scale(vec.SetX(0));
        Vector3 scale = LastTransform.localScale;
        scale += vec.SetY(0);
        if (vec.x > 0) {
            if (scale.x > 1) {
                LastTransform.localScale = LastTransform.localScale.SetX(1);
                SpawnChild();
                float newX = scale.x - 1;
                LastTransform.localScale = LastTransform.localScale.SetX(newX);
                LastTransform.localPosition -= Vector3.right * (1 - newX) / 2;
            } else {
                LastTransform.localScale = scale;
                LastTransform.localPosition += vec.SetY(0) / 2;
            }
        } else if (vec.x < 0){
            if (scale.x < 0) {
                if (transform.childCount > 0)
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

    GameObject LastChild {
        get {
            if (transform.childCount > 0)
            {
                return transform.GetChild(transform.childCount - 1).gameObject;
            } else {
                return gameObject;
            }
        }
    }

    Transform LastTransform {
        get {
            if (transform.childCount > 0)
            {
                return transform.GetChild(transform.childCount - 1);
            } else {
                return transform;
            }
        }
    }

    protected override string GenerateSaveData()
    {
        string info = base.GenerateSaveData();
        info += PCLParser.CreateAttribute("Children", transform.childCount);
        return info;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        int numChildren = PCLParser.ParseInt(tile.NextLine);

        for (int i = 0; i < numChildren; i++) {
            SpawnChild();
        }
    }

    void SpawnChild() {
		GameObject child = Instantiate(stair);
		child.transform.SetParent(transform);
		child.transform.localScale = stair.transform.localScale.SetY(1 + transform.childCount / 2.0f);
		child.transform.localPosition = transform.right * dir * transform.childCount + (transform.up * (transform.childCount) / 4.0f);
    }

    public override void FlipX()
    {
        dir *= -1;
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            child.localPosition = child.localPosition.SetX(dir * i);
        }
    }
}
