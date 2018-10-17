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
            }
            else
            {
                gameObject.transform.Rotate(Vector3.forward, angle);
            }
        }
        return null;
    }
}

#if UNITY_EDITOR
public class RotationEditor : CutsceneElementEditor
{
    string mover = "";
    float ang = 0;
    float time = 0;

    public RotationEditor() {
        editorType = "Rotate Object";
        type = CutsceneElements.Rotate;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        mover = EditorGUILayout.TextField("Character", mover);
        ang = EditorGUILayout.FloatField("Angle", ang);
        float t = EditorGUILayout.FloatField("Time", time);
        if (t > 0) {
            time = t;
        }
    }

    public override string GenerateSaveData(bool json)
    {
        string data = "";
        data += PCLParser.CreateAttribute("Character", mover);
        data += PCLParser.CreateAttribute("Angle", ang);
        data += PCLParser.CreateAttribute("Time", time);
        return data;
    }

    public override void GenerateFromData(string[] data)
    {
        mover = PCLParser.ParseLine(data[0]);
        ang = PCLParser.ParseFloat(data[1]);
        time = PCLParser.ParseFloat(data[2]);
    }
}

#endif