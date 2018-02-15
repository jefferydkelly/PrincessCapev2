using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : ActivatedObject
{
    [SerializeField]
    GameObject bridgeTile;
    [SerializeField]
    bool startActive = false;
    [SerializeField]
    Vector3 dir = Vector3.right;

    private void Start()
    {
        if (!startActive)
        {
            Deactivate();
        }
    }
    public override void Activate()
    {
        StartCoroutine(RevealTiles());
    }

    IEnumerator RevealTiles()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            go.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    public override void Deactivate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            go.SetActive(false);

        }
    }

#if UNITY_EDITOR
    public override void ScaleY(bool up)
    {

    }

    public override void ScaleX(bool right)
    {
        if (right)
        {
            SpawnChild();
        }
        else if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }

    void SpawnChild() {
		GameObject tile = Instantiate(bridgeTile);
		tile.transform.SetParent(transform);
        tile.transform.localPosition = dir * transform.childCount + Vector3.down * 0.5f;
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

		for (int i = 0; i < numChildren; i++)
		{
			SpawnChild();
		}
	}
#endif
}
