using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Sign : InteractiveObject {
    [SerializeField]
    TextAsset text;

    public override void Interact()
    {
        Cutscene.Instance.LoadTextFile(text);
        Cutscene.Instance.StartCutscene();
    }

    public TextAsset Text {
        get {
            return text;
        }

        set {
            text = value;
        }
    }

}
