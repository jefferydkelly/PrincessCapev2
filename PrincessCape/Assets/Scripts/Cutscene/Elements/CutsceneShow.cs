using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneShow : CutsceneElement
{

    string target;
    Vector3 position = Vector3.negativeInfinity;
    bool show;

    public CutsceneShow(string name, bool isShown)
    {
        target = name;
        show = isShown;

    }

    public CutsceneShow(string name, bool isShown, Vector3 pos) : this(name, isShown)
    {
        position = pos;
    }

    public override Timer Run()
    {
        CutsceneActor myActor = Cutscene.Instance.FindActor(target);
        if (myActor)
        {
            myActor.IsHidden = !show;
            if (show && position.x > float.NegativeInfinity)
            {
                myActor.Position = position;
            }
        }

        return null;
    }
}

#if UNITY_EDITOR
public class HideEditor : CutsceneElementEditor
{
    string hideName;

    public HideEditor()
    {
        editorType = "Hide Object";
        type = CutsceneElements.Hide;
    }
    public override void GenerateFromData(string[] data)
    {
        hideName = PCLParser.ParseLine(data[0]);
    }

    public override void GenerateFromText(string[] data)
    {
        hideName = data[1];
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("To Be Hidden", hideName);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        hideName = EditorGUILayout.TextField("To Be Hidden", hideName);
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("hide {0}", hideName);
        }
    }

    public override void Skip()
    {
        CutsceneActor actor = FindActor(hideName).GetComponent<CutsceneActor>();
        actor.IsHidden = true;
    }
}


public class ShowEditor : CutsceneElementEditor
{
    string name;
    Vector2 pos;

    public ShowEditor()
    {
        editorType = "Show Object";
        type = CutsceneElements.Show;
    }

    public override void GenerateFromText(string[] data)
    {
        name = data[1];
        if (data.Length > 2) {
            pos = new Vector2(float.Parse(data[2]), float.Parse(data[3]));
        }
    }
    public override void GenerateFromData(string[] data)
    {
        name = PCLParser.ParseLine(data[0]);
        pos = PCLParser.ParseVector2(data[1]);
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Name", name);
        data += PCLParser.CreateAttribute("Location", pos);
        return data;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        name = EditorGUILayout.TextField("Name", name);
        pos = EditorGUILayout.Vector2Field("Location", pos);
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("show {0} {1} {2}", name, pos.x, pos.y);
        }
    }

    public override void Skip()
    {
        CutsceneActor actor = FindActor(name).GetComponent<CutsceneActor>();
        actor.IsHidden = false;
        actor.transform.position = pos;
    }
}
#endif