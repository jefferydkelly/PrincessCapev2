using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneFlip : CutsceneElement{
    string affected = "Character";
    bool horizontalFlip = false;
    public CutsceneFlip(string target, bool horizontal) {
        affected = target;
        horizontalFlip = horizontal;
        autoAdvance = true;
    }

    public override Timer Run()
    {
        CutsceneActor myActor = Cutscene.Instance.FindActor(affected);
        if (horizontalFlip) {
            myActor.FlipX();
        } else {
            myActor.FlipY();
        }
        return null;
    }
}
