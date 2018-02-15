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

    public override string SaveData()
    {
		if (transform.parent.name != "Map")
		{
			return "";
		}
		string info = "{\n";
		info += string.Format("\"Name\": \"{0}\"", name.Split('(')[0]) + lineEnding;
		info += string.Format("\"ID\": \"{0}\"", ID) + lineEnding;
		info += string.Format("\"Position\": \"{0}\"", transform.position) + lineEnding;
		info += string.Format("\"Rotation\": \"{0}\"", transform.rotation) + lineEnding;
		info += string.Format("\"Scale\": \"{0}\"", transform.localScale) + lineEnding;
        info += string.Format("\"Children\": \"{0}\"", transform.childCount) + lineEnding;
		info += "}" + lineEnding;
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
