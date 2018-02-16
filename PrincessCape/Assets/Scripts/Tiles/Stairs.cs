using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MapTile {
    [SerializeField]
    GameObject stair;
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
        int numChildren = PCLParser.ParseInt(tile.info[3]);

        for (int i = 0; i < numChildren; i++) {
            SpawnChild();
        }
    }

    void SpawnChild() {
		GameObject child = Instantiate(stair);
		child.transform.SetParent(transform);
		child.transform.localScale = stair.transform.localScale.SetY(1 + transform.childCount / 2.0f);
		child.transform.localPosition = transform.right * transform.childCount + (transform.up * (transform.childCount) / 4.0f);
    }
}
