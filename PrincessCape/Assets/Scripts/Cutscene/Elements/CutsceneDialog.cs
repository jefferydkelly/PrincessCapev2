using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A container for cutscene dialog
/// </summary>
public class CutsceneDialog : CutsceneElement
{
    string speaker = "Character";
    string dialog = "Hi, I'm a character";

    public override string SaveData
    {
        get
        {
            string data = PCLParser.CreateAttribute<string>("Speaker", speaker);
            data += PCLParser.CreateAttribute<string>("Line", dialog);
            return data;
        }
    }

    public override string ToText {
        get {
            return string.Format("{0}: {1}", speaker, dialog);
        }
    }

    public CutsceneDialog() {
        autoAdvance = false;
        canSkip = true;
        type = CutsceneElements.Dialog;
    }

    /// <summary>
    /// Displays the dialog of message
    /// </summary>
    public override Timer Run()
    {
        return UIManager.Instance.ShowMessage(dialog, speaker);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        speaker = EditorGUILayout.TextField("Speaker", speaker);
        dialog = EditorGUILayout.TextField("Line", dialog);
    }
    #endif
    public override void CreateFromJSON(string[] data)
    {
        speaker = PCLParser.ParseLine(data[0]);
        dialog = PCLParser.ParseLine(data[1]);
    }


    public override void CreateFromText(string[] data)
    {
        if (data.Length == 2)
        {
            speaker = data[0].Trim();
            dialog = data[1].Trim();
        }
        else
        {
            speaker = "";
            dialog = data[0].Trim();
        }
    }
}