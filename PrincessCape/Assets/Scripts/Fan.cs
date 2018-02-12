using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : ActivatedObject
{
    Animator myAnimator;
    public override void Activate()
    {
        isActivated = true;
        transform.GetChild(0).gameObject.SetActive(true);
        myAnimator.SetBool("isActivated", true);
    }

    public override void Deactivate()
    {
        isActivated = false;
        transform.GetChild(0).gameObject.SetActive(false);
        myAnimator.SetBool("isActivated", false);
    }

    // Use this for initialization
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        transform.GetChild(0).gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    public override void ScaleY(bool up)
    {
        transform.GetChild(0).GetComponent<AirColumn>().ScaleY(up);
    }

    public override string SaveData()
    {
        string info = "{\n";
        info += string.Format("\"Name\": \"{0}\"", name.Split('(')[0]) + lineEnding;
        info += string.Format("\"ID\": \"{0}\"", ID) + lineEnding;
        info += string.Format("\"Position\": \"{0}\"", transform.position) + lineEnding;
        info += string.Format("\"Rotation\": \"{0}\"", transform.rotation) + lineEnding;
        info += string.Format("\"Scale\": \"{0}\"", transform.localScale) + lineEnding;
        info += string.Format("\"Air Scale\": \"{0}\"", transform.GetChild(0).localScale.y) + lineEnding;
        info += "}" + lineEnding;
        return info;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        GameObject air = transform.GetChild(0).gameObject;
        float airScale = float.Parse(PCLParser.ParseLine(tile.info[3]));
        air.transform.localScale = air.transform.localScale.SetY(airScale);
    }
#endif
}
