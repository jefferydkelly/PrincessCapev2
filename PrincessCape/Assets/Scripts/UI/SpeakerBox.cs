using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerBox : MonoBehaviour {
	static string speaker;
	Text text;
	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
        EventManager.StartListening("ShowDialog", Show);
		text = GetComponentInChildren<Text>();
	}
	
    void Show() {
        EventManager.StopListening("ShowDialog", Show);
        gameObject.SetActive(true);
        text.text = speaker;
        EventManager.StartListening("EndOfDialog", Hide);
        EventManager.StartListening("ShowMessage", Hide);
    }

    void Hide() {
        EventManager.StopListening("EndOfDialog", Hide);
        EventManager.StopListening("ShowMessage", Hide);
        gameObject.SetActive(false);
        EventManager.StartListening("ShowDialog", Show);
    }

    public static void SetSpeaker(string s) {
        speaker = s;
    }
}
