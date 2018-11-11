using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Cutscene element.
/// </summary>
public abstract class CutsceneElement
{
    protected CutsceneElements type;
    public CutsceneElement nextElement = null;
    public CutsceneElement prevElement = null;
    protected bool canSkip = false;

    protected bool autoAdvance = false;
    protected Timer runTimer;
    bool show = true;

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

    public string ToJSON {
        get {
            string data = PCLParser.StructStart;
            data += PCLParser.CreateAttribute("Element Type", type);
            data += SaveData;
            data += PCLParser.StructEnd;
            return data;
        }
    }
    public abstract string SaveData {
        get;
    }

    public abstract string ToText {
        get;
    }
    #if UNITY_EDITOR
    public abstract void RenderEditor();
    public void RenderUI() {
        show = EditorGUILayout.Foldout(show, type.ToString());

        if (show) {
            EditorGUI.indentLevel++;
            RenderEditor();
            EditorGUI.indentLevel--;
        }

        
    }
    #endif
    public abstract void CreateFromJSON(string[] data);
    public abstract void CreateFromText(string[] data);

    protected GameObject FindActor(string actorName) {
        foreach(CutsceneActor actor in GameObject.FindObjectsOfType<CutsceneActor>()) {
            if (actor.CharacterName == actorName || actor.name == actorName) {
                return actor.gameObject;
            }
        }
        return null;
    }

    protected GameObject FindTile(string tileName) {
        foreach(MapTile mt in GameObject.FindObjectsOfType<MapTile>()) {
            if (mt.name == tileName || mt.InstanceName == tileName) {
                return mt.gameObject;
            }
        }

        return null;
    }

    protected Timer CreateTimer(float time) {
        return new Timer(1.0f / 30.0f, (int)(time * 30));
    }

    public CutsceneElements Type {
        get {
            return type;
        }
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