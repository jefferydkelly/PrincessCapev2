using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneFade : CutsceneElement
{
    bool useObject = false;
    string actorName;
    GameObject actor;
    bool fadeIn = false;
    float time;


    public CutsceneFade() {
        autoAdvance = true;
        canSkip = true;
        type = CutsceneElements.Fade;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Use Object?", useObject);
            data += PCLParser.CreateAttribute("Actor", useObject ? actor.name : actorName);
            data += PCLParser.CreateAttribute("Fade-In?", fadeIn);
            data += PCLParser.CreateAttribute("Over", time);
            return data;
        }
    }

    public override string ToText {
        get
        {
            return string.Format("Fade-{0} {1} {2}", fadeIn ? "in" : "out", useObject ? actor.name : actorName, time);
        }
    }

    public override void CreateFromText(string[] data)
    {
        fadeIn = data[0].Contains("in");
        GameObject found = GameObject.Find(data[1]);
        if (found == null)
        {
            //found = FindActor(data[1]);
        }
        if (found)
        {
            useObject = true;
            actor = found;
            actorName = actor.name;
        }
        else
        {
            useObject = false;
            actorName = data[1];
        }
        time = float.Parse(data[2]);
    }

    public override void CreateFromJSON(string[] data)
    {
        useObject = PCLParser.ParseBool(data[0]);
        if (useObject)
        {
            //actor = FindActor(data[1]);
        }
        else
        {
            actorName = PCLParser.ParseLine(data[1]);
        }
        fadeIn = PCLParser.ParseBool(data[2]);
        time = PCLParser.ParseFloat(data[3]);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        if (useObject)
        {
            actor = EditorGUILayout.ObjectField("Actor", actor, typeof(GameObject), true) as GameObject;
        }
        else
        {
            actorName = EditorGUILayout.TextField("Actor", actorName);
        }
        fadeIn = EditorGUILayout.Toggle("Fade-In?", fadeIn);
        float newTime = EditorGUILayout.FloatField("Time (in seconds)", time);
        if (newTime > 0)
        {
            time = newTime;
        }
    }
#endif

    public override Timer Run()
    {
        if (!actor)
        {
            actor = Cutscene.Instance.FindGameObject(actorName);
        }

        if (actor)
        {
            runTimer = new Timer(1.0f / 30.0f, (int)(time * 30));
            SpriteRenderer mySpriteRenderer = actor.GetComponent<SpriteRenderer>();
            if (!actor.activeSelf)
            {
                actor.SetActive(true);
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(0);
            }

            Color col = mySpriteRenderer.color;
            float startAlpha = col.a;
            float alphaDelta = fadeIn ? 1 : 0 - startAlpha;
            runTimer.OnTick.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(startAlpha + alphaDelta * runTimer.RunPercent);
            });

            runTimer.OnComplete.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(fadeIn ? 1 : 0);
            });

            return runTimer;
        }

        return null;
    }

    public override void Skip()
    {
        if (actor) {
            SpriteRenderer mySpriteRenderer = actor.GetComponent<SpriteRenderer>();
            if (!actor.activeSelf)
            {
                actor.SetActive(true);
            }

            mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(fadeIn ? 1 : 0);
        }
    }
}