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
    Vector3 panEnding;
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
        if (pType == PanType.ByAmount)
        {
            panDistance = PCLParser.ParseVector2(data[1]);
        }
        else if (pType == PanType.ToPosition)
        {
            panEnding = PCLParser.ParseVector3(data[1]);
        }
        else
        {
            panToName = PCLParser.ParseLine(data[1]);
        }

        panTime = PCLParser.ParseFloat(data[2]);

    }

    public override string GenerateSaveData(bool json)
    {
        string data = "";
        data += PCLParser.CreateAttribute("Pan Type", pType);
        if (pType == PanType.ByAmount)
        {
            data += PCLParser.CreateAttribute("Distance", panDistance);
        }
        else if (pType == PanType.ToPosition)
        {
            data += PCLParser.CreateAttribute("To", panEnding);
        }
        else
        {
            data += PCLParser.CreateAttribute("To", panToName);
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

        if (pType == PanType.ByAmount)
        {
            panDistance = EditorGUILayout.Vector2Field("By Amount", panDistance);
        }
        else if (pType == PanType.ToPosition)
        {
            panEnding = EditorGUILayout.Vector3Field("To", panEnding);
        }
        else
        {
            panToName = EditorGUILayout.TextField("To Character", panToName);
        }
        float time = EditorGUILayout.FloatField("Time", panTime);
        if (time > 0)
        {
            panTime = time;
        }
    }
}
#endif
