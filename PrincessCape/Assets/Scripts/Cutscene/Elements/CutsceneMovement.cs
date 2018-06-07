using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveTypes
{
    X, Y, XY, Rotate
}

public class CutsceneMovement : CutsceneElement
{
    string mover = "Character";
    MoveTypes moveType = MoveTypes.XY;
    float x = 0;
    float y = 0;
    float ang = 0;
    float time = 0;

    public CutsceneMovement(string target, MoveTypes mt, float dx, float dy, float dt)
    {
        mover = target;
        moveType = mt;
        x = dx;
        y = dy;
        time = dt;
        canSkip = true;
		autoAdvance = true;
    }

    public CutsceneMovement(string target, MoveTypes mt, float angle, float dt)
    {
        mover = target;
        moveType = mt;
        if (mt == MoveTypes.Rotate)
        {
            ang = angle;
        }
        else if (mt == MoveTypes.X)
        {
            x = angle;
        }
        else if (mt == MoveTypes.Y)
        {
            y = angle;
        }

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
                runTimer.name = "Move Timer";

                Vector3 startPosition = gameObject.transform.position;
                if (moveType == MoveTypes.X)
                {
                    float xDist = x - gameObject.transform.position.x;
                    runTimer.OnTick.AddListener(() =>
                    {
                        gameObject.transform.position = gameObject.transform.position.SetX(startPosition.x + xDist * runTimer.RunPercent);
                    });

                    runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.position = startPosition.SetX(x);
                    });
                }
                else if (moveType == MoveTypes.Y)
                {
                    float yDist = y - gameObject.transform.position.y;
                    runTimer.OnTick.AddListener(() =>
                    {
                        gameObject.transform.position = gameObject.transform.position.SetY(startPosition.y + yDist * runTimer.RunPercent);
                    });

                    runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.position = startPosition.SetY(y);
                    });
                }
                else if (moveType == MoveTypes.XY)
                {
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
                }
                else if (moveType == MoveTypes.Rotate)
                {
                    runTimer.name = "Rotate Timer";
                    float curRotation = 0;
                    runTimer.OnTick.AddListener(() => {
                        gameObject.transform.Rotate(Vector3.forward, -curRotation);
                        curRotation = ang * runTimer.RunPercent;
                        gameObject.transform.Rotate(Vector3.forward, curRotation);
                    });

                    runTimer.OnComplete.AddListener(() => {
                        gameObject.transform.Rotate(Vector3.forward, -curRotation);
                        gameObject.transform.Rotate(Vector3.forward, ang);
                    });
                }

                return runTimer;

            }
            else
			{
                if (moveType == MoveTypes.X)
                {
                    gameObject.transform.position = gameObject.transform.position.SetX(x);
                }
                else if (moveType == MoveTypes.Y)
                {
                    gameObject.transform.position = gameObject.transform.position.SetY(y);
                }
                else if (moveType == MoveTypes.XY)
                {
                    gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);
                }
                else if (moveType == MoveTypes.Rotate)
                {
                    gameObject.transform.Rotate(Vector3.forward, ang);
                }
            }
        }
        return null;
    }
}

