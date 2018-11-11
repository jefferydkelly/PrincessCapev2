using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneShow : CutsceneElement
{
    
    string target;
    CutsceneActor actor;
    bool useGameObject = false;
    Vector2 position = Vector2.negativeInfinity;
    bool show;

    public CutsceneShow(bool isShown) {
        show = isShown;
        type = show ? CutsceneElements.Show : CutsceneElements.Hide;
    }
    public override string SaveData
    {
        get
        {
            string data = PCLParser.CreateAttribute("Target", target);
            if (show) {
                data += PCLParser.CreateAttribute("Position", position);
            }
            return data;
        }
    }

    public override string ToText {
        get
        {
            if (show)
            {
                return string.Format("hide {0}", target);
            } else {
                return string.Format("show {0} {1} {2}", target, position.x, position.y);
            }       
        }
        
    }

    public override void CreateFromText(string[] data)
    {
        target = data[1];
        GameObject gameObject = FindActor(target);
        useGameObject = actor != null;
        if (useGameObject) {
            actor = gameObject.GetComponent<CutsceneActor>();
        }
       

        if (data.Length > 2)
        {
            position = new Vector2(float.Parse(data[2]), float.Parse(data[3]));
        }
    }

    public override void CreateFromJSON(string[] data)
    {
        target = PCLParser.ParseLine(data[0]);
        actor = FindActor(target).GetComponent<CutsceneActor>();
        useGameObject = actor != null;
        if (show) {
            position = PCLParser.ParseVector2(data[1]);
        }
    }
#if UNITY_EDITOR
    public override void RenderEditor()
    {
        useGameObject = EditorGUILayout.Toggle("Use Game Object?", useGameObject);
        if (useGameObject)
        {
            actor = EditorGUILayout.ObjectField("Game Object", actor, typeof(CutsceneActor), true) as CutsceneActor;
        }
        else
        {
            target = EditorGUILayout.TextField("Name", target);
        }
        show = EditorGUILayout.Toggle("Show Object?", show);
        if (show)
        {
            position = EditorGUILayout.Vector2Field("Position", position);
        }
    }
#endif

    public override Timer Run()
    {
        if (actor == null)
        {
            actor = Cutscene.Instance.FindActor(target);
        }
        if (actor)
        {
            actor.IsHidden = !show;
            if (show && position.x > float.NegativeInfinity)
            {
                actor.Position = position;
            }
        }

        return null;
    }

    public override void Skip()
    {
        Run();
    }
}