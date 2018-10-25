using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Cutscene element.
/// </summary>
public class CutsceneElement
{
    public CutsceneElement nextElement = null;
    public CutsceneElement prevElement = null;
    protected bool canSkip = false;

    protected bool autoAdvance = false;
    protected Timer runTimer;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:CutsceneElement"/> advances automatically.
    /// </summary>
    /// <value><c>true</c> if auto advance; otherwise, <c>false</c>.</value>
	public bool AutoAdvance
    {
        get
        {
            return autoAdvance;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:CutsceneElement"/> can be skipped.
    /// </summary>
    /// <value><c>true</c> if can be skipped; otherwise, <c>false</c>.</value>
	public bool CanSkip
    {
        get
        {
            return canSkip;
        }
    }

    /// <summary>
    /// Run this instance.
    /// </summary>
	public virtual Timer Run()
    {
        return null;
    }

    /// <summary>
    /// Skip this instance.
    /// </summary>
    public virtual void Skip()
    {

    }
}

public enum CutsceneElements
{
    Activate,
    Add,
    Align,
    Animation,
    Creation,
    Destroy,
    Dialog,
    Enable,
    Fade,
    Flip,
    Follow,
    Hide,
    Movement,
    Pan,
    Play,
    Rotate,
    Scale,
    Show,
    Wait,
    Change
}

#if UNITY_EDITOR
/// <summary>
/// Cutscene element editor.
/// </summary>
public abstract class CutsceneElementEditor
{

    bool show = true;
    protected string editorType = "Element";
    protected CutsceneElements type;

    /// <summary>
    /// Render the GUI for this instance.
    /// </summary>
    public void Render()
    {
        EditorGUILayout.BeginVertical();
        show = EditorGUILayout.Foldout(show, editorType, true);
        if (show)
        {
            EditorGUI.indentLevel = 2;
            DrawGUI();

        }
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// Gets the save data.
    /// </summary>
    /// <value>The save data.</value>
    public string SaveData
    {
        get
        {
            string data = PCLParser.StructStart;
            data += PCLParser.CreateAttribute("Element Type", type);
            data += GenerateSaveData();
            data += PCLParser.StructEnd;
            return data;
        }
    }

    public abstract string HumanReadable { get; }

    protected abstract void DrawGUI();
    public abstract string GenerateSaveData();
    public abstract void GenerateFromData(string[] data);
    public abstract void GenerateFromText(string[] data);

    public GameObject FindActor(string name) {
        foreach(CutsceneActor actor in GameObject.FindObjectsOfType<CutsceneActor>()) {
            if (actor.CharacterName == name || actor.name == name) {
                return actor.gameObject;
            }
        }

        return null;
    }

    public GameObject FindTile(string name) {
        foreach(MapTile mt in GameObject.FindObjectsOfType<MapTile>()) {
            if (mt.name == name || mt.InstanceName == name) {
                return mt.gameObject;
            }
        }

        return null;
    }
}
#endif