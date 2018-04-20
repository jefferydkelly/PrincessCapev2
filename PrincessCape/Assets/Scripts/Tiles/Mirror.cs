using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MapTile
{
    [SerializeField]
    GameObject reflectSurface;
    public override void Rotate(float ang)
    {
        if (Mathf.Abs(reflectSurface.transform.rotation.z + ang) >= 90)
        {
            reflectSurface.transform.rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
        }
    }

    public override bool Overlaps(Vector3 pos)
    {
        Vector3 dif = pos - transform.position;
        Vector3 bounds = Bounds / 2;
        return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
    }

    public override Vector3 Bounds
    {
        get
        {
            return Vector2.one + Vector2.up / 2;
        }
    }

    protected override string GenerateSaveData()
    {
        string info = "";
        info += PCLParser.CreateAttribute("Name", name.Split('(')[0]);
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

    public override void FromData(TileStruct tile)
    {
        ID = tile.id;
        transform.position = PCLParser.ParseVector3(tile.NextLine).SetZ((float)Layer);
        Vector3 rot = PCLParser.ParseVector3(tile.NextLine);

        transform.localScale = PCLParser.ParseVector3(tile.NextLine);
        reflectSurface.transform.rotation *= Quaternion.AngleAxis(rot.z, Vector3.forward);
    }
}
