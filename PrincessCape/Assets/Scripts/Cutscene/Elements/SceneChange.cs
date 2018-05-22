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

    public override Timer Run()
    {
        Game.Instance.LoadScene(newScene);
        return null;
    }
}
