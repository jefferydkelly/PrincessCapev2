using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneAnimation : CutsceneElement
{
    string triggerName;
    string actorName;

    public CutsceneAnimation(string aName, string tName)
    {
        triggerName = tName;
        actorName = aName;
    }

    public override Timer Run()
    {
        GameObject gameObject = GameObject.Find(actorName);
        if (gameObject)
        {
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator)
            {
                animator.SetTrigger(triggerName);
                runTimer = new Timer(0.5f);
                return runTimer;
            }
        }
        else
        {
            CutsceneActor cutsceneActor = Cutscene.Instance.FindActor(actorName);
            if (cutsceneActor)
            {
                cutsceneActor.Animate(triggerName);
                runTimer = new Timer(0.5f);
                return runTimer;
            }
        }
        return null;
    }
}

