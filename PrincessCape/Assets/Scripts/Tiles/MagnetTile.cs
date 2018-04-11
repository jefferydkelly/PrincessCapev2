using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MagnetTile : ActivatedObject {
    Animator myAnimator;
    [SerializeField]
    MagneticField magField;

    public override void Activate()
    {
        myAnimator.SetTrigger("Activate");
        magField.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        myAnimator.SetTrigger("Deactivate");
        magField.gameObject.SetActive(false);
    }

    public override void Init() {
        if (Application.isPlaying)
        {
            myAnimator = GetComponent<Animator>();
            magField.gameObject.SetActive(startActive);
            initialized = true;
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

    public override void ScaleY(bool up)
    {
        if (!magField) {
            magField = GetComponentInChildren<MagneticField>();
        }

        magField.ScaleY(up);
    }

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Field Height", magField.transform.localScale.y);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        float fieldHeight = PCLParser.ParseFloat(tile.NextLine);
        magField.ScaleY(fieldHeight);
    }
}
