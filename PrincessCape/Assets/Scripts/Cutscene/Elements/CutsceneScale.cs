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
		GameObject gameObject = null;
        CutsceneActor actor = Cutscene.Instance.FindActor(actorName);
		if (actor)
		{
			gameObject = actor.gameObject;
		}
		else {
			gameObject = GameObject.Find(actorName);
		}

		if (gameObject)
		{
			if (time > 0)
			{
				runTimer = new Timer(1.0f / 30.0f, (int)(time * 30));
				if (type == ScaleType.All)
				{
					float startScale = gameObject.transform.localScale.x;
					float scaleDif = scale - startScale;
                    
					runTimer.OnTick.AddListener(() => {
						float curScale = startScale + scaleDif * runTimer.RunPercent;
                        gameObject.transform.localScale = new Vector3(curScale, curScale, 1);
                    });

					runTimer.OnComplete.AddListener(() => {
                        gameObject.transform.localScale = new Vector3(scale, scale, 1);
                    });
				}
				else if (type == ScaleType.X)
				{
					float startScale = gameObject.transform.localScale.x;
					float scaleDif = scale - startScale;

					runTimer.OnTick.AddListener(() => {
						gameObject.transform.localScale = gameObject.transform.localScale.SetX(startScale + scaleDif * runTimer.RunPercent);
                    });

					runTimer.OnComplete.AddListener(() => {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetX(scale);
                    });
                    
				}
				else if (type == ScaleType.Y)
				{
					float startScale = gameObject.transform.localScale.y;
                    float scaleDif = scale - startScale;

                    runTimer.OnTick.AddListener(() => {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetY(startScale + scaleDif * runTimer.RunPercent);
                    });

                    runTimer.OnComplete.AddListener(() => {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetY(scale);
                    });
				}
				else if (type == ScaleType.Ind)
				{
					Vector3 startScale = gameObject.transform.localScale;
					Vector3 endScale = new Vector3(scale, scale2);
					Vector3 scaleDif = endScale - startScale;
                    scaleDif.z = 0;

                    

					runTimer.OnTick.AddListener(() => {
						gameObject.transform.localScale = startScale + scaleDif * runTimer.RunPercent;
                    });

					runTimer.OnComplete.AddListener(() => {
						gameObject.transform.localScale = endScale;
                    });

				}

				return runTimer;
			} else {
				if (type == ScaleType.X) {
					gameObject.transform.localScale = gameObject.transform.localScale.SetX(scale);
				} else if (type == ScaleType.Y) {
					gameObject.transform.localScale = gameObject.transform.localScale.SetY(scale);

				} else if (type == ScaleType.All) {
					gameObject.transform.localScale = gameObject.transform.localScale.SetX(scale).SetY(scale);
				} else if (type == ScaleType.Ind) {
					gameObject.transform.localScale = gameObject.transform.localScale.SetX(scale).SetY(scale2);
				}
			}
		}

        return null;
    }
}

public enum ScaleType
{
    All, X, Y, Ind
}