using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public enum PanType {
    ToPosition,
    ToCharacter,
    ByAmount
}

