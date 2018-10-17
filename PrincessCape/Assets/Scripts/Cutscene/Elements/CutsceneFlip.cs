using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneFlip : CutsceneElement
{
    string affected = "Character";
    bool horizontalFlip = false;
    public CutsceneFlip(string target, bool horizontal)
    {
        affected = target;
        horizontalFlip = horizontal;
        autoAdvance = true;
    }

    public override Timer Run()
    {
        CutsceneActor myActor = Cutscene.Instance.FindActor(affected);
        if (horizontalFlip)
        {
            myActor.FlipX();
        }
        else
        {
            myActor.FlipY();
        }
        return null;
    }
}

#if UNITY_EDITOR
public class FlipEditor : CutsceneElementEditor
{
    bool horizontal = true;

    public FlipEditor()
    {
        editorType = "Flip Sprite";
        type = CutsceneElements.Flip;
    }
    public override void GenerateFromData(string[] data)
    {
        horizontal = PCLParser.ParseBool(data[0]);
    }

    public override string GenerateSaveData(bool json)
    {
        return PCLParser.CreateAttribute("Horizontal", horizontal);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        horizontal = EditorGUILayout.Toggle("Flip Horizontal", horizontal);
    }
}
#endif