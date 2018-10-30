﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneMovement : CutsceneElement
{
    string mover = "Character";
    float x = 0;
    float y = 0;
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

                if (float.IsPositiveInfinity(x))
                {
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
                    }
                    else
                    {
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

#if UNITY_EDITOR
public class MovementEditor : CutsceneElementEditor
{
    bool useObject = true;
    GameObject gameObject;
    string name;
    Vector2 moveTo;
    float time;
    MoveType moveType;
    public MovementEditor()
    {
        editorType = "Move Object";
        type = CutsceneElements.Movement;
        moveType = MoveType.Regular;
    }

    public override void GenerateFromData(string[] data)
    {
        useObject = PCLParser.ParseBool(data[0]);
        if (useObject)
        {
            gameObject = GameObject.Find(PCLParser.ParseLine(data[1]));
        }
        else
        {
            name = PCLParser.ParseLine(data[1]);
        }
        moveTo = PCLParser.ParseVector2(data[2]);
        time = PCLParser.ParseFloat(data[3]);
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Use Object", useObject);
        data += PCLParser.CreateAttribute("Object Name", useObject ? gameObject.name : name);
        data += PCLParser.CreateAttribute("Move To", moveTo);
        data += PCLParser.CreateAttribute("Over", time);
        return data;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        useObject = EditorGUILayout.Toggle("Use GameObject?", useObject);
        if (useObject)
        {
            gameObject = EditorGUILayout.ObjectField("Object", gameObject, typeof(GameObject), true) as GameObject;
        }
        else
        {
            name = EditorGUILayout.TextField("Name", name);
        }
        moveTo = EditorGUILayout.Vector2Field("Move To", moveTo);
        float newTime = EditorGUILayout.FloatField("Over", time);
        if (newTime > 0)
        {
            time = newTime;
        }
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("move {0} {1} {2} {3}", useObject ? gameObject.name : name, moveTo.x, moveTo.y, time);
        }
    }

    public override void GenerateFromText(string[] data)
    {
        GameObject found = FindActor(data[1]);
        if (found)
        {
            useObject = true;
            gameObject = found;
        }
        else
        {
            useObject = false;
            name = data[1];
        }
        if (data[data.Length - 1] != "and")
        {
            time = float.Parse(data[data.Length - 1]);
        } else {
            time = float.Parse(data[data.Length - 2]);
        }
        if (data[0] == "move") {
            moveTo = new Vector2(float.Parse(data[2]), float.Parse(data[3]));
        } else if (data[0] == "move-x") {
            moveTo = new Vector2(float.Parse(data[2]), 0);
            moveType = MoveType.X;
        } else if (data[0] == "move-in") {
            moveTo = new Vector2(0, float.Parse(data[2]));
            moveType = MoveType.Y;
        }
    }

    public override void Skip()
    {
        GameObject found = gameObject;
        if (!useObject) {
            found = FindActor(name);
        }

        if (found) {
            if (moveType == MoveType.Regular)
            {
                found.transform.position = moveTo;
            } else if (moveType == MoveType.X) {
                found.transform.position = found.transform.position.SetX(moveTo.x);
            } else if (moveType == MoveType.Y) {
                found.transform.position = found.transform.position.SetY(moveTo.y);
            }
        }
    }
}

public enum MoveType {
    Regular,
    X,
    Y
}
#endif