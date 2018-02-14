using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

    static List<string> message;
    Text text;
    int curLine = 0;
    int currentCharacter = 0;
    float revealTime = 0.05f;
    Timer revealTimer;
    private void Awake()
    {
        gameObject.SetActive(false);
        EventManager.StartListening("ShowMessage", DisplayMessage);
        EventManager.StartListening("ShowLine", DisplayLine);
        text = GetComponentInChildren<Text>();
    }

    void DisplayLine() {
        if (!gameObject.activeSelf)
        {
            EventManager.StartListening("HideItemMenu", Hide);
            gameObject.SetActive(true);
        }
        text.text = message[0];
    }

    void Hide() {
        EventManager.StopListening("HideItemMenu", Hide);
        EventManager.StopListening("HideMessage", Hide);
        gameObject.SetActive(false);
    }
    void DisplayMessage() {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            EventManager.StartListening("HideMessage", Hide);
        }
        curLine = 0;
        StartReveal();


    }

    void StartReveal() {
		currentCharacter = 0;
		text.text = "";
		revealTimer = new Timer(revealTime, message[curLine].Length - 1);
		revealTimer.OnTick.AddListener(RevealCharacter);
		revealTimer.OnComplete.AddListener(() => { EventManager.StartListening("ItemOneActivated", NextLine); });
		revealTimer.Start();
    }
    void RevealCharacter() {
        currentCharacter++;
        text.text = message[curLine].Substring(0, currentCharacter);
    }
    void NextLine() {
        EventManager.StopListening("ItemOneActivated", NextLine);
        curLine++;
        if (curLine < message.Count) {
            StartReveal();
        } else {
            EventManager.TriggerEvent("EndOfMessage");
        }
    }

    public static void SetMessage(List<string> words) {
        message = words;
    }

    public static void SetLine(string line) {
        message = new List<string>() { line };
    }
}
