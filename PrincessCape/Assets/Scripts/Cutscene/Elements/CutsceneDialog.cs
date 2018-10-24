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

    /// <summary>
    /// Initializes a new instance of the <see cref="CutsceneDialog"/> class with a speaker and a line.
    /// </summary>
    /// <param name="spk">Spk.</param>
    /// <param name="dia">Dia.</param>
    public CutsceneDialog(string spk, string dia)
    {
        speaker = spk;
        dialog = dia.Replace("\\n", "\n").Trim();
        canSkip = true;
        autoAdvance = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CutsceneDialog"/> class for duration.
    /// </summary>
    /// <param name="dia">Dia.</param>
    public CutsceneDialog(string dia)
    {
        speaker = null;
        dialog = dia.Replace("\\n", "\n").Trim();
        autoAdvance = false;
    }

    /// <summary>
    /// Displays the dialog of message
    /// </summary>
    public override Timer Run()
    {
        return UIManager.Instance.ShowMessage(dialog, speaker);
    }
}

#if UNITY_EDITOR
public class DialogEditor : CutsceneElementEditor
{
    string speaker = "";
    string line = "";

    public DialogEditor()
    {
        editorType = "Dialog";
        type = CutsceneElements.Dialog;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        speaker = EditorGUILayout.TextField("Speaker", speaker);
        line = EditorGUILayout.TextField("Line", line);
    }

    public override string GenerateSaveData()
    {
        string data = PCLParser.CreateAttribute<string>("Speaker", speaker);
        data += PCLParser.CreateAttribute<string>("Line", line);
        return data;
    }

    public override void GenerateFromData(string[] data)
    {
        speaker = PCLParser.ParseLine(data[0]);
        line = PCLParser.ParseLine(data[1]);
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("{0}: {1}", speaker, line);
        }
    }

    public override void GenerateFromText(string[] data)
    {
        if (data.Length == 2)
        {
            speaker = data[0].Trim();
            line = data[1].Trim();
        } else {
            line = data[0].Trim();
        }
    }
}
#endif