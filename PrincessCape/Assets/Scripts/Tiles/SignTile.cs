using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sign))]
[ExecuteInEditMode]
public class SignTile : MapTile {
    Sign sign;

    private void Awake()
    {
        Init();
    }

    public override void Init() {
        sign = GetComponent<Sign>();
    }

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Text", sign.Text.name);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
		string fileName = PCLParser.ParseLine(tile.NextLine);
        sign.Text = Resources.Load<TextAsset>("Signs/" + fileName);
    }

}
