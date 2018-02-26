using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : ActivatedObject
{
    [SerializeField]
    GameObject bridgeTile;
    [SerializeField]
    Vector3 dir = Vector3.right;
    Timer revealTimer;
    float revealTime = 0.1f;

    private void Start()
    {
		revealTimer = new Timer(revealTime, transform.childCount - 1);
		revealTimer.OnTick.AddListener(RevealTile);
        if (!startActive)
        {
            Deactivate();
        }

    }
    public override void Activate()
    {
        revealTimer.Start();
    }

    void RevealTile()
    {
        transform.GetChild(revealTimer.TicksCompleted).gameObject.SetActive(true);

    }

    public override void Deactivate()
    {
        revealTimer.Stop();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

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
        int numChildren = PCLParser.ParseInt(tile.NextLine);

		for (int i = 0; i < numChildren; i++)
		{
			SpawnChild();
		}
	}
}
