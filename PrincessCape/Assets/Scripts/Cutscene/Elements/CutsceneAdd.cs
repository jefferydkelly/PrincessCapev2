using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneAdd : CutsceneElement
{
    string itemName;

    public CutsceneAdd(string item)
    {
        itemName = item;
        autoAdvance = true;
        canSkip = false;
    }

    public override Timer Run()
    {
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemName) as MagicItem, false);
        return null;
    }
}
