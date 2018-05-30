using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneFade : CutsceneElement
{
    string actorName;
	GameObject target;
    float alpha;
    float time;
    public CutsceneFade(string actor, float toAlpha, float dt)
    {
        actorName = actor;
		target = Cutscene.Instance.FindGameObject(actor);
        alpha = toAlpha;
        time = dt;
        canSkip = true;
    }

    public override Timer Run()
	{
		if (!target)
		{
			target = Cutscene.Instance.FindGameObject(actorName);
		}

		if (target)
        {
			runTimer = new Timer(1.0f / 30.0f, (int)(time * 30));
			SpriteRenderer mySpriteRenderer = target.GetComponent<SpriteRenderer>();
			if (!target.activeSelf)
            {
                target.SetActive(true);
				mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(0);
            }

			Color col = mySpriteRenderer.color;
            float startAlpha = col.a;
			float alphaDelta = alpha - startAlpha;
            runTimer.OnTick.AddListener(() => {
				mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(startAlpha + alphaDelta * runTimer.RunPercent);
            });

			runTimer.OnComplete.AddListener(() => {
				mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(alpha);
            });

			return runTimer;
        }

        return null;
    }
}
