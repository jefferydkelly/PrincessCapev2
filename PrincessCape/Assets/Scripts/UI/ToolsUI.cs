using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsUI : MonoBehaviour {

    [SerializeField]
	Button moveButton;
    [SerializeField]
    Button rotateButton;
    [SerializeField]
    Button scaleButton;
    [SerializeField]
    Button flipButton;
    [SerializeField]
    Button alignButton;

    public MapEditMode Mode {
        set {
            moveButton.image.color = value == MapEditMode.Translate ? Color.red : Color.white;
            rotateButton.image.color = value == MapEditMode.Rotate ? Color.red : Color.white;
            scaleButton.image.color = value == MapEditMode.Scale ? Color.red : Color.white;
            flipButton.image.color = value == MapEditMode.Flip ? Color.red : Color.white;
            alignButton.image.color = value == MapEditMode.Align ? Color.red : Color.white;
        }
    }
}
