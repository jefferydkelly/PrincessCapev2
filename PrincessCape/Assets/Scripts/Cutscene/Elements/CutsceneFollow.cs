using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraFollow : CutsceneElement
{
    public string targetName;

    public CameraFollow(string name)
    {
        targetName = name;
        canSkip = true;
        autoAdvance = true;
    }

    public override Timer Run()
    {
        CameraManager.Instance.Target = GameObject.Find(targetName);
        CameraManager.Instance.IsFollowing = true;
        return null;
    }
}

#if UNITY_EDITOR
public class FollowEditor : CutsceneElementEditor
{
    string followedName;
    public FollowEditor()
    {
        editorType = "Follow Character";
        type = CutsceneElements.Follow;
    }
    public override void GenerateFromData(string[] data)
    {
        followedName = PCLParser.ParseLine(data[0]);
    }

    public override string GenerateSaveData(bool json)
    {
        return PCLParser.CreateAttribute("Follow", followedName);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        followedName = EditorGUILayout.TextField("Follow", followedName);
    }
}
#endif