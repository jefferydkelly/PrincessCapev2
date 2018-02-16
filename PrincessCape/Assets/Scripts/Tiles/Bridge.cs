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

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Children", transform.childCount);
        return data;
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
