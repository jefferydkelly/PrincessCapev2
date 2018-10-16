using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneShow : CutsceneElement {

    string target;
    Vector3 position = Vector3.negativeInfinity;
    bool show;
	
    public CutsceneShow(string name, bool isShown) {
        target = name;
        show = isShown;

    }

    public CutsceneShow(string name, bool isShown, Vector3 pos):this(name, isShown) {
        position = pos;
    }

    public override Timer Run()
    {
        CutsceneActor myActor = Cutscene.Instance.FindActor(target);
        if (myActor) {
            myActor.IsHidden = !show;
            if (show && position.x > float.NegativeInfinity) {
                myActor.Position = position;
            }
        }

        return null;
    }
}
