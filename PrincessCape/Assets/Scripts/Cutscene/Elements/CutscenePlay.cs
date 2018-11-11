using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutscenePlay : CutsceneElement
{
    string clipName;
    static string[] soundEffects;
    int selectedFX;

    public CutscenePlay() {
        if (soundEffects == null)
        {
            List<string> fx = new List<string>();
            foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Sounds"))
            {
                fx.Add(clip.name);
            }
            soundEffects = fx.ToArray();
        }

        type = CutsceneElements.Play;
    }

    public override string SaveData
    {
        get
        {
            return PCLParser.CreateAttribute("Sound Effect", soundEffects[selectedFX]);
        }
    }

    public override string ToText {
        get {
            return string.Format("play {0}", soundEffects[selectedFX]);
        }
    }

    public override void CreateFromText(string[] data)
    {
        clipName = data[1];
        selectedFX = ArrayUtility.IndexOf(soundEffects, clipName);

    }

    public override void CreateFromJSON(string[] data)
    {
        clipName = data[0];
        selectedFX = ArrayUtility.IndexOf(soundEffects, PCLParser.ParseLine(clipName));
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        selectedFX = EditorGUILayout.Popup("Sound Effect", selectedFX, soundEffects);
    }
#endif

    public override Timer Run()
    {
        float length = SoundManager.Instance.PlaySound(clipName);
        return new Timer(length);
    }
}