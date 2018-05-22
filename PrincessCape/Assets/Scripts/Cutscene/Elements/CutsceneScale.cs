using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneScale : CutsceneElement
{
    ScaleType type;
    float scale = 1.0f;
    float scale2 = 1.0f;
    float time = 0;
    string actorName;
    public CutsceneScale(ScaleType st, string actor, float sc, float dt)
    {
        actorName = actor;
        type = st;
        scale = sc;
        time = dt;
        canSkip = true;
    }

    public CutsceneScale(string actor, float sc1, float sc2, float dt)
    {
        actorName = actor;
        type = ScaleType.Ind;
        scale = sc1;
        scale2 = sc2;
        time = dt;
        canSkip = true;
    }

    public override Timer Run()
    {
        CutsceneActor actor = Cutscene.Instance.FindActor(actorName);
        if (type == ScaleType.All)
        {
            actor.Scale(scale, time);
        }
        else if (type == ScaleType.X)
        {
            actor.ScaleX(scale, time);
        }
        else if (type == ScaleType.Y)
        {
            actor.ScaleY(scale, time);
        }
        else if (type == ScaleType.Ind)
        {
            actor.ScaleXY(new Vector3(scale, scale2, 1), time);
        }

        return null;
    }
}

public enum ScaleType
{
    All, X, Y, Ind
}