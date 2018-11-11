using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneAdd : CutsceneElement
{
    ItemLevel item;

    public CutsceneAdd(ItemLevel iLevel = ItemLevel.None)
    {
        item = iLevel;
        autoAdvance = true;
        canSkip = false;
        type = CutsceneElements.Add;
    }

    public override string SaveData
    {
        get
        {
            return PCLParser.CreateAttribute("Magic Item", item);
        }
    }

    public override string ToText {
        get {
            return string.Format("add {0}", item);
        }
    }

    public override void CreateFromText(string[] data)
    {
        item = (ItemLevel)System.Enum.Parse(typeof(ItemLevel), data[1]);
    }

    public override void CreateFromJSON(string[] data)
    {
        item = PCLParser.ParseEnum<ItemLevel>(data[0]);
    }
#if UNITY_EDITOR
    public override void RenderEditor()
    {
        item = (ItemLevel)EditorGUILayout.EnumPopup("Magic Item", item);
    }
#endif

    public override Timer Run()
    {
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(item.ToString()) as MagicItem, false);
        return null;
    }

    public override void Skip()
    {
        Run();
    }
}