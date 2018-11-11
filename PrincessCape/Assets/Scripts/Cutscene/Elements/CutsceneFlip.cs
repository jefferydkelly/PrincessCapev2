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

    public CutsceneFlip() {
        autoAdvance = true;
        canSkip = true;
        type = CutsceneElements.Flip;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Character", affected);
            data += PCLParser.CreateAttribute("Horizontal", horizontalFlip);
            return data;
        }
    }

    public override string ToText {
        get
        {
            return string.Format("flip-{0} {1}", horizontalFlip ? "x" : "y", affected);
        }
    }
    public override void CreateFromText(string[] data)
    {
        horizontalFlip = data[0].Contains("x");
        affected = data[1];
    }

    public override void CreateFromJSON(string[] data)
    {
        affected = PCLParser.ParseLine(data[0]);
        horizontalFlip = PCLParser.ParseBool(data[1]);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        affected = EditorGUILayout.TextField("Character", affected);
        horizontalFlip = EditorGUILayout.Toggle("Flip Horizontal", horizontalFlip);
    }
#endif
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

    public override void Skip()
    {
        Run();
    }
}