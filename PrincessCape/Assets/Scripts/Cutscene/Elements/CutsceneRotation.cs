using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneRotation : CutsceneElement {
    string mover;
    float angle;
    float time;
    public CutsceneRotation(string target, float ang, float dt)
    {
        mover = target;
        angle = ang;
        time = dt;
        autoAdvance = true;
    }

    public override Timer Run()
    {
        CutsceneActor actor = Cutscene.Instance.FindActor(mover);
        GameObject gameObject = actor ? actor.gameObject : GameObject.Find(mover);

        if (gameObject)
        {
            if (time > 0)
            {
                runTimer = new Timer(1.0f / 30.0f, (int)(time * 30));
                runTimer.name = "Rotate Timer";
                float curRotation = 0;
                runTimer.OnTick.AddListener(() =>
                {
                    gameObject.transform.Rotate(Vector3.forward, -curRotation);
                    curRotation = angle * runTimer.RunPercent;
                    gameObject.transform.Rotate(Vector3.forward, curRotation);
                });

                runTimer.OnComplete.AddListener(() =>
                {
                    gameObject.transform.Rotate(Vector3.forward, -curRotation);
                    gameObject.transform.Rotate(Vector3.forward, angle);
                });
                return runTimer;
            } else {
                gameObject.transform.Rotate(Vector3.forward, angle);
            }
        }
        return null;
    }
}
