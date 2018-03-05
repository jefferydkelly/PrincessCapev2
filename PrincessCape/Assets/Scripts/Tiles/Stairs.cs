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
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject, false);
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
