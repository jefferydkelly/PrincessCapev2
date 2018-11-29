using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpeakerBox : MonoBehaviour {
	Text text;
    string speaker = "";
	// Use this for initialization
	void Start () {
        gameObject.SetActive(Game.Instance.IsInCutscene);
		text = GetComponentInChildren<Text>();
        text.text = speaker;

	}
	
    public void Show() {
        gameObject.SetActive(true);
		UIManager.Instance.OnMessageEnd.AddListener(Hide);
        Cutscene.Instance.OnEnd.AddListener(Hide);
    }

    public void Hide() {
		UIManager.Instance.OnMessageEnd.RemoveListener(Hide);
        Cutscene.Instance.OnEnd.RemoveListener(Hide);
		gameObject.SetActive(false);
    }
    
	public string Speaker {
		set
		{
            if (text)
            {
                text.text = value;
            } else {
                speaker = value;
            }
		}

        get {
            if (text)
            {
                return text.text;

            }
            return speaker;
        }
    }
}
