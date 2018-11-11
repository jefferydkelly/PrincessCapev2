using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneRotation : CutsceneElement
{
    string mover;
    float angle;
    float time;

    public CutsceneRotation() {
        autoAdvance = true;
        type = CutsceneElements.Rotate;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Character", mover);
            data += PCLParser.CreateAttribute("Angle", angle);
            data += PCLParser.CreateAttribute("Time", time);
            return data;
        }
    }

    public override string ToText {
        get
        {
            return string.Format("rotate {0} {1} {2}", mover, angle, time);
        }
    }

    public override void CreateFromText(string[] data)
    {
        mover = data[1];
        angle = float.Parse(data[2]);
        time = float.Parse(data[3]);
    }

    public override void CreateFromJSON(string[] data)
    {
        mover = PCLParser.ParseLine(data[0]);
        angle = PCLParser.ParseFloat(data[1]);
        time = PCLParser.ParseFloat(data[2]);
    }

    public override void RenderEditor()
    {
        mover = EditorGUILayout.TextField("Character", mover);
        angle = EditorGUILayout.FloatField("Angle", angle);
        float t = EditorGUILayout.FloatField("Time", time);
        if (t > 0)
        {
            time = t;
        }
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
            }
            else
            {
                
            }
        }
        return null;
    }

    public override void Skip()
    {
        CutsceneActor actor = Cutscene.Instance.FindActor(mover);
        GameObject gameObject = actor ? actor.gameObject : GameObject.Find(mover);

        if (gameObject)
        {
            gameObject.transform.Rotate(Vector3.forward, angle);
        }
    }
}