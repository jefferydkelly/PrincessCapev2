using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlay : CutsceneElement {
    AudioClip clip;

    public CutscenePlay(string clipPath) {
        clip = Resources.Load<AudioClip>("Sound Effects/" + clipPath);
    }
    public override Timer Run()
    {
        SoundManager.Instance.PlaySound(clip);
        return new Timer(clip.length);
    }
}
