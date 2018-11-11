using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Camera pan.
/// </summary>
public class CutscenePan : CutsceneElement
{
    Vector2 panDistance = Vector2.zero;
    Vector3 panEnding;
    bool panTo;
    string panToName = "";
    float panTime = 1.0f;
    PanType panType;

    public CutscenePan() {
        canSkip = true;
        autoAdvance = true;
        type = CutsceneElements.Pan;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Pan Type", panType);

            if (panType == PanType.ToCharacter)
            {
                data += PCLParser.CreateAttribute("To", panToName);
            }
            else
            {
                data += PCLParser.CreateAttribute(panType == PanType.ByAmount ? "Distance" : "To", panDistance);
            }
            data += PCLParser.CreateAttribute("Over", panTime);
            return data;
        }
    }

    public override string ToText {
        get
        {
            if (panType == PanType.ByAmount)
            {
                return string.Format("pan {0} {1} {2}", panDistance.x, panDistance.y, panTime);
            }
            else if (panType == PanType.ToPosition)
            {
                return string.Format("pan to {0} {1} {2}", panDistance.x, panDistance.y, panTime);
            }
            else
            {
                return string.Format("pan to {0} {1}", panToName, panTime);
            }
        }
    }

    public override void CreateFromText(string[] data)
    {
        if (data[data.Length - 1] != "and")
        {
            panTime = float.Parse(data[data.Length - 1]);
        }
        else
        {
            panTime = float.Parse(data[data.Length - 2]);
        }

        if (data[1] == "to")
        {
            if (data.Length == 4)
            {
                panType = PanType.ToCharacter;
                panToName = data[2];
            }
            else
            {
                panType = PanType.ToPosition;
                panDistance = new Vector2(float.Parse(data[2]), float.Parse(data[3]));

            }
        }
        else
        {
            panType = PanType.ByAmount;
            panDistance = new Vector2(float.Parse(data[1]), float.Parse(data[2]));
        }
    }

    public override void CreateFromJSON(string[] data)
    {
        panType = PCLParser.ParseEnum<PanType>(data[0]);

        if (panType == PanType.ToCharacter)
        {
            panToName = PCLParser.ParseLine(data[1]);
        }
        else
        {
            panDistance = PCLParser.ParseVector2(data[1]);
        }

        panTime = PCLParser.ParseFloat(data[2]);
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        panType = (PanType)EditorGUILayout.EnumPopup("Pan Type", panType);

        if (panType == PanType.ToCharacter)
        {
            panToName = EditorGUILayout.TextField("To Character", panToName);
        }
        else
        {
            panDistance = EditorGUILayout.Vector2Field(panType == PanType.ByAmount ? "By Amount" : "To", panDistance);
        }
        float time = EditorGUILayout.FloatField("Time", panTime);
        if (time > 0)
        {
            panTime = time;
        }
    }
#endif

    public override Timer Run()
    {
        if (panTo)
        {
            if (panToName.Length > 0)
            {
                return CameraManager.Instance.Pan(Cutscene.Instance.FindGameObject(panToName), panTime);
            }
            else
            {
                return CameraManager.Instance.PanTo(panEnding, panTime);
            }
        }
        else
        {
            return CameraManager.Instance.Pan(panDistance, panTime);
        }
    }

    public override void Skip()
    {
        if (panTo)
        {
            if (panToName.Length > 0)
            {
                GameObject gameObject = FindActor(panToName);
                Camera.main.transform.position = gameObject.transform.position.SetZ(Camera.main.transform.position.z);
            }
            else
            {
                Camera.main.transform.position = panEnding;
            }
        }
        else
        {
            Camera.main.transform.position += (Vector3)panDistance;
        }
    }
}

public enum PanType
{
    ToPosition,
    ToCharacter,
    ByAmount
}
