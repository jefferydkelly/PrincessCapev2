using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            runTimer.OnTick.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(startAlpha + alphaDelta * runTimer.RunPercent);
            });

            runTimer.OnComplete.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(alpha);
            });

            return runTimer;
        }

        return null;
    }
}

#if UNITY_EDITOR
public class FadeEditor : CutsceneElementEditor
{
    string actorName;
    bool fadeIn = true;
    float time;

    public FadeEditor()
    {
        editorType = "Fade In/Out";
        type = CutsceneElements.Fade;
    }
    public override void GenerateFromData(string[] data)
    {
        actorName = PCLParser.ParseLine(data[0]);
        fadeIn = PCLParser.ParseBool(data[1]);
        time = PCLParser.ParseFloat(data[2]);
    }

    public override string GenerateSaveData()
    {
        string data = PCLParser.CreateAttribute("Actor", actorName);
        data += PCLParser.CreateAttribute("Fade-In?", fadeIn);
        data += PCLParser.CreateAttribute("Over", time);
        return data;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        actorName = EditorGUILayout.TextField("Actor", actorName);
        fadeIn = EditorGUILayout.Toggle("Fade-In?", fadeIn);
        float newTime = EditorGUILayout.FloatField("Time (in seconds)", time);
        if (newTime > 0)
        {
            time = newTime;
        }
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("Fade-{0} {1} {2}", fadeIn ? "in" : "out", actorName, time);
        }
    }

    public override void GenerateFromText(string[] data)
    {
        fadeIn = data[0].Contains("in");
        actorName = data[1];
        time = float.Parse(data[2]);
    }
}
#endif