using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Cutscene wait.
/// </summary>
public class CutsceneWait : CutsceneElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CutsceneWait"/> class.
    /// </summary>
    /// <param name="dt">The duration of the wait.</param>
    public CutsceneWait(float dt)
    {
        canSkip = true;
        runTimer = new Timer(dt);
        autoAdvance = true;

    }
    public override Timer Run()
    {
        return runTimer;
    }
}

#if UNITY_EDITOR
public class WaitEditor : CutsceneElementEditor
{
    float time;
    public WaitEditor()
    {
        editorType = "Wait For";
        type = CutsceneElements.Wait;
    }

    public override void GenerateFromData(string[] data)
    {
        time = PCLParser.ParseFloat(data[0]);
    }

    public override string GenerateSaveData(bool json)
    {
        return PCLParser.CreateAttribute("Time", time);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        float t = EditorGUILayout.FloatField("Time", time);
        if (t > 0)
        {
            time = t;
        }
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("wait {0}", time);
        }
    }
}
#endif