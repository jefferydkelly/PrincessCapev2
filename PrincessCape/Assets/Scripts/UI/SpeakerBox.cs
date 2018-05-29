using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerBox : MonoBehaviour {
	Text text;
	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		text = GetComponentInChildren<Text>();
	}
	
    public void Show() {
        //EventManager.StopListening("ShowDialog", Show);
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
			text.text = value;
		}
    }
}
