using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEffect : CutsceneElement
{
    EffectType type = EffectType.Show;
    string affected = "Character";
    float x = 0.0f;
    float y = 0.0f;

    public CutsceneEffect(string target, EffectType et)
    {
        affected = target;
        type = et;
        autoAdvance = true;
        canSkip = true;
    }

    public CutsceneEffect(string target, EffectType et, float dx, float dy)
    {
        affected = target;
        type = et;
        x = dx;
        y = dy;
        autoAdvance = true;
        canSkip = true;
    }

    public override Timer Run()
    {
        CutsceneActor myActor = Cutscene.Instance.FindActor(affected);
        if (type == EffectType.Show)
        {
           
            if (myActor)
            {

                Vector3 aPosition = new Vector3(x, y);

                myActor.Position = aPosition;
                myActor.IsHidden = false;

            }
        }
        else if (type == EffectType.Hide)
        {
            if (myActor && !myActor.IsHidden)
            {
                myActor.IsHidden = true;
            }
            //auto advance
        }
        else if (type == EffectType.FlipHorizontal)
        {
            myActor.FlipX();

        }
        else if (type == EffectType.FlipVertical)
        {
            myActor.FlipY();
        }

        return null;
    }
}

public enum EffectType
{
    Show, Hide, FlipHorizontal, FlipVertical
}
