using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneAlign : CutsceneElement
{
    bool left;

    public CutsceneAlign() {
        autoAdvance = true;
        canSkip = true;
        type = CutsceneElements.Align;
    }

    public override string SaveData
    {
        get
        {
            return PCLParser.CreateAttribute("Is Left?", left);
        }
    }

    public override string ToText {
        get {
            return string.Format("align {0}", left ? "left" : "right");
        }
    }

    public override void CreateFromText(string[] data)
    {
        left = data[1] == "left";
    }

    public override void CreateFromJSON(string[] data)
    {
        left = PCLParser.ParseBool(data[0]);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        left = EditorGUILayout.Toggle("Is Left Algined?", left);
    }
#endif
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

    public override void Skip()
    {
        Run();
    }
}