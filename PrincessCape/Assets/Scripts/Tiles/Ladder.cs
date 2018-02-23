using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ladder : MapTile
{
    [SerializeField]
    GameObject plainLadder;
    BoxCollider2D myCollider;
    public void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    public override void ScaleY(bool up)
    {
        if (up)
        {

            transform.position += Vector3.up;
            GameObject newChain = Instantiate(plainLadder);

            newChain.transform.SetParent(transform);
            newChain.transform.position = transform.position + Vector3.down * (transform.childCount - 1);
            newChain.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        }
        else if (transform.childCount > 1)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            transform.position += Vector3.down;
        }

        myCollider.size = myCollider.size.SetY(transform.childCount);
        myCollider.offset = new Vector2(0, -(myCollider.size.y - 1) / 2);
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

        int numLinks = PCLParser.ParseInt(tile.info[3]);
        for (int i = 0; i < numLinks; i++)
        {
            GameObject newChain = Instantiate(plainLadder);

            newChain.transform.SetParent(transform);
            newChain.transform.position = transform.position + Vector3.down * (transform.childCount - 1);
            newChain.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        }

		myCollider.size = myCollider.size.SetY(transform.childCount);
		myCollider.offset = new Vector2(0, -(myCollider.size.y - 1) / 2);
    }
}
