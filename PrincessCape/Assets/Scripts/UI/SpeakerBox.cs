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
        EventManager.StartListening("EndOfDialog", Hide);
        Cutscene.Instance.OnEnd.AddListener(Hide);
        EventManager.StartListening("ShowMessage", Hide);
    }

    public void Hide() {
        EventManager.StopListening("EndOfDialog", Hide);
        Cutscene.Instance.OnEnd.RemoveListener(Hide);
        EventManager.StopListening("ShowMessage", Hide);
        gameObject.SetActive(false);
        EventManager.StartListening("ShowDialog", Show);
    }
    
	public string Speaker {
		set
		{
			text.text = value;
		}
    }
}
