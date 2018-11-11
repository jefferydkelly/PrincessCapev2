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
    float time;
 
    public CutsceneWait() {
        autoAdvance = true;
        type = CutsceneElements.Wait;
    }

    public override string SaveData
    {
        get
        {
            return PCLParser.CreateAttribute("Time", time);
        }
    }

    public override string ToText {
        get {
            return string.Format("wait {0}", time);
        }
    }

    public override void CreateFromText(string[] data)
    {
        time = float.Parse(data[1]);
    }

    public override void CreateFromJSON(string[] data)
    {
        time = PCLParser.ParseFloat(data[0]);
    }
#if UNITY_EDITOR
    public override void RenderEditor()
    {
        float t = EditorGUILayout.FloatField("Time", time);
        if (t > 0)
        {
            time = t;
        }
    }
#endif

    public override Timer Run()
    {
        return runTimer;
    }
}