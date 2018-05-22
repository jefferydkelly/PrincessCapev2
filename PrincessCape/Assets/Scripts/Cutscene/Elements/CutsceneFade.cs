using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneFade : CutsceneElement
{
    string actorName;
    float alpha;
    float time;
    public CutsceneFade(string actor, float toAlpha, float dt)
    {
        actorName = actor;
        alpha = toAlpha;
        time = dt;
        canSkip = true;
    }

    public override Timer Run()
    {
        CutsceneActor actor = Cutscene.Instance.FindActor(actorName);
        if (actor)
        {
            actor.Fade(alpha, time);
        }

        return null;
    }
}
