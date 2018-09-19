using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MapTile
{
    [SerializeField]
    GameObject reflectSurface;
    /// <summary>
    /// Rotates the mirror surface by the given angle
    /// </summary>
    /// <param name="ang">Ang.</param>
    public override void Rotate(float ang)
    {
        if (Mathf.Abs(reflectSurface.transform.rotation.z + ang) >= 90)
        {
            reflectSurface.transform.rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
        }
    }

    /// <summary>
    /// Whether or not the position overlaps 
    /// </summary>
    /// <returns>The overlaps.</returns>
    /// <param name="pos">Position.</param>
    public override bool Overlaps(Vector3 pos)
    {
        Vector3 dif = pos - transform.position;
        Vector3 bounds = Bounds / 2;
        return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
    }

    /// <summary>
    /// Gets the bounds of the Mirror.
    /// </summary>
    /// <value>The bounds.</value>
    public override Vector2 Bounds
    {
        get
        {
            return Vector2.one + Vector2.up / 2;
        }
    }

    /// <summary>
    /// Generates a string of save data for the mirror
    /// </summary>
    /// <returns>The save data.</returns>
    protected override string GenerateSaveData()
    {
        string info = "";
        info += PCLParser.CreateAttribute("Prefab", name.Split('(')[0]);
        info += PCLParser.CreateAttribute("Instance Name", instanceName);
        info += PCLParser.CreateAttribute("ID", ID);
        info += PCLParser.CreateAttribute("Position", transform.position);
        float ang = 0;
        Vector3 ax = Vector3.zero;
        reflectSurface.transform.rotation.ToAngleAxis(out ang, out ax);
        ax = new Vector3(0, 0, ang);
        info += PCLParser.CreateAttribute("Rotation", ax);
        info += PCLParser.CreateAttribute("Scale", transform.localScale);
        return info;
    }

    /// <summary>
    /// Sets the variables of a Mirror from the data in the tile given
    /// </summary>
    /// <param name="tile">Tile.</param>
    public override void FromData(TileStruct tile)
    {
        ID = tile.id;
        transform.position = PCLParser.ParseVector3(tile.NextLine).SetZ((float)Layer);
        Vector3 rot = PCLParser.ParseVector3(tile.NextLine);

        transform.localScale = PCLParser.ParseVector3(tile.NextLine);
        reflectSurface.transform.rotation *= Quaternion.AngleAxis(rot.z, Vector3.forward);
    }
}
