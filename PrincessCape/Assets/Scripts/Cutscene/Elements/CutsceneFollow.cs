using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneFollow : CutsceneElement
{
    public string followedName;

    public CutsceneFollow() {
        canSkip = true;
        autoAdvance = true;
        type = CutsceneElements.Follow;
    }

    public override string SaveData
    {
        get
        {
            return PCLParser.CreateAttribute("Follow", followedName);
        }
    }

    public override string ToText {
        get
        {
            return string.Format("follow {0}", followedName);
        }
    }

    public override void CreateFromText(string[] data)
    {
        followedName = data[1];
    }

    public override void CreateFromJSON(string[] data)
    {
        followedName = PCLParser.ParseLine(data[0]);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        followedName = EditorGUILayout.TextField("Follow", followedName);
    }
#endif

    public override Timer Run()
    {
        CameraManager.Instance.Target = GameObject.Find(followedName);
        CameraManager.Instance.IsFollowing = true;
        return null;
    }

    public override void Skip()
    {
        Run();
    }
}