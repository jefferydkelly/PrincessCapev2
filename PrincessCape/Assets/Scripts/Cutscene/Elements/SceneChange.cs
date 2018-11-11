using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : CutsceneElement
{
    string newScene;

    public SceneChange(string scene)
    {
        newScene = scene;
        canSkip = false;
        autoAdvance = true;
    }

    public override string SaveData
    {
        get
        {
            return "";
        }
    }

    public override string ToText {
        get {
            return "";
        }
    }

    public override void CreateFromJSON(string[] data)
    {

    }

    public override void CreateFromText(string[] data)
    {

    }

    public override void RenderEditor()
    {
   
    }

    public override Timer Run()
    {
        Game.Instance.LoadScene(newScene);
        return null;
    }
}
