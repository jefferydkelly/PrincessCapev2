using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

    List<string> message;
    Text textbox;
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
            textbox.alignment = TextAnchor.UpperLeft;
        });

        EventManager.StartListening("AlignRight", () => {
            textbox.alignment = TextAnchor.UpperRight;
        });
        textbox = GetComponentInChildren<Text>();
    }

    void DisplayLine() {
        if (!gameObject.activeSelf)
        {
            EventManager.StartListening("Inventory", Hide);
			EventManager.StartListening("EndCutscene", Hide);
            gameObject.SetActive(true);
        }


        textbox.text = ParseKeys(message[0]);
        curLine = 0;
    }

    string ParseKeys(string line) {
        while (line.Contains("[[["))
		{
			int start = line.IndexOf("[[[", System.StringComparison.Ordinal);
			int end = line.IndexOf("]]]", System.StringComparison.Ordinal);
            string sub = line.Substring(start, end - start + 3);
           
            string key = sub.Replace("[[[", "").Replace("]]]", "");
            line = line.Replace(sub, Controller.Instance.GetKey(key, true));

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
		textbox.text = "";
		revealTimer = new Timer(revealTime, message[curLine].Length - 1);
		revealTimer.OnTick.AddListener(RevealCharacter);
        Controller.Instance.AnyKey.AddListener(FastReveal);
        //EventManager.StartListening("AnyKey", FastReveal);
		revealTimer.OnComplete.AddListener(() => { 
            /*EventManager.StartListening("AnyKey", NextLine);*/ 
            Controller.Instance.AnyKey.AddListener(NextLine);
        });
		revealTimer.Start();
    }

    void FastReveal() {
        revealTimer.Stop();
        currentCharacter = message[curLine].Length;
        textbox.text = message[curLine].Substring(0, currentCharacter);
        EventManager.TriggerEvent("EndOfLine");
        Controller.Instance.AnyKey.RemoveListener(FastReveal);
        Controller.Instance.AnyKey.AddListener(NextLine);
        //EventManager.StopListening("AnyKey", FastReveal);
        //EventManager.StartListening("AnyKey", NextLine);
    }
    void RevealCharacter() {
        currentCharacter++;
        currentCharacter = Mathf.Min(message[curLine].Length, currentCharacter);
        textbox.text = message[curLine].Substring(0, currentCharacter);

        if (currentCharacter == message[curLine].Length) {
            EventManager.TriggerEvent("EndOfLine");
        }


    }
    void NextLine() {
        Controller.Instance.AnyKey.RemoveListener(NextLine);
        //EventManager.StopListening("AnyKey", NextLine);
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

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public List<string> Message {
        set {
            message = value;
        }

        get {
            return message;
        }
    }

    public bool IsComplete {
        get {
            if (curLine >= message.Count) {
                return false;
            }
            return currentCharacter == message[curLine].Length;
        }
    }
}
