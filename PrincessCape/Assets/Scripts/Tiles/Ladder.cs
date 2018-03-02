using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ladder : ActivatedObject
{
    [SerializeField]
    GameObject plainLadder;
    BoxCollider2D myCollider;
    Timer revealTimer;
    float revealTime = 0.25f;
    public void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();

		revealTimer = new Timer(revealTime, transform.childCount - 1);
		revealTimer.OnTick.AddListener(RevealTile);
		if (!startActive)
		{
			Deactivate();
		}
    }

    public override void ScaleY(bool up)
    {
        if (up)
        {

            transform.position += Vector3.up;
            /*
            GameObject newChain = Instantiate(plainLadder);

            newChain.transform.SetParent(transform);
            newChain.transform.position = transform.position + Vector3.down * (transform.childCount - 1);
            newChain.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
            */
            SpawnChild();
        }
        else if (transform.childCount > 1)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            transform.position += Vector3.down;
			myCollider.size = myCollider.size.SetY(transform.childCount);
			myCollider.offset = new Vector2(0, -(myCollider.size.y - 1) / 2);
        }


    }

    public override MapHighlightState HighlightState
    {
        get
        {
            return base.HighlightState;
        }
        set
        {
            Color nextColor = Color.white;
            if (value == MapHighlightState.Primary)
            {
                nextColor = Color.blue;
            }
            else if (value == MapHighlightState.Secondary)
            {
                nextColor = Color.red;
            }

            foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
            {
                spr.color = nextColor;
            }
        }
    }


    public override bool Overlaps(Vector3 pos)
    {
        Vector3 dif = pos - (transform.position + (Vector3)myCollider.offset);
        Vector3 bounds = myCollider.size / 2;
        return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
    }
    public override bool Overlaps(MapTile other, Vector3 spawnPos)
    {
        Vector3 dif = spawnPos - (transform.position + (Vector3)myCollider.offset);
        Vector3 bounds = (other.Bounds + Bounds) / 2;
        return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
    }

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Lines", transform.childCount - 1);
        return data;
    }
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
       
        int numLinks = PCLParser.ParseInt(tile.NextLine);
        for (int i = 0; i < numLinks; i++)
        {
            /*
            GameObject newChain = Instantiate(plainLadder);

            newChain.transform.SetParent(transform);
            newChain.transform.position = transform.position + Vector3.down * (transform.childCount - 1);
            newChain.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
            */
            SpawnChild();
        }


		
    }

    public override void Activate()
    {
        revealTimer.Start();
    }

    public override void Deactivate()
    {
        revealTimer.Stop();

		for (int i = 1; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
        myCollider.size = Vector2.one;
        myCollider.offset = Vector2.zero;
    }

	void SpawnChild()
	{
        GameObject tile = Instantiate(plainLadder);
		tile.transform.SetParent(transform);
        tile.transform.localPosition = (transform.childCount - 1) * Vector3.down;
        tile.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
		myCollider.size = myCollider.size.SetY(transform.childCount);
		myCollider.offset = new Vector2(0, -(myCollider.size.y - 1) / 2);
	}

	void RevealTile()
	{
		transform.GetChild(revealTimer.TicksCompleted).gameObject.SetActive(true);
        myCollider.size = myCollider.size.SetY(revealTimer.TicksCompleted);
        myCollider.offset = new Vector2(0, -revealTimer.TicksCompleted / 2);
	}
}
