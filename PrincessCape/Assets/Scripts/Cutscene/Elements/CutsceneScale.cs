using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneScale : CutsceneElement
{
    ScaleType scaleType;
    float xScale = 1.0f;
    float yScale = 1.0f;
    float time = 0;
    string name;

    public CutsceneScale() {
        canSkip = true;
        autoAdvance = true;
        type = CutsceneElements.Scale;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Name", name);
            data += PCLParser.CreateAttribute("X", xScale);
            data += PCLParser.CreateAttribute("Y", yScale);
            data += PCLParser.CreateAttribute("Time", time);
            return data;
        }
    }

    public override string ToText {
        get
        {
            return string.Format("scale {0} {1} {2} {3}", name, xScale, yScale, time);
        }
    }

    public override void CreateFromText(string[] data)
    {
        name = data[1];
        xScale = float.Parse(data[2]);
        yScale = float.Parse(data[3]);
        time = float.Parse(data[4]);
    }

    public override void CreateFromJSON(string[] data)
    {
        name = PCLParser.ParseLine(data[0]);
        xScale = PCLParser.ParseFloat(data[1]);
        yScale = PCLParser.ParseFloat(data[2]);
        time = PCLParser.ParseFloat(data[3]);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        name = EditorGUILayout.TextArea("Name", name);
        float scale = EditorGUILayout.FloatField("X Scale", xScale);
        if (scale > 0)
        {
            xScale = scale;
        }
        scale = EditorGUILayout.FloatField("Y Scale", yScale);
        if (scale > 0)
        {
            yScale = scale;
        }

        scale = EditorGUILayout.FloatField("Time", time);
        if (scale > 0)
        {
            time = scale;
        }
    }
#endif

    public override Timer Run()
    {
        GameObject gameObject = null;
        CutsceneActor actor = Cutscene.Instance.FindActor(name);
        if (actor)
        {
            gameObject = actor.gameObject;
        }
        else
        {
            gameObject = GameObject.Find(name);
        }

        if (gameObject)
        {
            if (time > 0)
            {
                runTimer = CreateTimer(time);
                if (scaleType == ScaleType.All)
                {
                    float startScale = gameObject.transform.localScale.x;
                    float scaleDif = xScale - startScale;

                    runTimer.OnTick.AddListener(() =>
                    {
                        float curScale = startScale + scaleDif * runTimer.RunPercent;
                        gameObject.transform.localScale = new Vector3(curScale, curScale, 1);
                    });

                    runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.localScale = new Vector3(xScale, xScale, 1);
                    });
                }
                else if (scaleType == ScaleType.X)
                {
                    float startScale = gameObject.transform.localScale.x;
                    float scaleDif = xScale - startScale;

                    runTimer.OnTick.AddListener(() =>
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetX(startScale + scaleDif * runTimer.RunPercent);
                    });

                    runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale);
                    });

                }
                else if (scaleType == ScaleType.Y)
                {
                    float startScale = gameObject.transform.localScale.y;
                    float scaleDif = xScale - startScale;

                    runTimer.OnTick.AddListener(() =>
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetY(startScale + scaleDif * runTimer.RunPercent);
                    });

                    runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale.SetY(xScale);
                    });
                }
                else if (scaleType == ScaleType.Ind)
                {
                    Vector3 startScale = gameObject.transform.localScale;
                    Vector3 endScale = new Vector3(xScale, yScale);
                    Vector3 scaleDif = endScale - startScale;
                    scaleDif.z = 0;



                    runTimer.OnTick.AddListener(() =>
                    {
                        gameObject.transform.localScale = startScale + scaleDif * runTimer.RunPercent;
                    });

                    runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.localScale = endScale;
                    });

                }

                return runTimer;
            }
            else
            {
                if (scaleType == ScaleType.X)
                {
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale);
                }
                else if (scaleType == ScaleType.Y)
                {
                    gameObject.transform.localScale = gameObject.transform.localScale.SetY(xScale);

                }
                else if (scaleType == ScaleType.All)
                {
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale).SetY(xScale);
                }
                else if (scaleType == ScaleType.Ind)
                {
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale).SetY(yScale);
                }
            }
        }

        return null;
    }

    public override void Skip()
    {
        GameObject gameObject = null;
        CutsceneActor actor = Cutscene.Instance.FindActor(name);
        if (actor)
        {
            gameObject = actor.gameObject;
        }
        else
        {
            gameObject = GameObject.Find(name);
        }

        if (gameObject)
        {
            if (scaleType == ScaleType.X)
            {
                gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale);
            }
            else if (scaleType == ScaleType.Y)
            {
                gameObject.transform.localScale = gameObject.transform.localScale.SetY(xScale);

            }
            else if (scaleType == ScaleType.All)
            {
                gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale).SetY(xScale);
            }
            else if (scaleType == ScaleType.Ind)
            {
                gameObject.transform.localScale = gameObject.transform.localScale.SetX(xScale).SetY(yScale);
            }
        }
    }
}

public enum ScaleType
{
    All, X, Y, Ind
}