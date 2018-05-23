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
        textbox = GetComponentInChildren<Text>();
		revealTimer = new Timer(0);
		/*
        EventManager.StartListening("ShowDialog", DisplayMessage);
        EventManager.StartListening("ShowMessage", DisplayMessage);
        EventManager.StartListening("ShowLine", DisplayLine);
        EventManager.StartListening("AlignLeft", ()=>{
            textbox.alignment = TextAnchor.UpperLeft;
        });

        EventManager.StartListening("AlignRight", () => {
            textbox.alignment = TextAnchor.UpperRight;
        });
        */
        
    }

    public void DisplayLine() {
        if (!gameObject.activeSelf)
        {
            EventManager.StartListening("Inventory", Hide);
            Cutscene.Instance.OnEnd.AddListener(Hide);
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
        Cutscene.Instance.OnEnd.RemoveListener(Hide);
        gameObject.SetActive(false);
    }
    void DisplayMessage() {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            EventManager.StartListening("HideMessage", Hide);
			Cutscene.Instance.OnEnd.AddListener(Hide);
        }
        for (int i = 0; i < message.Count; i++) {
            message[i] = ParseKeys(message[i]);
        }
        curLine = 0;
		StartReveal();
    }

	public Timer StartReveal() {
		if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            //EventManager.StartListening("HideMessage", Hide);
            Cutscene.Instance.OnEnd.AddListener(Hide);
        }
		currentCharacter = 0;
		curLine = 0;
		textbox.text = "";
		revealTimer = new Timer(revealTime, message[curLine].Length - 1);
		revealTimer.name = message[0];
		revealTimer.OnTick.AddListener(RevealCharacter);
        Controller.Instance.AnyKey.AddListener(FastReveal);
		if (message.Count > 1)
		{
			Debug.Log("Let's check for the next line");
			revealTimer.OnComplete.AddListener(() =>
			{
				Controller.Instance.AnyKey.AddListener(NextLine);
			});
		}
		return revealTimer;
    }

    void FastReveal() {
		if (revealTimer.IsRunning)
		{
			revealTimer.Stop();
			revealTimer.OnComplete.Invoke();
			currentCharacter = message[curLine].Length;
			textbox.text = message[curLine].Substring(0, currentCharacter);
			EventManager.TriggerEvent("EndOfLine");
			Controller.Instance.AnyKey.RemoveListener(FastReveal);
			if (message.Count > 1)
			{
				Controller.Instance.AnyKey.AddListener(NextLine);
			}
		}
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
		Debug.Log("Next");
		Debug.Break();
        curLine++;
        if (curLine < message.Count)
        {
            StartReveal();
        }
        else
        {
			EventManager.TriggerEvent("EndOfMessage");
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

	public bool IsRevealing {
		get {
			return revealTimer.IsRunning;
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

	public TextAnchor Alignment {
		get {
			return textbox.alignment;
		}

		set {
			textbox.alignment = value;
		}
	}
}
