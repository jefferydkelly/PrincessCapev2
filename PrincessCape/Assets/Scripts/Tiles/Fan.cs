using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : ActivatedObject
{
    [SerializeField]
    AirColumn air;

    /// <summary>
    /// Turns on the air field
    /// </summary>
    public override void Activate()
    {
        IsActivated = true;
        air.gameObject.SetActive(true);
    }

    /// <summary>
    /// Turns of the air field 
    /// </summary>
    public override void Deactivate()
    {
        IsActivated = false;
        air.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        Init();
    }

    /// <summary>
    /// Initializes the Fan.
    /// </summary>
    public override void Init() {
        if (Application.isPlaying)
        {
            air.gameObject.SetActive(Game.Instance.IsInLevelEditor);

            if (startActive)
            {
                Activate();
            }
        } else {
            air.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Increases the size of the air column 
    /// </summary>
    /// <param name="up">If set to <c>true</c> increases the size.  Decreases otherwise.</param>
    public override void ScaleY(bool up)
    {
        air.ScaleY(up);
    }

    /// <summary>
    /// Generates the save data for this tile.
    /// </summary>
    /// <returns>The string of save data.</returns>
    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Air Scale", air.transform.localScale.y);
        return data;
    }

    /// <summary>
    /// Creates a new Fan tile from the TileStruct.
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        float airScale = PCLParser.ParseFloat(tile.NextLine);//float.Parse(PCLParser.ParseLine(tile.NextLine));
        air.transform.localScale = air.transform.localScale.SetY(airScale);
        air.transform.localPosition = Vector3.up * (1 + (airScale - 1) / 2);
    }
}
