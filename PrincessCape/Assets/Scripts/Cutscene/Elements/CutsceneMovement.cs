using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneMovement : CutsceneElement
{
    string mover = "Character";
    float x = 0;
    float y = 0;
    float ang = 0;
    float time = 0;

    public CutsceneMovement(string target, float dx = 0, float dy = 0, float dt = 0)
    {
        mover = target;
        x = dx;
        y = dy;
        time = dt;
        canSkip = true;
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
                runTimer.name = "Move Timer";

                Vector3 startPosition = gameObject.transform.position;

                if (float.IsPositiveInfinity(x)) {
                    x = startPosition.x;
                }

                if (float.IsPositiveInfinity(y))
                {
                    y = startPosition.y;
                }

                Vector3 dist = new Vector3(x, y) - gameObject.transform.position;

                runTimer.OnTick.AddListener(() =>
                {
					if (gameObject)
					{
						gameObject.transform.position = startPosition + dist * runTimer.RunPercent;
					} else {
						runTimer.Stop();
						runTimer.OnComplete.Invoke();
					}
                });

                runTimer.OnComplete.AddListener(() =>
                {
					if (gameObject)
					{
						gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);
					}
                });


                return runTimer;

            }
            else
			{
                gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);
            }
        }
        return null;
    }
}

