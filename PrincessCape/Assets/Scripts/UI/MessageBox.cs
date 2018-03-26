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
        EventManager.StartListening("ShowDialog", DisplayMessage);
        EventManager.StartListening("ShowMessage", DisplayMessage);
        EventManager.StartListening("ShowLine", DisplayLine);
        EventManager.StartListening("AlignLeft", ()=>{
            text.alignment = TextAnchor.UpperLeft;
        });

        EventManager.StartListening("AlignRight", () => {
            text.alignment = TextAnchor.UpperRight;
        });
        text = GetComponentInChildren<Text>();
    }

    void DisplayLine() {
        if (!gameObject.activeSelf)
        {
            EventManager.StartListening("Inventory", Hide);
			EventManager.StartListening("EndCutscene", Hide);
            gameObject.SetActive(true);
        }


        text.text = ParseKeys(message[0]);
        curLine = 0;
    }

    string ParseKeys(string line) {
        while (line.Contains("[[["))
		{
			int start = line.IndexOf("[[[", System.StringComparison.Ordinal);
			int end = line.IndexOf("]]]", System.StringComparison.Ordinal);
            string sub = line.Substring(start, end - start + 3);
           
            string key = sub.Replace("[[[", "").Replace("]]]", "");
            line = line.Replace(sub, Controller.Instance.GetKey(key));

		}

        return line;
    }
    void Hide() {
        EventManager.StopListening("HideItemMenu", Hide);
        EventManager.StopListening("HideMessage", Hide);
        EventManager.StopListening("EndCutscene", Hide);
        gameObject.SetActive(false);
    }
    void DisplayMessage() {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            EventManager.StartListening("HideMessage", Hide);
			EventManager.StartListening("EndCutscene", Hide);
        }
        for (int i = 0; i < message.Count; i++) {
            message[i] = ParseKeys(message[i]);
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
        currentCharacter = Mathf.Max(text.text.Length, currentCharacter);
        text.text = message[curLine].Substring(0, currentCharacter);
    }
    void NextLine() {
        
        EventManager.StopListening("ItemOneActivated", NextLine);
        curLine++;
        if (curLine < message.Count)
        {
            StartReveal();
        }
        else
        {
            EventManager.TriggerEvent("EndOfMessage");
            if (Game.Instance.IsInCutscene)
            {
                EventManager.TriggerEvent("ElementCompleted");
            }
        }

    }

    public static void SetMessage(List<string> words) {
        message = words;
    }

    public static void SetLine(string line) {
        message = new List<string>() { line };
    }
}
