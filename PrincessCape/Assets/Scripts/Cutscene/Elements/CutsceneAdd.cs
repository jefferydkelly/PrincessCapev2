using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
/// <summary>
/// An editor for adding an object to the players inventory
/// </summary>
public class AddEditor : CutsceneElementEditor
{
    ItemLevel item;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:AddEditor"/> class.
    /// </summary>
    public AddEditor()
    {
        editorType = "Add Magic Item";
        type = CutsceneElements.Add;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        item = (ItemLevel)EditorGUILayout.EnumPopup("Magic Item", item);
    }

    public override void GenerateFromData(string[] data)
    {
        item = PCLParser.ParseEnum<ItemLevel>(data[0]);
    }

    public override string GenerateSaveData(bool json)
    {
        return PCLParser.CreateAttribute("Magic Item", item);
    }
}
#endif