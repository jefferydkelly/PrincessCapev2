using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutscenePlay : CutsceneElement
{
    string clipName;

    public CutscenePlay(string clip)
    {
        clipName = clip;
    }
    public override Timer Run()
    {
        float length = SoundManager.Instance.PlaySound(clipName);
        return new Timer(length);
    }
}

#if UNITY_EDITOR
public class PlayEditor : CutsceneElementEditor {
    static string[] soundEffects;
    int selectedFX;
    public PlayEditor() {
        editorType = "Play Sound Effect";
        type = CutsceneElements.Play;
        if (soundEffects == null) {
            List<string> fx = new List<string>();
            foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Sounds")) {
                fx.Add(clip.name);
            }
            soundEffects = fx.ToArray();
        }
    }

    public override void GenerateFromData(string[] data)
    {
        selectedFX = ArrayUtility.IndexOf(soundEffects, PCLParser.ParseLine(data[0]));
    }

    public override string GenerateSaveData(bool json)
    {
        return PCLParser.CreateAttribute("Sound Effect", soundEffects[selectedFX]);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        selectedFX = EditorGUILayout.Popup("Sound Effect", selectedFX, soundEffects);
    }
}
#endif