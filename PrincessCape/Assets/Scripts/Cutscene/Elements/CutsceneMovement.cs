using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneMovement : CutsceneElement
{
    bool useObject;
    GameObject gameObject;
    string name = "Character";
    Vector3 moveTo;
    float time = 0;
    MoveType moveType;

    public CutsceneMovement() {
        canSkip = true;
        autoAdvance = true;
        type = CutsceneElements.Movement;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Use Object", useObject);
            data += PCLParser.CreateAttribute("Object Name", useObject ? gameObject.name : name);
            data += PCLParser.CreateAttribute("Move To", moveTo);
            data += PCLParser.CreateAttribute("Over", time);
            return data;
        }
    }

    public override string ToText
    {
        get
        {
            return string.Format("move {0} {1} {2} {3}", useObject ? gameObject.name : name, moveTo.x, moveTo.y, time);
        }
    }

    public override void CreateFromText(string[] data)
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
        }
        else
        {
            time = float.Parse(data[data.Length - 2]);
        }
        if (data[0] == "move")
        {
            moveTo = new Vector3(float.Parse(data[2]), float.Parse(data[3]));
        }
        else if (data[0] == "move-x")
        {
            moveTo = new Vector3(float.Parse(data[2]), 0);
            moveType = MoveType.X;
        }
        else if (data[0] == "move-y")
        {
            moveTo = new Vector3(0, float.Parse(data[2]));
            moveType = MoveType.Y;
        }
    }

    public override void CreateFromJSON(string[] data)
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

#if UNITY_EDITOR
    public override void RenderEditor()
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
#endif

    public override Timer Run()
    {
        if (gameObject == null)
        {
            CutsceneActor actor = Cutscene.Instance.FindActor(name);
            gameObject = actor ? actor.gameObject : GameObject.Find(name);
        }

        if (gameObject)
        {
            if (time > 0)
            {
                runTimer = CreateTimer(time);
                runTimer.name = "Move Timer";

                Vector3 startPosition = gameObject.transform.position;
                moveTo = moveTo.SetZ(startPosition.z);
                if (moveType == MoveType.Y)
                {
                    moveTo = moveTo.SetX(startPosition.x);
                }

                if (moveType == MoveType.X)
                {
                    moveTo = moveTo.SetY(startPosition.y);
                }

                Vector3 dist = (Vector3)moveTo - gameObject.transform.position;

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
                        gameObject.transform.position = moveTo;
                    }
                });

                return runTimer;

            }
            else
            {
                gameObject.transform.position = moveTo;
            }
        }
        return null;
    }

    public override void Skip()
    {

        if (gameObject == null)
        {
            gameObject = Cutscene.Instance.FindGameObject(name);
        }

        if (gameObject)
        {
            Vector3 startPosition = gameObject.transform.position;

            if (moveType == MoveType.Y)
            {
                moveTo = moveTo.SetX(startPosition.x);
            }

            if (moveType == MoveType.X)
            {
                moveTo = moveTo.SetY(startPosition.y);
            }

            gameObject.transform.position = moveTo.SetZ(startPosition.z);
        }
    }
}

public enum MoveType
{
    Regular,
    X,
    Y
}