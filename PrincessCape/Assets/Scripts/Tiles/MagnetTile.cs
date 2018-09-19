using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MagnetTile : ActivatedObject {
    [SerializeField]
    MagneticField magField;

    /// <summary>
    /// Activates the attached magnetic field
    /// </summary>
    public override void Activate()
    {
        IsActivated = true;
        magField.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the attached magnetic field
    /// </summary>
    public override void Deactivate()
    {
        IsActivated = false;
        magField.gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes the magnetic tile
    /// </summary>
    public override void Init() {
        if (Application.isPlaying)
        {
            base.Init();
            magField.gameObject.SetActive(startActive);
        }
        
    }

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (!initialized)
            {
                Init();
            }
        } else {
            magField.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Increases the height of the magnetic field
    /// </summary>
    /// <param name="up">If set to <c>true</c> increases the height.  Otherwise, decreases it.</param>
    public override void ScaleY(bool up)
    {
        if (!magField) {
            magField = GetComponentInChildren<MagneticField>();
        }

        magField.ScaleY(up);
    }

    /// <summary>
    /// Generates the save data for the MagnetTile
    /// </summary>
    /// <returns>The save data.</returns>
    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Field Height", magField.transform.localScale.y);
        return data;
    }

    /// <summary>
    /// Creates a MagnetTile from the TileStruct
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        float fieldHeight = PCLParser.ParseFloat(tile.NextLine);
        magField.ScaleY(fieldHeight);
    }
}
