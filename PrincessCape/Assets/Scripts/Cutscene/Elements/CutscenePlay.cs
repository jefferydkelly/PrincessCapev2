using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlay : CutsceneElement {
    string clipName;

    public CutscenePlay(string clip) {
        clipName = clip;
    }
    public override Timer Run()
    {
        float length = SoundManager.Instance.PlaySound(clipName);
        return new Timer(length);
    }
}
