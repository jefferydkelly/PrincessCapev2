using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Camera pan.
/// </summary>
public class CameraPan : CutsceneElement
{
    Vector2 panDistance = Vector2.zero;
    Vector3 panEnding;
    bool panTo;
    string panToName = "";
    float panTime = 1.0f;

    /// <summary>
    /// Initializes a new <see cref="CameraPan"/>.
    /// </summary>
    /// <param name="pd">The distance which the Camera will be panned.</param>
    /// <param name="time">The duration of the pan.</param>
    public CameraPan(Vector2 pd, float time = 1.0f)
    {
        panDistance = pd;
        panTo = false;
        canSkip = true;
        autoAdvance = true;
    }

    /// <summary>
    /// Initializes a new <see cref="CameraPan"/> to the given location.
    /// </summary>
    /// <param name="pd">The ending of the pan</param>
    /// <param name="time">The duration of the pan</param>
    public CameraPan(Vector3 pd, float time = 1.0f)
    {
        panEnding = pd;
        panTo = true;
        canSkip = true;
        autoAdvance = true;
    }

    public CameraPan(string name, float time = 1.0f)
    {
        panToName = name;

        panTo = true;
        canSkip = true;
        panTime = time;
        autoAdvance = true;
    }

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
}

public enum PanType
{
    ToPosition,
    ToCharacter,
    ByAmount
}

#if UNITY_EDITOR
public class PanEditor : CutsceneElementEditor
{
    PanType pType = PanType.ToPosition;
    Vector2 panDistance = Vector2.zero;
    string panToName = "";
    float panTime = 1.0f;

    public PanEditor()
    {
        editorType = "Camera Pan";
        type = CutsceneElements.Pan;
    }

    public override void GenerateFromData(string[] data)
    {
        pType = PCLParser.ParseEnum<PanType>(data[0]);

        if (pType == PanType.ToCharacter)
        {
            panToName = PCLParser.ParseLine(data[1]);
        } else {
            panDistance = PCLParser.ParseVector2(data[1]);
        }

        panTime = PCLParser.ParseFloat(data[2]);

    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Pan Type", pType);
       
        if (pType == PanType.ToCharacter)
        {
            data += PCLParser.CreateAttribute("To", panToName);
        } else {
            data += PCLParser.CreateAttribute(pType == PanType.ByAmount ? "Distance" : "To", panDistance);
        }
        data += PCLParser.CreateAttribute("Over", panTime);
        return data;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        pType = (PanType)EditorGUILayout.EnumPopup("Pan Type", pType);

        if (pType == PanType.ToCharacter)
        {
            panToName = EditorGUILayout.TextField("To Character", panToName);
        } else {
            panDistance = EditorGUILayout.Vector2Field(pType == PanType.ByAmount ? "By Amount" : "To", panDistance);
        }
        float time = EditorGUILayout.FloatField("Time", panTime);
        if (time > 0)
        {
            panTime = time;
        }
    }

    public override string HumanReadable
    {
        get
        {
            if (pType == PanType.ByAmount) {
                return string.Format("pan {0} {1} {2}", panDistance.x, panDistance.y, panTime);
            } else if (pType == PanType.ToPosition) {
                return string.Format("pan to {0} {1} {2}", panDistance.x, panDistance.y, panTime);
            } else {
                return string.Format("pan to {0} {1}", panToName, panTime);
            }
        }
    }

    public override void GenerateFromText(string[] data)
    {
        if (data[data.Length - 1] != "and") {
            panTime = float.Parse(data[data.Length - 1]);
        } else {
            panTime = float.Parse(data[data.Length - 2]);
        }

        if (data[1] == "to") {
            if (data.Length == 4) {
                pType = PanType.ToCharacter;
                panToName = data[2];
            } else {
                pType = PanType.ToPosition;
                panDistance = new Vector2(float.Parse(data[2]), float.Parse(data[3]));

            }
        } else {
            pType = PanType.ByAmount;
            panDistance = new Vector2(float.Parse(data[1]), float.Parse(data[2]));
        }
    }
}
#endif
