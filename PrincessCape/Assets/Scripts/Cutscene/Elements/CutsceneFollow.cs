using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

