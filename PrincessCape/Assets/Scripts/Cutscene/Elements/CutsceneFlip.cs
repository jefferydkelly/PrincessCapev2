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
    string character;
    public FlipEditor()
    {
        editorType = "Flip Sprite";
        type = CutsceneElements.Flip;
    }
    public override void GenerateFromData(string[] data)
    {
        character = PCLParser.ParseLine(data[0]);
        horizontal = PCLParser.ParseBool(data[1]);
    }

    public override string GenerateSaveData(bool json)
    {
        string data = "";
        data += PCLParser.CreateAttribute("Character", character);
        data += PCLParser.CreateAttribute("Horizontal", horizontal);
        return data;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        horizontal = EditorGUILayout.Toggle("Flip Horizontal", horizontal);
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("flip-{0} {1}", horizontal ? "x" : "y", character);
        }
    }
}
#endif