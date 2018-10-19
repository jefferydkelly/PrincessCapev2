using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneAlign : CutsceneElement
{
    bool left;

    public CutsceneAlign(bool l)
    {
        left = l;
        autoAdvance = true;
        canSkip = true;
    }

    public override Timer Run()
    {
        if (left)
        {
            UIManager.Instance.Alignment = TextAnchor.UpperLeft;
        }
        else
        {
            UIManager.Instance.Alignment = TextAnchor.UpperRight;
        }

        return null;
    }
}
#if UNITY_EDITOR
public class AlignmentEditor : CutsceneElementEditor
{
    bool left = true;

    public AlignmentEditor()
    {
        editorType = "Set Text Alignment";
        type = CutsceneElements.Align;
    }

    public override void GenerateFromData(string[] data)
    {
        left = PCLParser.ParseBool(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Is Left?", left);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        left = EditorGUILayout.Toggle("Is Left Algined?", left);
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("align {0}", left ? "left" : "right");
        }
    }

    public override void GenerateFromText(string[] data)
    {
        left = data[1] == "left";
    }
}
#endif